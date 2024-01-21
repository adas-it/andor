namespace Family.Budget.Infrastructure.Repositories.Registrations;
using Family.Budget.Domain.Entities.Registrations;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class RegistrationsConfig : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> entity)
    {
        entity.ToTable(nameof(Registration), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.FirstName).HasMaxLength(100);
        entity.Property(k => k.LastName).HasMaxLength(100);
        entity.Ignore(k => k.Events);
    }
}