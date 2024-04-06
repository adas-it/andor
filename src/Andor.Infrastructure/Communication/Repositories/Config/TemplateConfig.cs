using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Infrastructure.Communication.Repositories.Config;

public record TemplateConfig : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> entity)
    {
        entity.ToTable(nameof(Template), SchemasNames.Communication);
        entity.HasKey(k => k.Id);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => TemplateId.Load(value)
        );

        entity.Property(k => k.Partner).HasConversion(
            State => State.Key,
            value => Partner.GetByKey<Partner>(value));

        entity.HasOne(k => k.Rule).WithMany(x => x.Templates).HasForeignKey(x => x.RuleId);

        entity.Navigation(k => k.Rule).AutoInclude();
    }
}
