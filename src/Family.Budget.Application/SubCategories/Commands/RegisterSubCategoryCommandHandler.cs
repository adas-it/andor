namespace Family.Budget.Application.SubCategories.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.SubCategories.Errors;
using Family.Budget.Application.Dto.SubCategories.Requests;
using Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.SubCategories.Services;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Mapster;
using MediatR;

public record RegisterSubCategoryCommand : BaseSubCategory, IRequest<SubCategoryOutput>
{
    public RegisterSubCategoryCommand() { }
    public RegisterSubCategoryCommand(RegisterSubCategoryInput dto) : base(dto.Name,
        dto.Description,
        dto.StartDate,
        dto.DeactivationDate,
        dto.CategoryId)
    {
        DefaultPaymentMethodId = dto.DefaultPaymentMethodId;
    }

    public Guid DefaultPaymentMethodId { get; set; }
}

public class RegisterSubCategoryCommandHandler : BaseCommands, IRequestHandler<RegisterSubCategoryCommand, SubCategoryOutput>
{
    private readonly ISubCategoryRepository repository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ISubCategoryServices SubCategoryServices;
    private readonly IPaymentMethodRepository paymentMethodRepository;

    public RegisterSubCategoryCommandHandler(ISubCategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        ISubCategoryServices SubCategoryServices,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.SubCategoryServices = SubCategoryServices;
        this.categoryRepository = categoryRepository;
    }

    public async Task<SubCategoryOutput> Handle(RegisterSubCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetById(request.CategoryId, cancellationToken);

        if (category is null)
        {
            _notifier.Erros.Add(Errors.CategoryNotFound());
            return null!;
        }

        var paymentMethod = await paymentMethodRepository.GetById(request.DefaultPaymentMethodId, cancellationToken);

        var item = SubCategory.New(request.Name,
            request.Description,
            request.StartDate,
            request.DeactivationDate,
            category!,
            paymentMethod);

        await SubCategoryServices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await repository.Insert(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<SubCategoryOutput>();
    }
}