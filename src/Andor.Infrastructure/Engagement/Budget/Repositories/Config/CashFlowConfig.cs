using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Config;

public record CashFlowConfig : IEntityTypeConfiguration<CashFlow>
{
    public void Configure(EntityTypeBuilder<CashFlow> entity)
    {
        entity.ToTable(nameof(CashFlow), SchemasNames.Engagement);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Id).HasConversion(GetCashFlowIdConverter());
        entity.Property(k => k.AccountId).HasConversion(AccountConfig.GetAccountIdConverter());
        entity.Property(k => k.Year).HasConversion(GetYearConverter());
        entity.Property(k => k.Month).HasConversion(GetMonthConverter());

        entity.Property(k => k.AccountBalance).HasColumnType("decimal(18,2)");
        entity.Property(k => k.MonthRevenues).HasColumnType("decimal(18,2)");
        entity.Property(k => k.ForecastUpcomingRevenues).HasColumnType("decimal(18,2)");
        entity.Property(k => k.BalanceForecast).HasColumnType("decimal(18,2)");
        entity.Property(k => k.Expenses).HasColumnType("decimal(18,2)");
        entity.Property(k => k.ForecastExpenses).HasColumnType("decimal(18,2)");
        entity.Property(k => k.FinalBalancePreviousMonth).HasColumnType("decimal(18,2)");

        entity.Ignore(x => x.RevenuesBalance);
        entity.Ignore(x => x.BalanceForecast);
        entity.Ignore(x => x.MonthlyDeficitSurplus);

        entity.HasIndex(k => new { k.AccountId, k.Year, k.Month }).IsUnique();

        entity.HasOne(k => k.Account)
            .WithMany()
            .HasForeignKey(k => k.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

    }

    public static ValueConverter<CashFlowId, Guid> GetCashFlowIdConverter()
        => new(id => id!.Value, value => CashFlowId.Load(value));

    public static ValueConverter<Year, int> GetYearConverter()
        => new(id => id!.Value, value => Year.Load(value));

    public static ValueConverter<Month, int> GetMonthConverter()
        => new(id => id!.Value, value => Month.Load(value));
}
