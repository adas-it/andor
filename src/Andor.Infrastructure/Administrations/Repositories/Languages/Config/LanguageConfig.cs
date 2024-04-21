using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
<<<<<<< HEAD
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
=======
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
>>>>>>> main
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Infrastructure.Administrations.Repositories.Languages.Config;

public class LanguageConfig : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> entity)
    {
        entity.ToTable(nameof(Language), SchemasNames.Administration);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(70);
        entity.Property(k => k.Symbol).HasMaxLength(100);
        entity.Property(k => k.Iso).HasMaxLength(Iso.MaxLength);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => LanguageId.Load(value)
        );

        entity.Property(k => k.Iso)
        .HasConversion(
            id => id!.Value,
            value => Iso.Load(value)
        );
    }
}
