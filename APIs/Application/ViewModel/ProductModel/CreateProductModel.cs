using Application.ModelValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ProductModel
{
    public class CreateProductModel
    {
        [CheckFileExtension(new string[] {".jpg",".png",".jpeg"})]
        public IFormFile ProductImage { get; set; }
        public string ProductStatus { get; set; }
        public long ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public int CategoryId { get; set; }
        public int ConditionId { get; set; }
        public string? RequestedProduct { get; set; }
    }
}
