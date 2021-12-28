using Microsoft.AspNetCore.Mvc;
using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace AuthServer.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : CustomController
    {
        private readonly IUserService userservice;

        public UserController(IUserService userservice)
        {
            this.userservice = userservice;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUSerDto)
        {
            return ActionResultInstance(await userservice.CreateUSerAsync(createUSerDto));
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return ActionResultInstance(await userservice.GetUSerByNameAsync(HttpContext.User.Identity.Name));
        }
    }
}