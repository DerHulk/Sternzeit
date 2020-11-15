using IdentityModel;
using PeterO.Cbor;
using Sternzeit.Server.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server
{
    /// <summary>    
    /// </summary>
    public class AttestionParser
    {
        /// <summary>
        /// Data required for Registration.
        /// </summary>
        public class AttestionData
        {
            /// <summary>
            /// Get/Set the attestation statement format identifier.
            /// </summary>
            /// <remarks>
            /// https://www.w3.org/TR/webauthn/#generating-an-attestation-object
            /// </remarks>
            public string Fmt { get; set; }

            /// <summary>
            /// Get/Set a Flag that descripte if the user is presented.
            /// </summary>
            /// <remarks>
            /// https://www.w3.org/TR/webauthn/#concept-user-present
            /// </remarks>
            public bool IsUserPresent { get; set; }

            /// <summary>
            /// Get/Set a Flag that descripte if the user is verified.
            /// </summary>
            /// <remarks>
            /// https://www.w3.org/TR/webauthn/#user-verification
            /// </remarks>
            public bool IsUserVerified { get; set; }

            /// <summary>
            /// Indicates whether the authenticator added attested credential data.
            /// </summary>
            public bool ExistsAttestedCredentialData { get; set; }

            public bool ExtensionDataIncluded { get; set; }

            /// <summary>
            /// https://www.w3.org/TR/webauthn/#signature-counter
            /// </summary>
            public uint Counter { get; set; }

            /// <summary>
            /// Each authenticator has an AAGUID, 
            /// which is a 128-bit identifier indicating the type (e.g. make and model) of the authenticator.
            /// The Relying Party MAY use the AAGUID to infer certain properties of the authenticator, such as certification level and strength of key protection, using information from other sources.
            /// (https://www.w3.org/TR/webauthn)
            /// </summary>
            /// <remarks>
            /// https://www.w3.org/TR/webauthn/#aaguid
            /// </remarks>
            public byte[] AAGuid { get; set; }

            /// <summary>
            /// Byte length of Credential ID.
            /// </summary>
            public int CredentialIdLength { get; set; }

            /// <summary>
            /// A probabilistically-unique byte sequence identifying a public key credential source and its authentication assertions.
            /// </summary>
            /// <remarks>
            /// https://www.w3.org/TR/webauthn/#credential-id
            /// </remarks>
            public byte[] CredentialId { get; set; }

            public PublicKey Key { get; set; }

            public byte[] RelayingPartyHash { get; set; }
        }

       

        public ISerializationService SerializationService { get; }
       
        public AttestionParser(ISerializationService serializationService)
        {
            this.SerializationService = serializationService 
                ?? throw new ArgumentNullException(nameof(serializationService));
        }

        public AttestionData Parse(string toParse)
        {
            var result = new AttestionData();

            // Perform CBOR decoding on the attestationObject field of the AuthenticatorAttestationResponse structure
            // to obtain the attestation statement format fmt, the authenticator data authData, and the attestation statement attStmt.
            CBORObject cbor;
            using (var stream = new MemoryStream(Base64Url.Decode(toParse)))
                cbor = CBORObject.Read(stream);

            var authData = cbor["authData"].GetByteString();
            result.Fmt = cbor["fmt"].AsString();

            var span = authData.AsSpan();

            result.RelayingPartyHash = span.Slice(0, 32).ToArray(); 
            span = span.Slice(32);

            var flags = new BitArray(span.Slice(0, 1).ToArray()); 
            span = span.Slice(1);
            result.IsUserPresent = flags[0]; // (UP)
            // Bit 1 reserved for future use (RFU1)
            result.IsUserVerified = flags[2]; // (UV)
            // Bits 3-5 reserved for future use (RFU2)
            result.ExistsAttestedCredentialData = flags[6]; // (AT) 
            result.ExtensionDataIncluded = flags[7]; // (ED)

            // Signature counter (4 bytes, big-endian unint32)
            var counterBuf = span.Slice(0, 4); span = span.Slice(4);
            result.Counter = BitConverter.ToUInt32(counterBuf); 

            // Attested Credential Data
            // cred data - AAGUID (16 bytes)            
            result.AAGuid = span.Slice(0, 16).ToArray();

            span = span.Slice(16);

            // cred data - L (2 bytes, big-endian uint16)
            var credIdLenBuf = span.Slice(0, 2); span = span.Slice(2);
            credIdLenBuf.Reverse();
            result.CredentialIdLength = BitConverter.ToUInt16(credIdLenBuf);

            // cred data - Credential ID (L bytes)
            result.CredentialId = span.Slice(0, result.CredentialIdLength).ToArray(); 
            span = span.Slice(result.CredentialIdLength);
                                  
            var coseStruct = CBORObject.DecodeFromBytes(span.ToArray());
            result.Key = this.SerializationService.Deserialize<PublicKey>(coseStruct.ToJSONString());

            return result;
        }
    }
}
