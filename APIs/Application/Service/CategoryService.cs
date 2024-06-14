using Application.InterfaceService;
using Application.ViewModel.CategoryModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryViewModel>> GetAllCategory()
        {
            var category = await _unitOfWork.CategoryRepository.GetAllCategoryAsync();
            return category;
        }
    }
}

