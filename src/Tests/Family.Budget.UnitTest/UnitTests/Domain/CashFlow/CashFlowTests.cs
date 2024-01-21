namespace Family.Budget.UnitTest.UnitTests.Domain.CashFlow;

using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using FluentAssertions;
using System;
using Xunit;

[Collection(nameof(CashFlowTestFixture))]
public class CashFlowTests
{
    private readonly CashFlowTestFixture fixture;

    public CashFlowTests(CashFlowTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate()
    {
        //Act
        var accountId = Guid.NewGuid();
        var validData = fixture.GetValidCashFlow(accountId);

        //Assert
        validData.Should().NotBeNull();
        validData.Id.Should().NotBeEmpty();
        validData.Events.Should().NotBeEmpty();
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(0);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneySpending))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneySpending()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneySpending;
        var status = MovementStatus.Accomplished;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 2000;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);
        //Act
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(0);
        validData.RevenuesBalance.Should().Be(0);
        validData.Expenses.Should().Be(validValue);
        validData.AccountBalance.Should().Be(validValue * -1);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(validValue * -1);
        validData.MonthlyDeficitSurplus.Should().Be(validValue * -1);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneyDeposit))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneyDeposit()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneyDeposit;
        var status = MovementStatus.Accomplished;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);
        //Act
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(validValue);
        validData.FinalBalancePreviousMonth.Should().Be(0);
        validData.RevenuesBalance.Should().Be(validValue);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(validValue);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(validValue);
        validData.MonthlyDeficitSurplus.Should().Be(validValue);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneySpending_Forecast))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneySpending_Forecast()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneySpending;
        var status = MovementStatus.Expected;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);
        //Act
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(0);
        validData.RevenuesBalance.Should().Be(0);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(0);
        validData.ForecastExpenses.Should().Be(validValue);
        validData.BalanceForecast.Should().Be(validValue * -1);
        validData.MonthlyDeficitSurplus.Should().Be(validValue * -1);
    }



    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneyDeposit_Forecast))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneyDeposit_Forecast()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneyDeposit;
        var status = MovementStatus.Expected;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);
        //Act
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(0);
        validData.RevenuesBalance.Should().Be(0);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(0);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(validValue);
        validData.MonthlyDeficitSurplus.Should().Be(validValue);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneySpending_PreviousMonth_Value))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneySpending_PreviousMonth_Value()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneySpending;
        var status = MovementStatus.Accomplished;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 2000;
        var previousMonthValue = 1000;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);
        //Act
        validData.SetFinalBalancePreviousMonth(1000);
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonthValue);
        validData.RevenuesBalance.Should().Be(previousMonthValue);
        validData.Expenses.Should().Be(validValue);
        validData.AccountBalance.Should().Be(previousMonthValue + (validValue * -1));
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(previousMonthValue + (validValue * -1));
        validData.MonthlyDeficitSurplus.Should().Be(validValue * -1);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneyDeposit_PreviousMonth_Value))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneyDeposit_PreviousMonth_Value()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneyDeposit;
        var status = MovementStatus.Accomplished;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;
        var previousMonthValue = 1000;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonthValue);
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(validValue);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonthValue);
        validData.RevenuesBalance.Should().Be(validValue + previousMonthValue);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(validValue + previousMonthValue);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(validValue + previousMonthValue);
        validData.MonthlyDeficitSurplus.Should().Be(validValue);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneySpending_Forecast_PreviousMonth_Value))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneySpending_Forecast_PreviousMonth_Value()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneySpending;
        var status = MovementStatus.Expected;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;
        var previousMonthValue = 1000;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonthValue);
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonthValue);
        validData.RevenuesBalance.Should().Be(previousMonthValue);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(previousMonthValue);
        validData.ForecastExpenses.Should().Be(validValue);
        validData.BalanceForecast.Should().Be(previousMonthValue + (validValue * -1));
        validData.MonthlyDeficitSurplus.Should().Be(validValue * -1);
    }

    [Fact(DisplayName = nameof(Instantiate_and_AddNewFinancialMovement_MoneyDeposit_Forecast_PreviousMonth_Value))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Instantiate_and_AddNewFinancialMovement_MoneyDeposit_Forecast_PreviousMonth_Value()
    {
        //Act
        var accountId = Guid.NewGuid();
        var type = MovementType.MoneyDeposit;
        var status = MovementStatus.Expected;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPayment = fixture.GetValidPaymentMethod(type);
        var validValue = 1500;
        var previousMonthValue = 1000;

        var financialMovement = fixture.GetValidFinancialMovement(accountId, type,
            validPayment, status, validValue);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonthValue);
        validData.AddFinancialMovement(financialMovement);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonthValue);
        validData.RevenuesBalance.Should().Be(previousMonthValue);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(previousMonthValue);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be(previousMonthValue + validValue);
        validData.MonthlyDeficitSurplus.Should().Be(validValue);
    }

    [Fact(DisplayName = nameof(Mike_baguncinha))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void Mike_baguncinha()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var financialMovementDepositAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Accomplished, 885.44m);

        var financialMoneySpendingtAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, 7451.44m);

        var financialMoneySpendingtAccomplished2 = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, 205.36m);

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, 185.21m);

        var financialMoneySpendingtExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, 23.88m);

        var financialMoneySpendingtExpected2 = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, 1223.88m);

        //Act
        validData.SetFinalBalancePreviousMonth(5200);
        validData.AddFinancialMovement(financialMovementDepositAccomplished);
        validData.AddFinancialMovement(financialMoneySpendingtAccomplished);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMoneySpendingtExpected);
        validData.AddFinancialMovement(financialMoneySpendingtAccomplished2);

        //Assert
        validData.MonthRevenues.Should().Be(885.44m);
        validData.FinalBalancePreviousMonth.Should().Be(5200);
        validData.RevenuesBalance.Should().Be(6085.44m);
        validData.Expenses.Should().Be(7656.80m);
        validData.AccountBalance.Should().Be(-1571.36m);
        validData.ForecastExpenses.Should().Be(23.88m);
        validData.BalanceForecast.Should().Be(-1410.03m);
        validData.MonthlyDeficitSurplus.Should().Be(-6610.03m);

        validData.SetFinalBalancePreviousMonth(236);

        validData.MonthRevenues.Should().Be(885.44m);
        validData.FinalBalancePreviousMonth.Should().Be(236);
        validData.RevenuesBalance.Should().Be(1121.44m);
        validData.Expenses.Should().Be(7656.80m);
        validData.AccountBalance.Should().Be(-6535.36m);
        validData.ForecastExpenses.Should().Be(23.88m);
        validData.BalanceForecast.Should().Be(-6374.03m);
        validData.MonthlyDeficitSurplus.Should().Be(-6610.03m);


        validData.AddFinancialMovement(financialMoneySpendingtExpected2);
        validData.SetFinalBalancePreviousMonth(536.88m);

        validData.MonthRevenues.Should().Be(885.44m);
        validData.FinalBalancePreviousMonth.Should().Be(536.88m);
        validData.RevenuesBalance.Should().Be(1422.32m);
        validData.Expenses.Should().Be(7656.80m);
        validData.AccountBalance.Should().Be(-6234.48m);
        validData.ForecastExpenses.Should().Be(1247.76m);
        validData.BalanceForecast.Should().Be(-7297.03m);
        validData.MonthlyDeficitSurplus.Should().Be(-7833.91m);
    }

    [Fact(DisplayName = nameof(UpdateFinancialMovementExpense_Expect_to_Accomplished_With_NewValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementExpense_Expect_to_Accomplished_With_NewValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);

        var financialMoneySpendingtExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, expense);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMoneySpendingtExpected);

        validData.ForecastExpenses.Should().Be(expense);

        expense += 20;

        var financialMoneySpendingtAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, expense);

        validData.UpdateFinancialMovement(financialMoneySpendingtExpected, financialMoneySpendingtAccomplished);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.Expenses.Should().Be(expense);
        validData.AccountBalance.Should().Be(previousMonth - expense);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be((previousMonth + deposit) - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }

    [Fact(DisplayName = nameof(UpdateFinancialMovementExpense_Accomplished_to_Expect_With_NewValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementExpense_Accomplished_to_Expect_With_NewValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementSpendingAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, expense);

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);


        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMovementSpendingAccomplished);

        validData.Expenses.Should().Be(expense);

        expense += 20;

        var financialMovementSpendingtExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, expense);

        validData.UpdateFinancialMovement(financialMovementSpendingAccomplished, financialMovementSpendingtExpected);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(previousMonth);
        validData.ForecastExpenses.Should().Be(expense);
        validData.BalanceForecast.Should().Be((previousMonth + deposit) - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }



    [Fact(DisplayName = nameof(UpdateFinancialMovementExpense_Accomplished_to_Expect_With_DifferentValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementExpense_Accomplished_to_Expect_With_DifferentValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementSpendingAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, expense);

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);


        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMovementSpendingAccomplished);

        validData.Expenses.Should().Be(expense);

        expense += 20;

        var financialMovementSpendingAccomplished2 = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Accomplished, expense);

        validData.UpdateFinancialMovement(financialMovementSpendingAccomplished, financialMovementSpendingAccomplished2);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.Expenses.Should().Be(expense);
        validData.AccountBalance.Should().Be(previousMonth - expense);
        validData.ForecastExpenses.Should().Be(0);
        validData.BalanceForecast.Should().Be((previousMonth + deposit) - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }

    [Fact(DisplayName = nameof(UpdateFinancialMovementDeposit_Expect_to_Accomplished_With_NewValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementDeposit_Expect_to_Accomplished_With_NewValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);

        var financialMovementSpendingExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, expense);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMovementSpendingExpected);

        validData.ForecastUpcomingRevenues.Should().Be(deposit);

        deposit += 20;

        var financialMovementDepositAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneySpending, MovementStatus.Accomplished, deposit);

        validData.UpdateFinancialMovement(financialMovementDepositExpected, financialMovementDepositAccomplished);

        //Assert
        validData.MonthRevenues.Should().Be(deposit);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(deposit + previousMonth);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(deposit + previousMonth);
        validData.ForecastExpenses.Should().Be(expense);
        validData.BalanceForecast.Should().Be(deposit + previousMonth - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }

    [Fact(DisplayName = nameof(UpdateFinancialMovementDeposit_Accomplished_to_Expect_With_NewValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementDeposit_Accomplished_to_Expect_With_NewValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementDepositAccomplished = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneySpending, MovementStatus.Accomplished, deposit);

        var financialMovementSpendingExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, expense);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositAccomplished);
        validData.AddFinancialMovement(financialMovementSpendingExpected);

        validData.RevenuesBalance.Should().Be(previousMonth + deposit);
        validData.ForecastUpcomingRevenues.Should().Be(0);

        deposit += 20;

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);

        validData.UpdateFinancialMovement(financialMovementDepositAccomplished, financialMovementDepositExpected);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.ForecastUpcomingRevenues.Should().Be(deposit);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(previousMonth);
        validData.ForecastExpenses.Should().Be(expense);
        validData.BalanceForecast.Should().Be(deposit + previousMonth - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }



    [Fact(DisplayName = nameof(UpdateFinancialMovementDeposit_Accomplished_to_Expect_WithDifferentValue))]
    [Trait("Domain", "CashFlow - Aggregates")]
    public void UpdateFinancialMovementDeposit_Accomplished_to_Expect_WithDifferentValue()
    {
        //Act
        var accountId = Guid.NewGuid();
        var typeMoneyDeposit = MovementType.MoneyDeposit;
        var typeMoneySpending = MovementType.MoneySpending;

        var validData = fixture.GetValidCashFlow(accountId);
        var validPaymentMoneyDeposit = fixture.GetValidPaymentMethod(typeMoneyDeposit);
        var validPaymentMoneySpending = fixture.GetValidPaymentMethod(typeMoneySpending);

        var deposit = 885;
        var expense = 1200;
        var previousMonth = 1000;

        var financialMovementDepositExpected = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);

        var financialMovementSpendingExpected = fixture.GetValidFinancialMovement(accountId, typeMoneySpending,
            validPaymentMoneySpending, MovementStatus.Expected, expense);

        //Act
        validData.SetFinalBalancePreviousMonth(previousMonth);
        validData.AddFinancialMovement(financialMovementDepositExpected);
        validData.AddFinancialMovement(financialMovementSpendingExpected);

        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.ForecastUpcomingRevenues.Should().Be(deposit);

        deposit += 20;

        var financialMovementDepositExpected2 = fixture.GetValidFinancialMovement(accountId, typeMoneyDeposit,
            validPaymentMoneyDeposit, MovementStatus.Expected, deposit);

        validData.UpdateFinancialMovement(financialMovementDepositExpected, financialMovementDepositExpected2);

        //Assert
        validData.MonthRevenues.Should().Be(0);
        validData.ForecastUpcomingRevenues.Should().Be(deposit);
        validData.FinalBalancePreviousMonth.Should().Be(previousMonth);
        validData.RevenuesBalance.Should().Be(previousMonth);
        validData.Expenses.Should().Be(0);
        validData.AccountBalance.Should().Be(previousMonth);
        validData.ForecastExpenses.Should().Be(expense);
        validData.BalanceForecast.Should().Be(deposit + previousMonth - expense);
        validData.MonthlyDeficitSurplus.Should().Be(deposit - expense);
    }
}