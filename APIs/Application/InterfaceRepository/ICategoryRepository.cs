using Application.ViewModel.CategoryModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryViewModel>> GetAllCategoryAsync();
    }
}