using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthServer.Core;
using System.Threading.Tasks;
using AuthServer.Core.Services;
using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Configurations;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenServices
    {
        private readonly UserManager<UserApp> userManager;
        private readonly CustomTokenOption customTokenOption;
        public TokenService(UserManager<UserApp> UserManager,IOptions<CustomTokenOption> options)
        {
            userManager = UserManager;
            customTokenOption = options.Value;
        }


        private IEnumerable<Claim> GetClaim(UserApp userApp,List<string> audience)
        {
            var userList = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userList.AddRange(audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());
            return claims;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(customTokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(customTokenOption.RefreshTokenExpiration);
            var securitykey = SignService.GetSymmetricSecurityKey(customTokenOption.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer:customTokenOption.Issuer,
                expires:accessTokenExpiration,
                notBefore:DateTime.Now,
                claims : GetClaim(userApp,customTokenOption.Audience),
                signingCredentials:signingCredentials
            );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;

        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(customTokenOption.AccessTokenExpiration);
            var securitykey = SignService.GetSymmetricSecurityKey(customTokenOption.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: customTokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
            );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };
            return tokenDto;
        }
    }
}
