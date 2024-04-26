using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public class SubCategoryConfig : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> entity)
    {
        entity.ToTable(nameof(SubCategory), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetSubCategoryIdConverter());
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(255);
        entity.Ignore(k => k.Events);

        entity.HasOne(k => k.Category).WithMany(x => x.SubCategories).HasForeignKey(x => x.CategoryId);
        entity.HasOne(k => k.DefaultPaymentMethod).WithMany(x => x.SubCategories).HasForeignKey(x => x.DefaultPaymentMethodId);

        entity.Navigation(x => x.Category).AutoInclude();
    }

    public static ValueConverter<SubCategoryId, Guid> GetSubCategoryIdConverter()
        => new(id => id!.Value, value => SubCategoryId.Load(value));
}
