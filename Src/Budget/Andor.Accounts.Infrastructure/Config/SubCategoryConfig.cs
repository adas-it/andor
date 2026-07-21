using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class SubCategoryConfig : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> entity)
    {
        _ = entity.ToTable(nameof(SubCategory), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => SubCategoryId.Load(value));

        _ = entity.Property(k => k.Owner)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value.Value,
                value => value == null ? (AccountId?)null : AccountId.Load(value.Value));

        _ = entity.Property(k => k.Name)
            .HasConversion(Converters.GetNameConverter())
            .HasMaxLength(Name.MaxLength);

        _ = entity.Property(k => k.Description)
            .HasConversion(Converters.GetDescriptionConverter())
            .HasMaxLength(Description.MaxLength);

        _ = entity.Property(k => k.CategoryId)
            .HasConversion(id => id.Value, value => CategoryId.Load(value));

        _ = entity.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        _ = entity.Property(k => k.DefaultPaymentMethodId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value.Value,
                value => value == null ? (PaymentMethodId?)null : PaymentMethodId.Load(value.Value));

        _ = entity.HasOne(x => x.DefaultPaymentMethod)
            .WithMany()
            .HasForeignKey(x => x.DefaultPaymentMethodId);

        _ = entity.Property(k => k.IsDeleted);
    }
}
