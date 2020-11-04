using IdentityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class AssertionParser
    {
        /// <summary>
        /// Data required for login.
        /// </summary>
        public class AssertionData
        {
            public byte[] RelayingPartyHash { get; set; }

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
            /// Source as Base64-Decoded Bytes.
            /// </summary>
            /// <remarks>
            /// Should be 32 Byte.
            /// </remarks>
            public byte[] SourceAsBase64 { get; set; }
        }

        public AssertionData Parse(string toParse)
        {
            var result = new AssertionData();            
            var span = Base64Url.Decode(toParse).AsSpan();
            // RP ID Hash (32 bytes)
            result.RelayingPartyHash = span.Slice(0, 32).ToArray(); 
            span = span.Slice(32);

            // Flags (1 byte)
            var flagsBuf = span.Slice(0, 1).ToArray();
            var flags = new BitArray(flagsBuf); 
            span = span.Slice(1);
            result.IsUserPresent = flags[0];            
            result.IsUserVerified = flags[2];
            // Bits 3-5 reserved for future use (RFU2)
            result.ExistsAttestedCredentialData = flags[6];
            result.ExtensionDataIncluded = flags[7];
            
            var counterBuf = span.Slice(0, 4);             
            result.Counter = BitConverter.ToUInt32(counterBuf);                     

            // 16. Using the credential public key looked up in step 3, verify that sig is a valid signature over the binary concatenation of aData and hash.
            result.SourceAsBase64 = Base64Url.Decode(toParse);           

            return result;
        }
    }
}
