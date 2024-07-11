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
    internal class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasOne(x => x.Wallet).WithMany(x => x.Transactions).HasForeignKey(x => x.WalletId).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            builder.HasOne(x => x.Subcription).WithMany(x => x.WalletTransactions).HasForeignKey(x => x.SubscriptionId);
        }
    }
}
