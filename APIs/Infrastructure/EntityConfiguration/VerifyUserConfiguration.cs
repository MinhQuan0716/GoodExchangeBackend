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
    internal class VerifyUserConfiguration : IEntityTypeConfiguration<VerifyUser>
    {
        public void Configure(EntityTypeBuilder<VerifyUser> builder)
        {
            builder.HasOne(x=>x.User).WithOne(x=>x.VerifyUser).HasForeignKey<VerifyUser>(x=>x.UserId);
            builder.HasOne(x => x.VerificationStatus).WithMany(x => x.VerifyUsers).HasForeignKey(x => x.VerifyStatusId);
        }
    }
}
