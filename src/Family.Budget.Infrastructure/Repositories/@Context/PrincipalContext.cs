namespace Family.Budget.Infrastructure.Repositories.Context;
using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.Registrations;
using Family.Budget.Domain.Entities.SubCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

public class PrincipalContextFactory : IDesignTimeDbContextFactory<PrincipalContext>
{
    public PrincipalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PrincipalContext>();
        optionsBuilder.UseNpgsql("inmemory");

        return new PrincipalContext(optionsBuilder.Options);
    }
}

public partial class PrincipalContext : DbContext
{
    public PrincipalContext(DbContextOptions<PrincipalContext> options) : base(options)
    {
    }

    public DbSet<Configuration> Configuration => Set<Configuration>();
    public DbSet<Category> Category => Set<Category>();
    public DbSet<SubCategory> SubCategory => Set<SubCategory>();
    public DbSet<Currency> Currency => Set<Currency>();
    public DbSet<PaymentMethod> PaymentMethod => Set<PaymentMethod>();
    public DbSet<Registration> Registration => Set<Registration>();
    public DbSet<Account> Account => Set<Account>();
    public DbSet<CashFlow> CashFlow => Set<CashFlow>();
    public DbSet<Invite> Invite => Set<Invite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.GetForeignKeys()
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

            entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string))
                .ToList()
                .ForEach(p => p.SetMaxLength(255));
        }


        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public void Upsert<T>(T entity) where T : class
    {
        if (Entry(entity).State == EntityState.Detached)
            Set<T>().Add(entity);
    }

    public void UpsertRange<T>(IEnumerable<T> entities) where T : class
    {
        foreach (var entity in entities)
        {
            if (Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Add(entity);
            }
        }
    }
}