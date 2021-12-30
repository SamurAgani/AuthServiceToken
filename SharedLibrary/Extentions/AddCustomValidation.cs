﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extentions
{
    public static class AddCustomValidationResponse
    {
        public static void UseAddCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x=>x.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(),true);
                    var responce = Response<NoContentResult>.Fail(errorDto, 400);

                    return new BadRequestObjectResult(responce);
                };
            });
        }
    }
}
