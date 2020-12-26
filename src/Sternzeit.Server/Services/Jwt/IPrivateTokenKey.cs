using Microsoft.IdentityModel.Tokens;

namespace Sternzeit.Server.Services.Jwt
{
    public interface IPrivateTokenKey
    {
        RsaSecurityKey Key { get; }
    }
}