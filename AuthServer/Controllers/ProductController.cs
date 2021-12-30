using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : CustomController
    {
        private readonly ServiceGeneric<ProductController, ProductDto> serviceGeneric;

        public ProductController(ServiceGeneric<ProductController, ProductDto> serviceGeneric)
        {
            this.serviceGeneric = serviceGeneric;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return ActionResultInstance(await serviceGeneric.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto productDto)
        {
            return ActionResultInstance(await serviceGeneric.AddAsync(productDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            return ActionResultInstance(await serviceGeneric.Update(productDto, productDto.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await serviceGeneric.Remove(id));
        }
    }
}