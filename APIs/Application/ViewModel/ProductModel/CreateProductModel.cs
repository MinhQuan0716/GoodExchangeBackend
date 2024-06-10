﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ProductModel
{
    public class CreateProductModel
    {
        public IFormFile ProductImage { get; set; }
        public string ProductName { get; set; }
        public long ProductPrice { get; set; }
        public int CategoryId { get; set; }
        public int ProductTypeId { get; set; }
       
    }
    public class CreatePostViewModel
    {
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public CreateProductModel productModel { get; set; }
    }
}
