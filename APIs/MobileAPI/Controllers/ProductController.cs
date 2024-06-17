using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    
  /*  public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            List<Product> products=await _productService.GetAllProducts();
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm]CreatePostModel product) 
        {
            bool isCreate=await _productService.CreateProduct(product);
            if (isCreate)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            bool isUpdated=await _productService.UpdateProduct(product);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveProduct(Guid productId)
        {
            bool isRemoved=await _productService.DeleteProduct(productId);
            if (isRemoved)
            {
                return NoContent();
            }
            return BadRequest();
        }
       
    }*/
}
