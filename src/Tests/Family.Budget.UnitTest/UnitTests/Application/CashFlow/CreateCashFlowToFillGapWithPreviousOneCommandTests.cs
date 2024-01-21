using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.MonthlyCashFlow.Commands;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Family.Budget.UnitTest.UnitTests.Domain.CashFlow;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using cash = Family.Budget.Domain.Entities.CashFlow;

namespace Family.Budget.UnitTest.UnitTests.Application.CashFlow;

[Collection(nameof(CashFlowTestFixture))]
public class CreateCashFlowToFillGapWithPreviousOneCommandTests
{
    private readonly CashFlowTestFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
    public CreateCashFlowToFillGapWithPreviousOneCommandTests(CashFlowTestFixture fixture)
    {
        _fixture = fixture;
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
    }

    [Fact]
    public async Task ShouldInsertGap()
    {
        var app = GetApp();
        var accountId = Guid.NewGuid();

        var item = new CreateCashFlowToFillGapWithPreviousOneCommand()
        {
            Entity = GetChashFlow(
                2023,
                3,
                accountId,
                0)
        };

        _cashFlowRepositoryMock.Setup(x => x.GetPreviousCashFlowByAccountIdAsync(accountId, 2023, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetChashFlow(
                2023,
                1,
                accountId,
                0));

        await app.Handle(item, CancellationToken.None);

        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId && c.Year == 2023 && c.Month == 2), 
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);


        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldInsertGapWithYearOfDifference()
    {
        var app = GetApp();
        var accountId = Guid.NewGuid();

        var item = new CreateCashFlowToFillGapWithPreviousOneCommand()
        {
            Entity = GetChashFlow(
                2023,
                2,
                accountId,
                0)
        };

        _cashFlowRepositoryMock.Setup(x => x.GetPreviousCashFlowByAccountIdAsync(accountId, 2023, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetChashFlow(
                2022,
                11,
                accountId,
                0));

        await app.Handle(item, CancellationToken.None);

        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId && c.Year == 2023 && c.Month == 1),
            It.IsAny<CancellationToken>()), Times.Once);
        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId && c.Year == 2022 && c.Month == 12),
            It.IsAny<CancellationToken>()), Times.Once);

        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldInsertGapWithYearsOfDifference()
    {
        var app = GetApp();
        var accountId = Guid.NewGuid();

        var item = new CreateCashFlowToFillGapWithPreviousOneCommand()
        {
            Entity = GetChashFlow(
                2023,
                2,
                accountId,
                0)
        };

        _cashFlowRepositoryMock.Setup(x => x.GetPreviousCashFlowByAccountIdAsync(accountId, 2023, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetChashFlow(
                2021,
                11,
                accountId,
                0));

        await app.Handle(item, CancellationToken.None);

        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId && c.Year == 2023 && c.Month == 1),
            It.IsAny<CancellationToken>()), Times.Once);
        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId && c.Year == 2022 && c.Month == 12),
            It.IsAny<CancellationToken>()), Times.Once);

        _cashFlowRepositoryMock.Verify(x => x.Insert(It.Is<cash.CashFlow>(c => c.AccountId == accountId),
            It.IsAny<CancellationToken>()), Times.Exactly(14));

        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private cash.CashFlow GetChashFlow(int year, int month, Guid accountId, int value)
    {
        return cash.CashFlow.New(year,
                month,
                accountId,
                value);
    }


    private CreateCashFlowToFillGapWithPreviousOneCommandHandler GetApp()
    {
        return new CreateCashFlowToFillGapWithPreviousOneCommandHandler(
            new Budget.Application.Models.Notifier(),
            _cashFlowRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }
}
