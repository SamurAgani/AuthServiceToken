﻿using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Response<UserAppDto>> CreateUSerAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp() {Email = createUserDto.Email, UserName = createUserDto.UserName };
            var result = await userManager.CreateAsync(user,createUserDto.Password);

            if (result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<UserAppDto>.Fail(new ErrorDto(errors,true), 400);

            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<UserAppDto>> GetUSerByNameAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if(user == null)
            {
                return Response<UserAppDto>.Fail("User name not found", 404, true);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}
