using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Extentions
{
    public static class CustomExceptionHandle
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeatures = context.Features.Get<IExceptionHandlerFeature>();

                    if (errorFeatures != null)
                    {
                        var ex = errorFeatures.Error;
                        ErrorDto errorDto = null;
                        if (ex is CustomException)
                        {
                            errorDto = new ErrorDto(ex.Message, true);
                        }
                        else
                        {
                            errorDto = new ErrorDto(ex.Message, false);
                        }
                        var responce = Response<NoDataDto>.Fail(errorDto, 500);

                        await context.Response.WriteAsync(JsonSerializer.Serialize(responce));

                    }
                });
            });
        }
    }
}
