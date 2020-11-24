using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public abstract class BaseParserAbstract
    {
        /// <summary>
        /// Parse the flags from the given array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        protected (bool IsUserPresent, bool IsUserVerified, bool ExistsAttestedCredentialData, bool ExtensionDataIncluded) ParseFlags(byte[] value)
        {
            (bool IsUserPresent, bool IsUserVerified, bool ExistsAttestedCredentialData, bool ExtensionDataIncluded) result;            
            var flags = new BitArray(value);         
            result.IsUserPresent = flags[0]; // (UP)
            // Bit 1 reserved for future use (RFU1)
            result.IsUserVerified = flags[2]; // (UV)
            // Bits 3-5 reserved for future use (RFU2)
            result.ExistsAttestedCredentialData = flags[6]; // (AT) 
            result.ExtensionDataIncluded = flags[7]; // (ED)

            return result;
        }

        /// <summary>
        /// Parse the Login-Counter from the given array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected uint ParseCounter(byte[] value)
        {
            // Signature counter (4 bytes, big-endian unint32)
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse().ToArray();
            }

            return BitConverter.ToUInt32(value, 0);

        }
    }
}
