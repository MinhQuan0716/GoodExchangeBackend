using Application.ViewModel.RatingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IRatingService
    {
        Task<bool> RateUserAsync(CreateRatingModel createRatingModel);
    }
}
