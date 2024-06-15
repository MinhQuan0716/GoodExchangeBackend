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
    internal class ExchangeConditionConfiguration : IEntityTypeConfiguration<ExchangeCondition>
    {
        public void Configure(EntityTypeBuilder<ExchangeCondition> builder)
        {
            builder.HasKey(x => x.ConditionId);
            builder.HasData(new ExchangeCondition
            {
                ConditionId=1,
                ConditionType="For selling"
            },
            new ExchangeCondition
            {
                ConditionId = 2,
                ConditionType="For exchanging"
            },
            new ExchangeCondition
            {
                ConditionId=3,
                ConditionType="For donation"
            }
            );
        }
    }
}
