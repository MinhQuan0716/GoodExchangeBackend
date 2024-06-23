using Application.ViewModel.RatingModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IRatingRepository:IGenericRepository<Rating>
    {
        Task<List<Rating>> GetAllRatingByRaterId(Guid raterId);
        Task<List<RatingViewModel>> GetAllRatingByRatedUserId (Guid ratedUserId);    
    }
}
