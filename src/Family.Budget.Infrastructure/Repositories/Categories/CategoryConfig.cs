namespace Family.Budget.Infrastructure.Repositories.Categories;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        var typeConverter = new ValueConverter<MovementType, int>(
        v => v.Key,
        v => MovementType.GetByKey<MovementType>(v));

        entity.ToTable(nameof(Category), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Property(k => k.Type).HasConversion(typeConverter);
        entity.Ignore(k => k.Events);
    }
}