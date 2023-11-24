namespace Family.Budget.IntegrationTest.Category;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Infrastructure.Repositories.Categories;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using FluentAssertions;
using MediatR;
using Moq;

[Collection(nameof(CategoryTestFixture))]
public class CategoryRepositoryTests
{
    private readonly CategoryTestFixture fixture;
    private Mock<IMediator> _mediatorMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;

    public CategoryRepositoryTests(CategoryTestFixture fixture)
    {
        this.fixture = fixture;
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _currentUserServiceMock.Setup(x => x.User).Returns(new ApplicationUser(null!));
    }

    [Fact(DisplayName = nameof(InsertNewCategoryAsync))]
    [Trait("Integration", "Category - Access to a database")]
    public async void InsertNewCategoryAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        var app = new CategoryRepository(context);

        var item = fixture.GetValidCategory();

        context.Category.RemoveRange(context.Category);

        //Act
        await app.Insert(item, CancellationToken.None);

        //Assert
        using var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!);

        var database = context2.Category.Find(item.Id);

        database.Should().BeNull();

        var unit = new UnitOfWork(context, _mediatorMock.Object);

        await unit.CommitAsync(CancellationToken.None);

        database = context2.Category.Find(item.Id);

        database.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(UpdateCategoryAsync))]
    [Trait("Integration", "Category - Access to a database")]
    public async void UpdateCategoryAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidCategory();

        using (var ctx = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            ctx.Category.Add(item);
            ctx.SaveChanges();
        }

        var context = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!);

        var app = new CategoryRepository(context);
        var unitWork = new UnitOfWork(context, _mediatorMock.Object);

        var oldDescription = item.Description;
        var newDescription = fixture.GetStringRigthSize(10, 100);

        //Act
        var entity = item;

        entity!.SetCategory(entity.Name, newDescription, entity.StartDate, entity.DeactivationDate);

        await app.Update(entity, CancellationToken.None);

        //Assert
        using (var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            var database1 = context2.Category.Find(item.Id);
            database1?.Description.Should().Be(oldDescription);
        }

        await unitWork.CommitAsync(CancellationToken.None);

        using (var context2 = new PrincipalContext(TestsUtil.BaseFixture.DbContextOptions!))
        {
            var database1 = context2.Category.Find(item.Id);
            database1?.Description.Should().Be(newDescription);
        }
    }

    [Fact(DisplayName = nameof(DeleteCategoryAsync))]
    [Trait("Integration", "Category - Access to a database")]
    public async void DeleteCategoryAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidCategory();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        context.Category.Add(item);
        context.SaveChanges();

        var unitWork = new UnitOfWork(context, _mediatorMock.Object);

        var app = new CategoryRepository(context);

        //Act
        await app.Delete(item.Id, CancellationToken.None);

        //Assert
        var database = context.Category.Find(item.Id);

        database.Should().NotBeNull();

        await unitWork.CommitAsync(CancellationToken.None);

        database = context.Category.Find(item.Id);

        database.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GetByIdCategoryAsync))]
    [Trait("Integration", "Category - Access to a database")]
    public async void GetByIdCategoryAsync()
    {
        //Arrange
        TestsUtil.BaseFixture.CreateDatabase();

        var item = fixture.GetValidCategory();

        using PrincipalContext context = new(TestsUtil.BaseFixture.DbContextOptions!);

        context.Category.Add(item);
        context.SaveChanges();

        var app = new CategoryRepository(context);

        //Act
        var database = await app.GetById(item.Id, CancellationToken.None);

        //Assert
        database.Should().NotBeNull();
    }
}
