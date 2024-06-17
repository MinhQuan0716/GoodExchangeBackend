using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostModel
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public ProductModel Product { get; set; }
        public DateOnly CreationDate { get; set; }
    }

    public class ProductModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public long ProductPrice { get; set; }
        public int? ConditionId { get; set; }
        public string ConditionName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? RequestedProduct { get; set; }
    }
}
