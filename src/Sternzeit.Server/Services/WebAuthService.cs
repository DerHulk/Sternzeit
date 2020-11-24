using IdentityModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class WebAuthService
    {
        public ValidationResult ValidateRegistration(RegistrationData registrationData, RegistrationExpectations expectation)
        {
            if (registrationData.ClientData.Type != "webauthn.create")
                return new ValidationResult("Incorrect client data type", new[] { nameof(registrationData.ClientData.Type) });

            if (!Base64Url.Decode(registrationData.ClientData.Challenge).SequenceEqual(
                    Base64Url.Decode(expectation.Challenge)))
                return new ValidationResult("Incorrect challenge", new[] { nameof(registrationData.ClientData.Challenge) });

            if (!expectation.Origin.Contains(registrationData.ClientData.Origin))
                return new ValidationResult("Incorrect origin", new[] { nameof(registrationData.ClientData.Origin) });            

            //???
            // Verify that the value of C.tokenBinding.status matches the state of Token Binding for the TLS connection over which the assertion was obtained.
            // If Token Binding was used on that TLS connection, also verify that C.tokenBinding.id matches the base64url encoding of the Token Binding ID for the connection.
            // TODO: Token binding (once out of draft)
            if (!expectation.RelayingPartyHash.SequenceEqual(registrationData.Attestion.RelayingPartyHash))
                return new ValidationResult("Incorrect RP ID", new[] { nameof(registrationData.ClientData.Origin) });            

            // If user verification is required for this registration, verify that the User Verified bit of the flags in authData is set.
            // TODO: Handle user verificaton required

            // If user verification is not required for this registration, verify that the User Present bit of the flags in authData is set.
            if (!registrationData.Attestion.IsUserPresent)
                return new ValidationResult("User not present", new[] { nameof(registrationData.ClientData.Origin) });

            // Verify that the values of the client extension outputs in clientExtensionResults
            // TODO: Handle extension results

            // Determine the attestation statement format by performing a USASCII case-sensitive match on fmt against the set of supported WebAuthn Attestation Statement Format Identifier values
            // TODO: Handle accepted fmt values

            // Verify that attStmt is a correct attestation statement, conveying a valid attestation signature, by using the attestation statement format fmt’s verification procedure given attStmt, authData and the hash of the serialized client data computed in step 7.
            // TODO: Handle fmt specific attestation statement

            // If validation is successful, obtain a list of acceptable trust anchors (attestation root certificates or ECDAA-Issuer public keys) for that attestation type and attestation statement format fmt, from a trusted source or from policy.
            // For example, the FIDO Metadata Service [FIDOMetadataService] provides one way to obtain such information, using the aaguid in the attestedCredentialData in authData.
            // 16. Assess the attestation trustworthiness using the outputs of the verification procedure in step 14
            // TODO: Use of FIDO metadata service           
            return ValidationResult.Success;
        }

        public ValidationResult IsValidLogin(LoginData loginData, LoginExpectations expectation)
        {
            //Verify that the value of C.type is the string webauthn.get.
            if (loginData.ClientData.Type != "webauthn.get")
                return new ValidationResult("Incorrect client data type", new[] { nameof(loginData.ClientData.Type) });            

            //Verify that the value of C.challenge matches the challenge that was sent to the authenticator in the PublicKeyCredentialRequestOptions passed to the get() call.          
            if (!Base64Url.Decode(loginData.ClientData.Challenge).SequenceEqual(Base64Url.Decode(expectation.Challenge)))
                return new ValidationResult("Incorrect challenge", new[] { nameof(loginData.ClientData.Challenge) });            

            //Verify that the value of C.origin matches the Relying Party's origin.
            if (!expectation.Origins.Contains(loginData.ClientData.Origin))
                return new ValidationResult("Incorrect origin", new[] { nameof(loginData.ClientData.Origin) });            

            //Verify that the value of C.tokenBinding.status matches the state of Token Binding for the TLS connection over which the attestation was obtained.
            // If Token Binding was used on that TLS connection, also verify that C.tokenBinding.id matches the base64url encoding of the Token Binding ID for the connection.
            // TODO: Token binding once out of draft

            if (!expectation.RelayingPartyHash.SequenceEqual(loginData.Assertion.RelayingPartyHash))
                return new ValidationResult("Incorrect RelayingParty ID", new[] { nameof(loginData.Assertion.RelayingPartyHash) });            

            //If user verification is required for this assertion, verify that the User Verified bit of the flags in aData is set.
            // TODO: Handle user verificaton required

            //If user verification is not required for this assertion, verify that the User Present bit of the flags in aData is set.
            if (!loginData.Assertion.IsUserPresent)
                return new ValidationResult("User not present", new[] { nameof(loginData.Assertion.IsUserPresent) });            

            //Verify that the values of the client extension outputs in clientExtensionResults
            // TODO: Handle extension results

            // verify that signatur is a valid signature of the private Key over the binary concatenation of aData and hash.           
            var sigBase = new byte[loginData.Assertion.SourceAsBase64.Length + loginData.ClientData.Hash.Length];

            loginData.Assertion.SourceAsBase64.CopyTo(sigBase, 0);
            loginData.ClientData.Hash.CopyTo(sigBase, loginData.Assertion.SourceAsBase64.Length);

            var ecDsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    X = Base64Url.Decode(expectation.PublicUserKey.X),
                    Y = Base64Url.Decode(expectation.PublicUserKey.Y)
                }
            });

            var isValid = ecDsa.VerifyData(sigBase, DeserializeSignature(loginData.Signatur), HashAlgorithmName.SHA256);

            if (isValid)
            {
                if (expectation.Counter <= loginData.Assertion.Counter)
                    return new ValidationResult("Possible cloned authenticator", new[] { nameof(loginData.Assertion.Counter) });                
            }
            else
            {
                return new ValidationResult("Invalid Signature", new[] { nameof(loginData.Signatur) });                
            }

            return ValidationResult.Success;
        }

        private byte[] DeserializeSignature(string signature)
        {
            // Thanks to: https://crypto.stackexchange.com/questions/1795/how-can-i-convert-a-der-ecdsa-signature-to-asn-1
            var s = Base64Url.Decode(signature);

            using (var ms = new MemoryStream(s))
            {
                var header = ms.ReadByte(); // marker
                var b1 = ms.ReadByte(); // length of remaining bytes

                var markerR = ms.ReadByte(); // marker
                var b2 = ms.ReadByte(); // length of vr
                var vr = new byte[b2]; // signed big-endian encoding of r
                ms.Read(vr, 0, vr.Length);
                vr = RemoveAnyNegativeFlag(vr); // r

                var markerS = ms.ReadByte(); // marker 
                var b3 = ms.ReadByte(); // length of vs
                var vs = new byte[b3]; // signed big-endian encoding of s
                ms.Read(vs, 0, vs.Length);
                vs = RemoveAnyNegativeFlag(vs); // s

                var parsedSignature = new byte[vr.Length + vs.Length];
                vr.CopyTo(parsedSignature, 0);
                vs.CopyTo(parsedSignature, vr.Length);

                return parsedSignature;
            }
        }

        private byte[] RemoveAnyNegativeFlag(byte[] input)
        {
            if (input[0] != 0) return input;

            var output = new byte[input.Length - 1];
            Array.Copy(input, 1, output, 0, output.Length);
            return output;
        }
    }
}
