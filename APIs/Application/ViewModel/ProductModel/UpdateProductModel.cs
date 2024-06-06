﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ProductModel
{
    public class UpdateProductModel
    {
        public Guid ProductId { get; set; }
        public IFormFile ProductImage { get; set; }
        public string ProductName { get; set; }
        public long ProductPrice { get; set; }
    }
}
