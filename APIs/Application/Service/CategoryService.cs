using Application.InterfaceService;
using Application.ViewModel.CategoryModel;
using AutoMapper;
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
        private IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateCategory(CreateCategoryModel category)
        {
           Category newCategory=_mapper.Map<Category>(category);
            await _unitOfWork.CategoryRepository.CreateCategory(newCategory);
            return await _unitOfWork.SaveChangeAsync()>0;
        }
       
        public async Task<bool> DeleteCategory(int id)
        {
           var foundCategory=await _unitOfWork.CategoryRepository.GetById(id);
            _unitOfWork.CategoryRepository.Remove(foundCategory);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<CategoryViewModel>> GetAllCategory()
        {
            var category = await _unitOfWork.CategoryRepository.GetAllCategoryAsync();
            return category;
        }

        public async Task<CategoryViewModel> GetCategoryDetail(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            return category;
        }

        public async Task<bool> UpdateCategory(UpdateCategoryModel category)
        {
            var foundCategory = await _unitOfWork.CategoryRepository.GetById(category.CategoryId);
            _mapper.Map(category, foundCategory,typeof(UpdateCategoryModel),typeof(Category));
            _unitOfWork.CategoryRepository.UpdateCategory(foundCategory);
            return await _unitOfWork.SaveChangeAsync()>0;
        }
    }
}

