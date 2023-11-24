using Family.Budget.Application.Models.Authorization;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Configurations;
using Family.Budget.Infrastructure.Repositories.Context;
using FluentAssertions;
using MediatR;
using Moq;

namespace Family.Budget.IntegrationTest.Configurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationRepositoryTests
{
    private readonly ConfigurationTestFixture fixture;
    private Mock<IMediator> _mediatorMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;

    public ConfigurationRepositoryTests(ConfigurationTestFixture fixture)
    {
        this.fixture = fixture;
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _currentUserServiceMock.Setup(x => x.User).Returns(new ApplicationUser(null!));
    }

    [Fact(DisplayName = nameof(InsertNewConfigurationAsync))]
    [Trait("Integration", "Configuration - Access to a database")]
    public async void InsertNewConfigurationAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        var app = new ConfigurationRepository(context);

        var item = fixture.GetValidConfiguration();

        context.Configuration.RemoveRange(context.Configuration);

        //Act
        await app.Insert(item, CancellationToken.None);

        //Assert
        using var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!);

        var database = context2.Configuration.Find(item.Id);

        database.Should().BeNull();

        var unit = new UnitOfWork(context, _mediatorMock.Object);

        await unit.CommitAsync(CancellationToken.None);

        database = context2.Configuration.Find(item.Id);

        database.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(UpdateConfigurationAsync))]
    [Trait("Integration", "Configuration - Access to a database")]
    public async void UpdateConfigurationAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidConfiguration();

        using (var ctx = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            ctx.Configuration.Add(item);
            ctx.SaveChanges();
        }

        var context = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!);

        var app = new ConfigurationRepository(context);
        var unitWork = new UnitOfWork(context, _mediatorMock.Object);

        var oldDescription = item.Description;
        var newDescription = fixture.GetStringRigthSize(10, 100);

        //Act
        var entity = item;//.Projection();

        entity.ChangeConfiguration(
            item.Name,
            item.Value,
            newDescription,
            item.StartDate,
            item.FinalDate);

        await app.Update(entity, CancellationToken.None);

        //Assert
        using (var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            var database1 = context2.Configuration.Find(item.Id);
            database1?.Description.Should().Be(oldDescription);
        }

        await unitWork.CommitAsync(CancellationToken.None);

        using (var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            var database1 = context2.Configuration.Find(item.Id);
            database1?.Description.Should().Be(newDescription);
        }
    }

    [Fact(DisplayName = nameof(DeleteConfigurationAsync))]
    [Trait("Integration", "Configuration - Access to a database")]
    public async void DeleteConfigurationAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidConfiguration();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        context.Configuration.Add(item);
        context.SaveChanges();

        var unitWork = new UnitOfWork(context, _mediatorMock.Object);

        var app = new ConfigurationRepository(context);

        //Act
        await app.Delete(item.Id, CancellationToken.None);

        //Assert
        var database = context.Configuration.Find(item.Id);

        database.Should().NotBeNull();

        await unitWork.CommitAsync(CancellationToken.None);

        database = context.Configuration.Find(item.Id);

        database.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GetByIdConfigurationAsync))]
    [Trait("Integration", "Configuration - Access to a database")]
    public async void GetByIdConfigurationAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidConfiguration();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        context.Configuration.Add(item);
        context.SaveChanges();

        var app = new ConfigurationRepository(context);

        //Act
        var database = await app.GetById(item.Id, CancellationToken.None);

        //Assert
        database.Should().NotBeNull();
    }
}