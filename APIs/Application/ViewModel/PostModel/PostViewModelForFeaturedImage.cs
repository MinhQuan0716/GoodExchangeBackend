using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostViewModelForFeaturedImage
    {
        public Guid PostId { get; set; }
        public string ImageUrl { get; set; }
        public DateOnly CreationDate { get; set; }
    }
}
