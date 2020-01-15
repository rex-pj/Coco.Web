﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Coco.Entities.Constant;
using Coco.Entities.Domain.Account;

namespace Coco.UserDAL.MappingConfigs.AccountMappings
{
    public class UserMappingConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User), TableSchemaConst.ACCOUNT);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.UserInfo)
                .WithOne(x => x.User)
                .HasForeignKey<UserInfo>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CreatedUserInfos)
                .WithOne(x => x.CreatedBy)
                .HasForeignKey(x => x.CreatedById);

            builder.HasMany(x => x.UpdatedUserInfos)
                .WithOne(x => x.UpdatedBy)
                .HasForeignKey(x => x.UpdatedById);

            builder.HasMany(c => c.UserCareers)
               .WithOne(x => x.User)
               .HasForeignKey(c => c.UserId);

            builder.HasMany(c => c.UserRoles)
               .WithOne(x => x.User)
               .HasForeignKey(c => c.UserId);
        }
    }
}