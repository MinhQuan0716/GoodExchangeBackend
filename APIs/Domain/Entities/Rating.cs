using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Rating:BaseEntity
    {
        public Guid RaterId { get; set; }
        public User Rater { get; set; }
        public float RatingPoint { get; set; }
        public string? RatingTitle { get; set; }
        public string? ReviewContent { get; set; }
        public Guid RatedUserId {  get; set; }
        public User RatedUser { get; set; }
    }
}
