using Andor.Communications.Domain;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Type = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Infrastructure.Config;

public class RuleConfig : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> entity)
    {
        _ = entity.ToTable(nameof(Rule), "Communication");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id!.Value, value => value);

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Type)
            .HasConversion(id => id!.Key, value => Type.GetByKey<Type>(value));

        _ = entity.Property(x => x.CreatedAt);

        _ = entity.HasMany(x => x.Templates)
            .WithOne(x => x.Rule)
            .HasForeignKey(x => x.RuleId);

        _ = entity.Ignore(x => x.Events);
    }
}

public class TemplateConfig : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> entity)
    {
        _ = entity.ToTable(nameof(Template), "Communication");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id!.Value, value => value);

        _ = entity.Property(k => k.RuleId)
            .HasConversion(id => id!.Value, value => value);

        _ = entity.Property(x => x.Value);
        _ = entity.Property(x => x.ContentLanguage).HasMaxLength(10);
        _ = entity.Property(x => x.Title).HasMaxLength(50);

        _ = entity.Property(k => k.Partner)
            .HasConversion(id => id!.Key, value => Partner.GetByKey<Partner>(value));

        _ = entity.Property(x => x.CreatedAt);
    }
}
