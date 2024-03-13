namespace Family.Budget.Infrastructure.Repositories.SubCategories;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class SubCategoryConfig : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> entity)
    {
        entity.ToTable(nameof(SubCategory), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Ignore(x => x.Events);

        entity.Navigation(x => x.Category).AutoInclude();
    }
}