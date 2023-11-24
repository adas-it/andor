namespace Family.Budget.Kernel.Extensions;

using Family.Budget.Application._Common.Behaviours;
using Family.Budget.Application.Administrations.Commands;
using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Categories.Services;
using Family.Budget.Application.Currencies.Services;
using Family.Budget.Application.FinancialMovements.Services;
using Family.Budget.Application.Models;
using Family.Budget.Application.PaymentMethod.Services;
using Family.Budget.Application.SubCategories.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class UseCasesExtension
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<Notifier>();

        services.AddValidatorsFromAssembly(
                Assembly.GetAssembly(typeof(RegisterConfigurationCommandValidator)));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionBehavior<,>));

        services.AddTransient<IDateValidationServices, DateValidationServices>();
        services.AddTransient<ICategoryServices, CategoryServices>();
        services.AddTransient<ISubCategoryServices, SubCategoryServices>();
        services.AddTransient<ICurrencieservices, Currencieservices>();
        services.AddTransient<IFinancialMovementservices, FinancialMovementservices>();
        services.AddTransient<IPaymentMethodServices, PaymentMethodServices>();

        return services;
    }
}