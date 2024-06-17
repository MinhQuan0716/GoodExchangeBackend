using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityConfiguration
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Apparel and Accessories",
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Electronics"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Office Supplies"
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Sporting Goods"
                },
                 new Category
                 {
                     CategoryId = 5,
                     CategoryName = "Home and Kitchen"
                 },
                 new Category
                 {
                     CategoryId = 6,
                     CategoryName = "Toys and Games"
                  },
                  new Category
                  {
                      CategoryId = 7,
                      CategoryName = "Books"
                  }
                );
        }
    }
}
