using Application.ViewModel.ProductModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class CreatePostModel
    {
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public CreateProductModel productModel { get; set; }
        public string PaymentType { get; set; }
    }
}
