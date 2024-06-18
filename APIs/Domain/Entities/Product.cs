using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product:BaseEntity
    {
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public string? ProductDescription { get; set; }
        public long ProductPrice { get; set; }
        public string? RequestedProduct { get; set; }
        public string? ProductStatus { get; set; }
        public int? ProductQuantity { get; set; }
        public int? ConditionId { get; set; }
        public ExchangeCondition ConditionType { get; set; }
        public Post Post { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
