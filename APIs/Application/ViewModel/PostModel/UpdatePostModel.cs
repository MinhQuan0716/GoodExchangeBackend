using Application.ViewModel.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class UpdatePostModel
    {
        public Guid PostId { get; set; }
        public string? PostTitle { get; set; }
        public string? PostContent { get; set; }
        public UpdateProductModel productModel { get; set; }
    }
}
