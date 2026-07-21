using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.Errors;
using Andor.Accounts.Domain.Invites;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.Users;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts;

/// <summary>
/// Represents a financial account aggregate root that manages categories, subcategories, 
/// payment methods, members, invites, and financial movements.
/// </summary>
public class Account : AggregateRoot<AccountId>, ISoftDeletableEntity
{
    /// <summary>
    /// Gets the account name.
    /// </summary>
    public Name Name
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the account description.
    /// </summary>
    public Description? Description
    {
        get; private set;
    }

    /// <summary>
    /// Gets the account currency.
    /// </summary>
    public Currency Currency
    {
        get; private set;
    }

    /// <summary>
    /// Gets a value indicating whether the account is softly deleted.
    /// </summary>
    public bool IsDeleted
    {
        get; private set;
    }

    /// <summary>
    /// Gets the read-only collection of categories associated with this account.
    /// </summary>
    public IReadOnlyCollection<AccountCategory> Categories => [.. _categories];
    private ICollection<AccountCategory> _categories;

    /// <summary>
    /// Gets the read-only collection of subcategories associated with this account.
    /// </summary>
    public IReadOnlyCollection<AccountSubCategory> SubCategories => [.. _subCategories];
    private ICollection<AccountSubCategory> _subCategories;

    /// <summary>
    /// Gets the read-only collection of payment methods associated with this account.
    /// </summary>
    public IReadOnlyCollection<AccountPaymentMethod> PaymentMethods => [.. _paymentMethods];
    private ICollection<AccountPaymentMethod> _paymentMethods;

    /// <summary>
    /// Gets the read-only collection of members (users) associated with this account.
    /// </summary>
    public IReadOnlyCollection<AccountUser> Members => [.. _members];
    private ICollection<AccountUser> _members;

    /// <summary>
    /// Gets the read-only collection of pending invites for this account.
    /// </summary>
    public IReadOnlyCollection<Invite> Invites => [.. _invites];
    private ICollection<Invite> _invites;

    protected Account()
    {
        Id = AccountId.New();
        Name = Name.Empty;
        Description = null;
        Currency = Currency.Empty;
        _categories = [];
        _subCategories = [];
        _paymentMethods = [];
        _members = [];
        _invites = [];
    }

    private Account(AccountId accountId, Name name, Description description, Currency currency,
        bool isDeleted)
    {
        Id = accountId;
        Name = name;
        Description = description;
        IsDeleted = isDeleted;
        Currency = currency;

        _categories = [];
        _subCategories = [];
        _paymentMethods = [];
        _members = [];
        _invites = [];
    }

    /// <summary>
    /// Creates a new account asynchronously with the specified properties.
    /// The creating user is automatically added as an Owner.
    /// </summary>
    /// <param name="accountId">The unique identifier for the account.</param>
    /// <param name="name">The account name.</param>
    /// <param name="description">The account description.</param>
    /// <param name="currency">The account currency.</param>
    /// <param name="userId">The ID of the user creating the account (will be added as Owner).</param>
    /// <param name="validator">The account validator for business rule validation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the domain result and the created account (null if validation fails).</returns>
    public static async Task<(DomainResult, Account?)> NewAsync(
        AccountId accountId,
        Name name,
        Description description,
        Currency currency,
        Guid userId,
        IAccountValidator validator,
        CancellationToken cancellationToken)
    {
        var entity = new Account(accountId, name, description, currency, false);

        entity._members.Add(new AccountUser(entity, new User() { Id = userId }, PermissionType.Owner, 1));

        var result = await entity.ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            entity.RaiseDomainEvent(AccountCreatedDomainEvent.FromAggregator(entity, userId));
        }

        return (result, result.IsSuccess ? entity : null);
    }

    /// <summary>
    /// Adds a template category to the account. Template categories are predefined categories
    /// that can be shared across multiple accounts.
    /// </summary>
    /// <param name="category">The template category to add. Must be a template (Owner is null) and not deleted.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult AddTemplateCategory(Category category, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (Category.IsNullOrEmpty(category))
        {
            AddNotification(nameof(Category), AccountErrorMessages.CategoryCannotBeNull, AccountErrorCode.CategoryCannotBeNull);
        }

        if (category is { IsTemplate: false })
        {
            AddNotification(nameof(Category), AccountErrorMessages.CategoryMustBeTemplate, AccountErrorCode.CategoryMustBeTemplate);
        }

        if (category is { IsDeleted: false } && _categories.Any(x => x.CategoryId == category.Id))
        {
            AddNotification(nameof(Category), AccountErrorMessages.CategoryAlreadyAdded, AccountErrorCode.CategoryAlreadyAdded);
        }

        if (category is { IsDeleted: true })
        {
            AddNotification(nameof(Category), AccountErrorMessages.CannotAddDeletedCategory, AccountErrorCode.CannotAddDeletedCategory);
        }

        var result = Validate();

        if (!result.IsSuccess)
            return result;

        var nextOrder = GetNextOrder(_categories, x => x.Order);

        _categories.Add(new AccountCategory(this, category!, nextOrder));

        RaiseDomainEvent(AccountCategoryAddedDomainEvent.FromAggregator(this, userId));

        return result;
    }

    /// <summary>
    /// Creates a new custom category specific to this account.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="description">The category description.</param>
    /// <param name="type">The movement type for the category.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult CreateCustomCategory(Name name, Description description, MovementType type, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        var result = Validate();
        if (result.IsFailure)
        {
            return result;
        }

        var (categoryResult, category) = Category.New(name, description, type, Id);
        if (categoryResult.IsFailure)
        {
            return categoryResult;
        }

        var nextOrder = GetNextOrder(_categories, x => x.Order);
        _categories.Add(new AccountCategory(this, category!, nextOrder));
        RaiseDomainEvent(AccountCategoryAddedDomainEvent.FromAggregator(this, userId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Adds a template subcategory to the account. The subcategory's category and 
    /// optional payment method must already belong to this account.
    /// </summary>
    /// <param name="subCategory">The template subcategory to add. Must be a template and not deleted.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult AddTemplateSubCategory(SubCategory subCategory, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (subCategory == null)
        {
            AddNotification(nameof(SubCategory), AccountErrorMessages.SubCategoryCannotBeNull, AccountErrorCode.SubCategoryCannotBeNull);
        }

        if (subCategory != null && !subCategory.IsTemplate)
        {
            AddNotification(nameof(SubCategory), AccountErrorMessages.SubCategoryMustBeTemplate, AccountErrorCode.SubCategoryMustBeTemplate);
        }

        if (subCategory != null && _subCategories.Any(x => x.SubCategoryId == subCategory.Id))
        {
            AddNotification(nameof(SubCategory), AccountErrorMessages.SubCategoryAlreadyAdded, AccountErrorCode.SubCategoryAlreadyAdded);
        }

        if (subCategory != null)
        {
            ValidateCategoryBelongsToAccount(subCategory.CategoryId, nameof(SubCategory));
        }

        if (subCategory != null && subCategory.DefaultPaymentMethodId.HasValue)
        {
            ValidatePaymentMethodBelongsToAccount(
                subCategory.DefaultPaymentMethodId.Value,
                nameof(SubCategory),
                AccountErrorCode.SubCategoryPaymentMethodNotInAccount,
                AccountErrorMessages.SubCategoryPaymentMethodNotInAccount);
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            var nextOrder = GetNextOrder(_subCategories, x => x.Order);
            _subCategories.Add(new AccountSubCategory(this, subCategory!, nextOrder));
            RaiseDomainEvent(AccountSubCategoryAddedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    /// <summary>
    /// Creates a new custom subcategory specific to this account. The category and 
    /// optional payment method must already belong to this account.
    /// </summary>
    /// <param name="name">The subcategory name.</param>
    /// <param name="description">The subcategory description.</param>
    /// <param name="category">The parent category. Must belong to this account.</param>
    /// <param name="defaultPaymentMethod">Optional default payment method. If provided, must belong to this account and match the category type.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult CreateCustomSubCategory(
        Name name,
        Description description,
        Category category,
        PaymentMethod? defaultPaymentMethod,
        Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (category == null)
        {
            AddNotification(nameof(Category), AccountErrorMessages.CategoryCannotBeNull, AccountErrorCode.CategoryCannotBeNull);
        }

        if (category != null)
        {
            ValidateCategoryBelongsToAccount(category.Id, nameof(Category));
        }

        if (defaultPaymentMethod != null)
        {
            ValidatePaymentMethodBelongsToAccount(
                defaultPaymentMethod.Id,
                nameof(PaymentMethod),
                AccountErrorCode.SubCategoryPaymentMethodNotInAccount,
                AccountErrorMessages.SubCategoryPaymentMethodNotInAccount);
        }

        var result = Validate();
        if (result.IsFailure)
        {
            return result;
        }

        var (subCategoryResult, subCategory) = SubCategory.New(name, description, category, Id);
        if (subCategoryResult.IsFailure)
        {
            return subCategoryResult;
        }

        if (defaultPaymentMethod != null)
        {
            var setPaymentMethodResult = subCategory!.SetDefaultPaymentMethod(defaultPaymentMethod);
            if (setPaymentMethodResult.IsFailure)
            {
                return setPaymentMethodResult;
            }
        }

        var nextOrder = GetNextOrder(_subCategories, x => x.Order);
        _subCategories.Add(new AccountSubCategory(this, subCategory!, nextOrder));
        RaiseDomainEvent(AccountSubCategoryAddedDomainEvent.FromAggregator(this, userId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Adds a template payment method to the account. Template payment methods are predefined
    /// payment methods that can be shared across multiple accounts.
    /// </summary>
    /// <param name="paymentMethod">The template payment method to add. Must be a template and not deleted.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult AddTemplatePaymentMethod(PaymentMethod paymentMethod, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (paymentMethod == null)
        {
            AddNotification(nameof(PaymentMethod), AccountErrorMessages.PaymentMethodCannotBeNull, AccountErrorCode.PaymentMethodCannotBeNull);
        }

        if (paymentMethod != null && !paymentMethod.IsTemplate)
        {
            AddNotification(nameof(PaymentMethod), AccountErrorMessages.PaymentMethodMustBeTemplate, AccountErrorCode.PaymentMethodMustBeTemplate);
        }

        if (paymentMethod != null && _paymentMethods.Any(x => x.PaymentMethodId == paymentMethod.Id))
        {
            AddNotification(nameof(PaymentMethod), AccountErrorMessages.PaymentMethodAlreadyAdded, AccountErrorCode.PaymentMethodAlreadyAdded);
        }

        if (paymentMethod != null && paymentMethod.IsDeleted)
        {
            AddNotification(nameof(PaymentMethod), AccountErrorMessages.CannotAddDeletedPaymentMethod, AccountErrorCode.CannotAddDeletedPaymentMethod);
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            var nextOrder = GetNextOrder(_paymentMethods, x => x.Order);
            _paymentMethods.Add(new AccountPaymentMethod(this, paymentMethod!, nextOrder));
            RaiseDomainEvent(AccountPaymentMethodAddedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    /// <summary>
    /// Creates a new custom payment method specific to this account.
    /// </summary>
    /// <param name="name">The payment method name.</param>
    /// <param name="description">The payment method description.</param>
    /// <param name="type">The movement type for the payment method.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult CreateCustomPaymentMethod(Name name, Description description, MovementType type, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        var result = Validate();
        if (result.IsFailure)
        {
            return result;
        }

        var (paymentMethodResult, paymentMethod) = PaymentMethod.New(name, description, type, Id);
        if (paymentMethodResult.IsFailure)
        {
            return paymentMethodResult;
        }

        var nextOrder = GetNextOrder(_paymentMethods, x => x.Order);

        _paymentMethods.Add(new AccountPaymentMethod(this, paymentMethod!, nextOrder));
        RaiseDomainEvent(AccountPaymentMethodAddedDomainEvent.FromAggregator(this, userId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Links (adds) a user as a member of this account with the specified permission level.
    /// Only account Owners can add new members.
    /// </summary>
    /// <param name="user">The user to add as a member. Cannot be null or already a member.</param>
    /// <param name="permissionType">The permission level to grant to the new member (Viewer, Editor, or Owner).</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult LinkMember(User user, PermissionType permissionType, Guid userId)
    {
        ValidateOwnerPermission(userId);

        if (user == null)
        {
            AddNotification(nameof(User), AccountErrorMessages.UserCannotBeNull, AccountErrorCode.UserCannotBeNull);
        }

        if (user != null && _members.Any(x => x.UserId.Equals(user.Id)))
        {
            AddNotification(nameof(User), AccountErrorMessages.UserAlreadyMember, AccountErrorCode.UserAlreadyMember);
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            var nextOrder = GetNextOrder(_members, x => x.Order);
            _members.Add(new AccountUser(this, user!, permissionType, nextOrder));
            RaiseDomainEvent(AccountMemberAddedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    /// <summary>
    /// Removes a member from this account. Only account Owners can remove members.
    /// Cannot remove the last Owner from the account.
    /// </summary>
    /// <param name="user">The user to remove from the account members. Cannot be null.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult RemoveMember(User user, Guid userId)
    {
        ValidateOwnerPermission(userId);

        if (user == null)
        {
            AddNotification(nameof(User), AccountErrorMessages.UserCannotBeNull, AccountErrorCode.UserCannotBeNull);
        }

        var memberToRemove = user != null ? _members.FirstOrDefault(x => x.UserId.Equals(user.Id)) : null;
        if (memberToRemove == null)
        {
            AddNotification(nameof(User), AccountErrorMessages.UserNotMember, AccountErrorCode.UserNotMember);
        }

        // Validate: Cannot remove the last owner
        if (memberToRemove?.PermissionType == PermissionType.Owner)
        {
            var ownerCount = _members.Count(x => x.PermissionType == PermissionType.Owner);
            if (ownerCount <= 1)
            {
                AddNotification(nameof(User), AccountErrorMessages.AccountShouldHaveOneOwner, AccountErrorCode.AccountShouldHaveOneOwner);
            }
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            _ = _members.Remove(memberToRemove!);
            RaiseDomainEvent(AccountMemberRemovedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    /// <summary>
    /// Invites an existing system user to become a member of this account.
    /// Only account Owners can invite new members.
    /// </summary>
    /// <param name="invitedUserId">The ID of the existing user to invite. Cannot already be a member or have an active invite.</param>
    /// <param name="permissionType">The permission level to grant when the invite is accepted.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult InviteMemberByUser(UserId invitedUserId, PermissionType permissionType, Guid userId)
    {
        ValidateOwnerPermission(userId);

        if (invitedUserId == default)
        {
            AddNotification(nameof(UserId), AccountErrorMessages.InviteCannotBeNull, AccountErrorCode.InviteCannotBeNull);
        }

        if (invitedUserId != default && _members.Any(x => x.UserId == invitedUserId.Value))
        {
            AddNotification(nameof(UserId), AccountErrorMessages.CannotInviteExistingMember, AccountErrorCode.CannotInviteExistingMember);
        }

        if (invitedUserId != default && _invites.Any(x => x.UserId.HasValue && x.UserId.Value == invitedUserId && x.IsActive))
        {
            AddNotification(nameof(UserId), AccountErrorMessages.InviteAlreadyExists, AccountErrorCode.InviteAlreadyExists);
        }

        var result = Validate();

        if (result.IsFailure)
        {
            return result;
        }

        var (inviteResult, invite) = Invite.NewForUser(Id, invitedUserId, permissionType);

        if (inviteResult.IsFailure)
        {
            return inviteResult;
        }

        _invites.Add(invite!);
        RaiseDomainEvent(AccountMemberInvitedDomainEvent.FromAggregator(this, userId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Invites a person by email to become a member of this account. This is used when the person
    /// doesn't have a system account yet. The invite will be linked to their user ID when they create an account.
    /// Only account Owners can invite new members.
    /// </summary>
    /// <param name="email">The email address of the person to invite. Cannot have an active invite already.</param>
    /// <param name="permissionType">The permission level to grant when the invite is accepted.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult InviteMemberByEmail(Email email, PermissionType permissionType, Guid userId)
    {
        ValidateOwnerPermission(userId);

        if (email == null)
        {
            AddNotification(nameof(Email), AccountErrorMessages.InviteCannotBeNull, AccountErrorCode.InviteCannotBeNull);
        }

        if (email != null && _invites.Any(x => x.Email != null && x.Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase) && x.IsActive))
        {
            AddNotification(nameof(Email), AccountErrorMessages.InviteAlreadyExists, AccountErrorCode.InviteAlreadyExists);
        }

        var result = Validate();

        if (result.IsFailure)
        {
            return result;
        }

        var (inviteResult, invite) = Invite.NewForEmail(Id, email!, permissionType);

        if (inviteResult.IsFailure)
        {
            return inviteResult;
        }

        _invites.Add(invite!);
        RaiseDomainEvent(AccountMemberInvitedDomainEvent.FromAggregator(this, userId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Links a user ID to an invite that was sent by email. This is called when a person
    /// creates a system account using an email that has a pending invite.
    /// </summary>
    /// <param name="email">The email address of the pending invite.</param>
    /// <param name="newUserId">The user ID of the newly created account to link to the invite.</param>
    /// <param name="executingUserId">The ID of the user performing the action.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult LinkUserToInvite(Email email, UserId newUserId, Guid executingUserId)
    {
        var invite = _invites.FirstOrDefault(x =>
            x.Email != null &&
            x.Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase) &&
            x.IsActive &&
            !x.UserId.HasValue);

        if (invite == null)
        {
            AddNotification(nameof(Invite), AccountErrorMessages.InviteNotFound, AccountErrorCode.InviteNotFound);
            return Validate();
        }

        var result = invite.LinkUser(newUserId);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(AccountInviteUserLinkedDomainEvent.FromAggregator(this, executingUserId));

        return DomainResult.Success();
    }

    /// <summary>
    /// Accepts an invite to join this account. The user must have an active invite and
    /// the invite must be linked to their user ID. A domain event is raised to trigger
    /// the addition of the user as a member via an event handler.
    /// </summary>
    /// <param name="inviteId">The ID of the invite to accept.</param>
    /// <param name="userId">The ID of the user accepting the invite. Must match the invite's user ID.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult RespondInvite(InviteId inviteId, Guid userId)
    {
        var invite = _invites.FirstOrDefault(x => x.Id == inviteId);

        if (invite == null)
        {
            AddNotification(nameof(Invite), AccountErrorMessages.InviteNotFound, AccountErrorCode.InviteNotFound);
            return Validate();
        }

        if (!invite.UserId.HasValue || invite.UserId.Value.Value != userId)
        {
            AddNotification(nameof(userId), AccountErrorMessages.UserNotInvited, AccountErrorCode.UserNotInvited);
            return Validate();
        }

        var result = invite.Accept();

        if (result.IsSuccess)
        {
            RaiseDomainEvent(AccountMemberInviteAcceptedDomainEvent.FromAggregator(
                this,
                inviteId.Value,
                userId,
                userId));
        }

        return result;
    }

    /// <summary>
    /// Rejects an invite to join this account. The user must have an active invite and
    /// the invite must be linked to their user ID.
    /// </summary>
    /// <param name="inviteId">The ID of the invite to reject.</param>
    /// <param name="userId">The ID of the user rejecting the invite. Must match the invite's user ID.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult RejectInvite(InviteId inviteId, Guid userId)
    {
        var invite = _invites.FirstOrDefault(x => x.Id == inviteId);

        if (invite == null)
        {
            AddNotification(nameof(Invite), AccountErrorMessages.InviteNotFound, AccountErrorCode.InviteNotFound);
            return Validate();
        }

        if (!invite.UserId.HasValue || invite.UserId.Value.Value != userId)
        {
            AddNotification(nameof(userId), AccountErrorMessages.UserNotInvited, AccountErrorCode.UserNotInvited);
            return Validate();
        }

        var result = invite.Reject();

        if (result.IsSuccess)
        {
            RaiseDomainEvent(AccountMemberInviteDeclinedDomainEvent.FromAggregator(
                this,
                inviteId.Value,
                userId,
                userId));
        }

        return result;
    }

    /// <summary>
    /// Adds a financial movement to this account. The movement's subcategory, payment method,
    /// and category must all belong to this account. The payment method type must match the category type.
    /// </summary>
    /// <param name="movement">The financial movement to add. Cannot be null.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult AddFinancialMovement(FinancialMovement movement, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (movement == null)
        {
            AddNotification(nameof(FinancialMovement),
                FinancialMovementErrorMessages.FinancialMovementCannotBeNull,
                FinancialMovementErrorCode.FinancialMovementCannotBeNull);
        }

        if (movement != null)
        {
            ValidateSubCategoryBelongsToAccount(movement.SubCategoryId, nameof(FinancialMovement));
            ValidatePaymentMethodBelongsToAccount(
                movement.PaymentMethodId,
                nameof(FinancialMovement),
                FinancialMovementErrorCode.PaymentMethodNotInAccount,
                FinancialMovementErrorMessages.PaymentMethodNotInAccount);
        }

        if (movement != null && movement.SubCategory != null)
        {
            ValidateCategoryBelongsToAccount(movement.SubCategory.CategoryId, nameof(FinancialMovement));
        }

        // Validate payment method type matches category type
        if (movement?.SubCategory?.Category != null && movement.PaymentMethod != null &&
            movement.PaymentMethod.Type != movement.SubCategory.Type)
        {
            AddNotification(nameof(FinancialMovement),
                FinancialMovementErrorMessages.PaymentMethodTypeMismatch,
                FinancialMovementErrorCode.PaymentMethodTypeMismatch);
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            RaiseDomainEvent(AccountFinancialMovementAddedDomainEvent.FromAggregator(this, movement!, userId));
        }

        return result;
    }

    /// <summary>
    /// Removes a financial movement from this account.
    /// </summary>
    /// <param name="movement">The financial movement to remove. Cannot be null.</param>
    /// <param name="userId">The ID of the user performing the action. Must be an Editor or Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult RemoveFinancialMovement(FinancialMovement movement, Guid userId)
    {
        ValidateEditorOrOwnerPermission(userId);

        if (movement == null)
        {
            AddNotification(nameof(FinancialMovement), AccountErrorMessages.FinancialMovementCannotBeNull, AccountErrorCode.FinancialMovementCannotBeNull);
        }

        var result = Validate();

        if (result.IsSuccess)
        {
            RaiseDomainEvent(AccountFinancialMovementRemovedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    /// <summary>
    /// Soft deletes this account by marking it as deleted. The account data is preserved
    /// but will not be accessible in normal operations.
    /// Only account Owners can delete an account.
    /// </summary>
    /// <param name="userId">The ID of the user performing the deletion. Must be an Owner.</param>
    /// <returns>A domain result indicating success or failure with validation errors.</returns>
    public DomainResult SoftDelete(Guid userId)
    {
        ValidateOwnerPermission(userId);

        var result = Validate();

        if (result.IsSuccess)
        {
            IsDeleted = true;
            RaiseDomainEvent(AccountDeletedDomainEvent.FromAggregator(this, userId));
        }

        return result;
    }

    #region Private Validation Methods

    /// <summary>
    /// Validates if the user is a member of the account and has Editor or Owner permission.
    /// </summary>
    private void ValidateEditorOrOwnerPermission(Guid userId)
    {
        var member = _members.FirstOrDefault(x => x.UserId == userId);

        if (member == null)
        {
            AddNotification(nameof(userId), AccountErrorMessages.UserNotMember, AccountErrorCode.UserNotMember);
            return;
        }

        if (member.PermissionType != PermissionType.Editor && member.PermissionType != PermissionType.Owner)
        {
            AddNotification(nameof(userId), AccountErrorMessages.InsufficientPermissions, AccountErrorCode.InsufficientPermissions);
        }
    }

    /// <summary>
    /// Validates if the user is a member of the account and has Owner permission.
    /// </summary>
    private void ValidateOwnerPermission(Guid userId)
    {
        var member = _members.FirstOrDefault(x => x.UserId == userId);

        if (member == null)
        {
            AddNotification(nameof(userId), AccountErrorMessages.UserNotMember, AccountErrorCode.UserNotMember);
            return;
        }

        if (member.PermissionType != PermissionType.Owner)
        {
            AddNotification(nameof(userId), AccountErrorMessages.OnlyOwnerCanInviteMembers, AccountErrorCode.OnlyOwnerCanInviteMembers);
        }
    }

    /// <summary>
    /// Validates if the category belongs to this account.
    /// </summary>
    private void ValidateCategoryBelongsToAccount(Andor.Accounts.Domain.Categories.ValueObjects.CategoryId categoryId, string propertyName)
    {
        if (_categories.Any(x => x.CategoryId == categoryId))
            return;

        // Use specific error code based on context
        if (propertyName == nameof(FinancialMovement))
        {
            AddNotification(propertyName,
                FinancialMovementErrorMessages.CategoryNotInAccount,
                FinancialMovementErrorCode.CategoryNotInAccount);
        }
        else
        {
            AddNotification(propertyName,
                AccountErrorMessages.SubCategoryCategoryNotInAccount,
                AccountErrorCode.SubCategoryCategoryNotInAccount);
        }
    }

    /// <summary>
    /// Validates if the subcategory belongs to this account.
    /// </summary>
    private void ValidateSubCategoryBelongsToAccount(Andor.Accounts.Domain.SubCategories.ValueObjects.SubCategoryId subCategoryId, string propertyName)
    {
        if (_subCategories.Any(x => x.SubCategoryId == subCategoryId))
            return;

        // Use specific error code based on context
        if (propertyName == nameof(FinancialMovement))
        {
            AddNotification(propertyName,
                FinancialMovementErrorMessages.SubCategoryNotInAccount,
                FinancialMovementErrorCode.SubCategoryNotInAccount);
        }
        else
        {
            AddNotification(propertyName,
                AccountErrorMessages.FinancialMovementSubCategoryNotInAccount,
                AccountErrorCode.FinancialMovementSubCategoryNotInAccount);
        }
    }

    /// <summary>
    /// Validates if the payment method belongs to this account.
    /// </summary>
    private void ValidatePaymentMethodBelongsToAccount(Andor.Accounts.Domain.PaymentMethods.ValueObjects.PaymentMethodId paymentMethodId, string propertyName, Andor.Domain.Common.ValuesObjects.DomainErrorCode errorCode, string errorMessage)
    {
        if (_paymentMethods.All(x => x.PaymentMethodId != paymentMethodId))
        {
            AddNotification(propertyName, errorMessage, errorCode);
        }
    }

    /// <summary>
    /// Calculates the next Order value for a collection.
    /// </summary>
    private int GetNextOrder<T>(IEnumerable<T> collection, Func<T, int> orderSelector)
    {
        return collection.Any()
            ? collection.Max(orderSelector) + 1
            : 1;
    }

    #endregion
}
