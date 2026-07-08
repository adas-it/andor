using Andor.Assets.Domain.Investments.Areas;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Assets.Infrastructure.Repositories.Config;

internal record AreaConfig : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> entity)
    {
        entity.ToTable(nameof(Area), "investments");

        entity.HasKey(k => k.Id);

        entity.Property(k => k.Id).HasConversion(GetInvestmentIdConverter());

        entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        entity.Property(k => k.IsDeleted);

        entity.Ignore(x => x.Events);
    }

    public static ValueConverter<AreaId, Guid> GetInvestmentIdConverter()
        => new(id => id!.Value, value => AreaId.Load(value));
}
