namespace Family.Budget.TestsUtil;

using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.Entities.Users.ValueObject;
using System;
using System.Collections.Generic;

public class AccountsBaseFixture : BaseFixture
{
    public Account GetValidAccount(Guid userId)
    {
        var subCategoryMoneyDeposit = GetValidSubCategory(MovementType.MoneyDeposit);
        var subCategoryMoneySpending = GetValidSubCategory(MovementType.MoneySpending);

        return Account.New(Faker.Person.FullName,
            Faker.Person.FullName,
            new List<Category>()
            {
                subCategoryMoneyDeposit.Category,
                subCategoryMoneySpending.Category
            },
            new List<SubCategory>()
            {
                subCategoryMoneyDeposit,
                subCategoryMoneySpending
            },
            new List<PaymentMethod>()
            {
                subCategoryMoneyDeposit.DefaultPaymentMethod!,
                subCategoryMoneySpending.DefaultPaymentMethod!
            },
            new List<Guid>
            {
                userId
            },
            GetCurrency());
    }

    public PaymentMethod GetValidPaymentMethod(MovementType movementType)
    {
        return PaymentMethod.New(
            GetStringRigthSize(5, 100),
            GetStringRigthSize(5, 100),
            movementType,
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(10));
    }

    public SubCategory GetValidSubCategory(MovementType movementType)
    {
        var category = GetValidCategory(movementType);
        var paymentMethod = GetValidPaymentMethod(movementType);

        return SubCategory.New(
            GetStringRigthSize(5, 100),
            GetStringRigthSize(5, 100),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(10),
            category,
            paymentMethod);
    }

    public Category GetValidCategory(MovementType movementType)
    {
        return Category.New(
            GetStringRigthSize(5, 100),
            GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(10),
            movementType);
    }

    public Category GetValidCategory()
    {
        return Category.New(
            GetStringRigthSize(5, 100),
            GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(10),
            MovementType.MoneySpending);
    }

    public FinancialMovement GetValidFinancialMovement(Guid? accountId,
        MovementType movementType,
        PaymentMethod paymentMethod,
        MovementStatus movementStatus,
        decimal value)
    {
        var subCategory = GetValidSubCategory(movementType);

        return FinancialMovement.New(
            DateTime.UtcNow,
            GetStringRigthSize(5, 1000),
            value,
            subCategory,
            movementType,
            movementStatus,
            paymentMethod,
            accountId ?? Guid.NewGuid());
    }

    public Currency GetCurrency()
        => Currency.New("Dolar", "DLR", "$");

    public User GetUser()
        => User.New(Guid.NewGuid(), Faker.Person.UserName, true, true, Faker.Person.FirstName,
        Faker.Person.LastName, Faker.Person.Email, Faker.Person.Avatar, DateTime.Now.AddDays(-10),
        true, DateTime.Now.AddDays(-10), true, DateTime.Now.AddDays(-10), new LocationInfos("pt-BR", GetCurrency()));
}
