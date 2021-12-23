using AuthServer.Core.Configurations;
using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> clients;
        private readonly ITokenServices tokenServices;
        private readonly UserManager<UserApp> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> clients, ITokenServices tokenServices, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            this.clients = clients.Value;
            this.tokenServices = tokenServices;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentException(nameof(loginDto));
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("Email or password is wrong", 400,true);
            if (!await userManager.CheckPasswordAsync(user, loginDto.Password)) return Response<TokenDto>.Fail("Email or password is wrong", 400, true);

            var token = tokenServices.CreateToken(user);

            var userRefreshToken = await userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if(userRefreshToken == null)
            {
                await userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id,Code = token.RefreshToken,ExpirationTime = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.ExpirationTime = token.RefreshTokenExpiration;
            }
            await unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found", 404, true);
            }
            var user = await userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user == null)
            {
                return Response<TokenDto>.Fail("User not found", 404, true);
            }
            var token = tokenServices.CreateToken(user);
            existRefreshToken.Code = token.RefreshToken;
            existRefreshToken.ExpirationTime = token.RefreshTokenExpiration;

            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClientDto(ClientLoginDto clientLoginDto)
        {
            var client = clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("Client or secret Not found", 404, true);
            }
            var token = tokenServices.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token,200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existrefreshToken = await userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existrefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);
            }
            userRefreshTokenService.Remove(existrefreshToken);
            await unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(200);
        }
    }
}
