using Application.InterfaceRepository;
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
        public async Task<List<Category>> GetAllCategoryAsync()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories;
        }
    }
}