using Elasticsearch.API.DTOs;
using Elasticsearch.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductCreateDto request)
        {
            return CreatedActionResult(await _productService.SaveAsync(request));
        }



        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto request)
        {
            return CreatedActionResult(await _productService.UpdateAsync(request));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreatedActionResult(await _productService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyId(string id)
        {
            return CreatedActionResult(await _productService.GetByIdAsync(id));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return CreatedActionResult(await _productService.DeleteAsync(id));
        }



    }
}
