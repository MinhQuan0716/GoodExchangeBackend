using Application.InterfaceService;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _CategoryService;
        public CategoryController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var Categorys = await _CategoryService.GetAllCategory();
            return Ok(Categorys);
        }
    }
}

