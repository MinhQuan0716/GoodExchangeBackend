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
    internal class VerficationStatusConfiguration : IEntityTypeConfiguration<VerificationStatus>
    {
        public void Configure(EntityTypeBuilder<VerificationStatus> builder)
        {
            builder.HasKey(x => x.VerificationStatusId);
            builder.HasData(new VerificationStatus
            {
                VerificationStatusId = 1,
                VerificationStatusName = "Pending",
            },
            new VerificationStatus
            {
                VerificationStatusId=2,
                VerificationStatusName="Approved"
            },
            new VerificationStatus
            {
                VerificationStatusId=3,
                VerificationStatusName="Denied"
            }
            );
        }
    }
}
