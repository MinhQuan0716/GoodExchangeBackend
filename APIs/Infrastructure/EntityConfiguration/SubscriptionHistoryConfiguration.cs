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
    internal class SubscriptionHistoryConfiguration : IEntityTypeConfiguration<SubscriptionHistory>
    {
        public void Configure(EntityTypeBuilder<SubscriptionHistory> builder)
        {
           builder.HasOne(x=>x.Subcription).WithMany(x=>x.SubcriptionHistories).HasForeignKey(x=>x.SubcriptionId);
            builder.HasOne(x => x.User).WithMany(x => x.SubscriptionHistories).HasForeignKey(x => x.UserId);
        }
    }
}
