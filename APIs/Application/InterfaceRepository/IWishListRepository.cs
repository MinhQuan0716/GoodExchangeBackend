using Application.ViewModel.WishListModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IWishListRepository:IGenericRepository<WishList>
    {
        Task<List<WishList>> FindWishListByPostId(Guid postId);
        Task<List<WishListViewModel>> FindWishListByUserId(Guid userId);
    }
}
