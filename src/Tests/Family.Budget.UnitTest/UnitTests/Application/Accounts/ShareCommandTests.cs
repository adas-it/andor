namespace Family.Budget.UnitTest.UnitTests.Application.Accounts;

using Family.Budget.Application.Accounts.Commands;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.UnitTest.UnitTests.Domain.CashFlow;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(AccountsTestsFixture))]
public class ShareCommandTests
{
    private readonly AccountsTestsFixture _fixture;
    private readonly Mock<IAccountRepository> _dbMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<Notifier> _notifierMock;

    public ShareCommandTests(AccountsTestsFixture fixture)
    {
        _fixture = fixture;
        _dbMock = new Mock<IAccountRepository>();
        _notifierMock = new Mock<Notifier>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact(DisplayName = nameof(ShareAccountHappyPathAsync))]
    [Trait("Domain", "Accounts - ShareAccount")]
    public async Task ShareAccountHappyPathAsync()
    {
        var app = GetApp();
        var id = Guid.NewGuid();
        var email = _fixture.Faker.Person.Email;
        var account = _fixture.GetValidAccount(id);

        var command = new ShareCommand() { AccountId = id, Email = email };

        _dbMock.Setup(x => x.GetById(
            It.Is<Guid>(z => z == id),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        //Act
        await app.Handle(command, CancellationToken.None);

        //Assert
    }

    private ShareCommandHandler GetApp()
        => new (_dbMock.Object,
            _notifierMock.Object,
            _unitOfWorkMock.Object);

}
