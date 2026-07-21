using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Andor.Accounts.Infrastructure.Config;

public class CashFlowConfig : IEntityTypeConfiguration<CashFlow>
{
    public void Configure(EntityTypeBuilder<CashFlow> entity)
    {
        _ = entity.ToTable(nameof(CashFlow), "Accounts");
        _ = entity.HasKey(k => k.Id);

        _ = entity.Property(k => k.Id)
            .HasConversion(id => id.Value, value => CashFlowId.Load(value));

        _ = entity.Property(k => k.AccountId)
            .HasConversion(id => id.Value, value => AccountId.Load(value));

        _ = entity.Property(k => k.Year)
            .HasConversion(y => y.Value, value => Year.Load(value));

        _ = entity.Property(k => k.Month)
            .HasConversion(m => m.Value, value => Month.Load(value));

        _ = entity.Property(k => k.PeriodKey);

        _ = entity.HasIndex(k => new { k.AccountId, k.PeriodKey }).IsUnique();

        _ = entity.Property(k => k.FinalBalancePreviousMonth).HasColumnType("decimal(18,2)");
        _ = entity.Property(k => k.MonthRevenues).HasColumnType("decimal(18,2)");
        _ = entity.Property(k => k.ForecastUpcomingRevenues).HasColumnType("decimal(18,2)");
        _ = entity.Property(k => k.Expenses).HasColumnType("decimal(18,2)");
        _ = entity.Property(k => k.ForecastExpenses).HasColumnType("decimal(18,2)");
        _ = entity.Property(k => k.AccountBalance).HasColumnType("decimal(18,2)");

        _ = entity.Ignore(x => x.RevenuesBalance);
        _ = entity.Ignore(x => x.BalanceForecast);
        _ = entity.Ignore(x => x.MonthlyDeficitSurplus);
    }
}
