using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityConfiguration
{
    internal class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasOne(x => x.Author).WithMany(x => x.Posts).HasForeignKey(x => x.UserId).OnDelete(deleteBehavior:DeleteBehavior.NoAction);
            builder.Property(x => x.IsPriority).IsRequired(false);
            builder.Property(x=>x.PaymentType).IsRequired(false);
        }
    }
}
