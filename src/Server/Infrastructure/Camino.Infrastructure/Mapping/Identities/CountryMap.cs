﻿using Camino.Infrastructure.Commons.Constants;
using Camino.Infrastructure.MapBuilders;
using Camino.Core.Domain.Identifiers;
using LinqToDB.Mapping;

namespace Camino.Infrastructure.Mapping.Identities
{
    public class CountryMap : EntityMapBuilder<Country>
    {
        public override void Map(FluentMappingBuilder builder)
        {
            builder.Entity<Country>()
                .HasTableName(nameof(Country))
                .HasSchemaName(TableSchemaConst.Auth)
                .HasIdentity(x => x.Id)
                .HasPrimaryKey(x => x.Id)
                .Association(c => c.UserInfos, (country, userInfos) => country.Id == userInfos.CountryId);
        }
    }
}
