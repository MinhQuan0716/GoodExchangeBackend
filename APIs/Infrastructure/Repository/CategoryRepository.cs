using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.CategoryModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        public CategoryRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task CreateCategory(Category category)
        {
           await _dbContext.Categories.AddAsync(category);

        }

        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var categories = await _dbContext.Categories.Select(x=>new CategoryViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
            }).ToListAsync();
            return categories;
        }

        public async Task<Category> GetById(int id)
        {
            return await _dbContext.Categories.Where(x => x.CategoryId == id)
                                              .SingleAsync();
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(int id)
        {
            var categories = await _dbContext.Categories.Where(x => x.CategoryId == id).Select(x => new CategoryViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
            }).SingleOrDefaultAsync();
            return categories;
        }

        public void Remove(Category category)
        {
           _dbContext.Categories.Remove(category);
        }

        public void UpdateCategory(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}