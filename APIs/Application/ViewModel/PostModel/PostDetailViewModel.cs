using Application.ViewModel.ProductModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostDetailViewModel
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public string ProductImageUrl { get; set; }
        public long ProductPrice { get; set; }
        public string ProductStatus { get; set; }
        public string RequestedProduct {  get; set; }
        public int? ProductQuantity { get; set; }
        public int ConditionTypeId {  get; set; }
        public string ConditionTypeName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public PostAuthor PostAuthor { get; set; }
    }
}
