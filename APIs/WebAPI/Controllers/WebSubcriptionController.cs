using Application.InterfaceService;
using Application.ViewModel.SubcriptionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
  
    public class WebSubcriptionController : BaseController
    {
        private readonly ISubcriptionService _subcriptionService;
        public WebSubcriptionController(ISubcriptionService subcriptionService)
        {
            _subcriptionService = subcriptionService;
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult>CreateSubcription(CreateSubcriptionModel createSubcriptionModel)
        {
            bool isCreated = await _subcriptionService.CreateSubcription(createSubcriptionModel);
            if(isCreated) 
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
