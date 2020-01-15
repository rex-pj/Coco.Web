﻿using Coco.Entities.Constant;
using Coco.Entities.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coco.UserDAL.MappingConfigs.AuthMappings
{
    public class RoleMappingConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role), TableSchemaConst.AUTH);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasMany(c => c.UserRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(c => c.RoleId);
        }
    }
}