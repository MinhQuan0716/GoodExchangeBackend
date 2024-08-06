using Application.InterfaceService;
using Application.ViewModel.CategoryModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
            var isCreated = await _categoryService.CreateCategory(model);
            if (isCreated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel model)
        {
            var isUpdated = await _categoryService.UpdateCategory(model);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted =await _categoryService.DeleteCategory(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]

        public async Task<IActionResult> GetAllCategory()
        {
            var listCategory=await _categoryService.GetAllCategory();
            return Ok(listCategory);
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet]

        public async Task<IActionResult> GetCategoryDetail(int id)
        {
            var category = await _categoryService.GetCategoryDetail(id);
            return Ok(category);
        }
    }
}
