using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public abstract class BaseParserAbstract
    {
        protected (bool IsUserPresent, bool IsUserVerified, bool ExistsAttestedCredentialData, bool ExtensionDataIncluded) ParseFlags(byte[] span)
        {
            (bool IsUserPresent, bool IsUserVerified, bool ExistsAttestedCredentialData, bool ExtensionDataIncluded) result;            
            var flags = new BitArray(span);         
            result.IsUserPresent = flags[0]; // (UP)
            // Bit 1 reserved for future use (RFU1)
            result.IsUserVerified = flags[2]; // (UV)
            // Bits 3-5 reserved for future use (RFU2)
            result.ExistsAttestedCredentialData = flags[6]; // (AT) 
            result.ExtensionDataIncluded = flags[7]; // (ED)

            return result;
        }

        protected uint ParseCounter(byte[] span)
        {            
            if (BitConverter.IsLittleEndian)
            {
                span = span.Reverse().ToArray();
            }

            return BitConverter.ToUInt32(span, 0);

        }
    }
}
