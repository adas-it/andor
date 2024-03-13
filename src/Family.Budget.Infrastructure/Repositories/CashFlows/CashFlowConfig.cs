namespace Family.Budget.Infrastructure.Repositories.CashFlows;

using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CashFlowConfig : IEntityTypeConfiguration<CashFlow>
{
    public void Configure(EntityTypeBuilder<CashFlow> entity)
    {
        entity.ToTable(nameof(CashFlow), SchemasNames.FamilyBudget);
        entity.HasKey(k => k.Id);
        entity.Ignore(k => k.Events);
        entity.Ignore(k => k.RevenuesBalance);
        entity.Ignore(k => k.BalanceForecast);
        entity.Ignore(k => k.MonthlyDeficitSurplus);
    }
}
