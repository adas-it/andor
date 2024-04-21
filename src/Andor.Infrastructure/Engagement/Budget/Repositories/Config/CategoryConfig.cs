using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.ToTable(nameof(Category), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetCategoryIdConverter());
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Property(k => k.Type).HasConversion(GetMovementTypeConverter());
        entity.Ignore(k => k.Events);

        entity.HasMany(x => x.SubCategories).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId);
    }

    public static ValueConverter<CategoryId, Guid> GetCategoryIdConverter()
        => new(id => id!.Value, value => CategoryId.Load(value));

    public static ValueConverter<MovementType, int> GetMovementTypeConverter()
        => new(v => v.Key, v => MovementType.GetByKey<MovementType>(v));
}
