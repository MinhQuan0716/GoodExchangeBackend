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
    internal class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasKey(reqstatus => reqstatus.StatusId);
            builder.HasData(new OrderStatus
            {
                StatusId = 1,
                StatusName=nameof(StatusName.Pending),
            },
            new OrderStatus
            {
                StatusId= 2,
                StatusName= nameof(StatusName.Accept),
            },
            new OrderStatus
            {
                StatusId=3,
                StatusName=nameof(StatusName.Reject),
            }
            );
        }
    }
}
