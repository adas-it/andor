using Andor.Assets.Domain.Investments.Tickers;
using Andor.Assets.Domain.Investments.Tickers.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Assets.Infrastructure.Repositories.Config;

public class Config
{
    public record TickerConfig : IEntityTypeConfiguration<Ticker>
    {
        public void Configure(EntityTypeBuilder<Ticker> entity)
        {
            entity.ToTable(nameof(Ticker), "investments");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Id).HasConversion(GetInvestmentIdConverter());
            entity.Property(k => k.Code).HasMaxLength(10);
            entity.Property(k => k.Quotas);
            entity.Property(k => k.IsDeleted);

            entity.Ignore(x => x.Events);
        }

        public static ValueConverter<TickerId, Guid> GetInvestmentIdConverter()
            => new(id => id!.Value, value => TickerId.Load(value));
    }
}
