using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Infrastructure.Communication.Repositories.Config;

public record RuleConfig : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> entity)
    {
        entity.ToTable(nameof(Rule), SchemasNames.Communication);
        entity.HasKey(k => k.Id);

        entity.Property(k => k.Name).HasMaxLength(70);

        entity.Property(k => k.Id).HasConversion(
            id => id!.Value,
            value => RuleId.Load(value));

        entity.Property(k => k.Type).HasConversion(
            State => State.Key,
            value => Domain.Communications.ValueObjects.Type.GetByKey<Domain.Communications.ValueObjects.Type>(value));

        entity.HasMany(k => k.Templates).WithOne(x => x.Rule).HasForeignKey(x => x.RuleId);

        entity.Navigation(k => k.Templates).AutoInclude();

    }
}
