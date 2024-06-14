using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IPostRepository:IGenericRepository<Post>
    {
        Task<List<Post>> GetAllPostsWithDetailsAsync();
        Task<List<Post>> GetAllPostsWithDetailsSortByCreationDayAsync();
        Task<List<Post>> SortPostByProductCategoryAsync(int categoryId);
    }
}
