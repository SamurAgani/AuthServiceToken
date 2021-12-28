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

namespace AuthServer.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomController : ControllerBase
    { 
        public IActionResult ActionResultInstance<T>(Response<T> response) where T : class
        {
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
    }
}
