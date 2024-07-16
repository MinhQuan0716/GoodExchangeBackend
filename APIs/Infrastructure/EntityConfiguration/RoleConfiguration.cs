using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.EntityConfiguration
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(new Role
            {
                RoleId = 1,
                RoleName=nameof(RoleName.Admin),
            },
            new Role
            {
                RoleId=2,
                RoleName=nameof(RoleName.Moderator),
            },
            new Role
            {
                RoleId=3,
                RoleName=nameof(RoleName.Member),
            }
            );
        }
    }
}
