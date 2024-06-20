using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.RatingModel
{
    public class CreateRatingModel
    {
        public string RatingTitle { get; set; }
        public string RatingReview {  get; set; }
        public double RatingPoint { get; set; }
        public Guid UserId { get; set; }
    }
}
