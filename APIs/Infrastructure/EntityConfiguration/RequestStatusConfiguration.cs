using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityConfiguration
{
    internal class RequestStatusConfiguration : IEntityTypeConfiguration<RequestStatus>
    {
        public void Configure(EntityTypeBuilder<RequestStatus> builder)
        {
            builder.HasKey(reqstatus => reqstatus.StatusId);
            builder.HasData(new RequestStatus
            {
                StatusId = 1,
                StatusName=nameof(StatusName.Pending),
            },
            new RequestStatus
            {
                StatusId= 2,
                StatusName= nameof(StatusName.Accept),
            },
            new RequestStatus
            {
                StatusId=3,
                StatusName=nameof(StatusName.Reject),
            }
            );
        }
    }
}
