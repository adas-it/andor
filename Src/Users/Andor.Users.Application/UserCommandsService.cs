using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Users.Application.Commands;
using Andor.Users.Application.Interfaces;
using Andor.Users.Application.Messages;
using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users;
using Andor.Users.Domain.Users.Repositories;

namespace Andor.Users.Application;

public class UserCommandsService(
    ICommandsUserRepository repository,
    IUserValidator validator,
    IMessageSenderInterface messageSender) : IUserCommandsService
{
    public async Task<ApplicationResult<UserPreferencesOutput?>> CreateUserAsync(CreateUserCommand command)
    {
        var response = ApplicationResult<UserPreferencesOutput?>.Success();

        var existing = await repository.GetByIdAsync(command.Id, command.CancellationToken);

        if (existing is not null)
        {
            await RequestProvisioningAsync(command, existing.ToUserPreferencesOutput());
            return response.SetData(existing.ToUserPreferencesOutput());
        }

        var (result, user) = await User.NewAsync(
            command.Id,
            command.Email,
            command.FirstName,
            command.LastName,
            command.PreferredCurrencyId,
            command.PreferredLanguageId,
            command.MarketingOptIn,
            command.TermsAndConditionsAccepted,
            command.PrivacyPolicyAccepted,
            validator,
            command.CancellationToken);

        if (result.IsFailure || user is null)
        {
            foreach (var _ in result.Errors)
            {
                response.AddError(Errors.Validation());
            }

            return response;
        }

        await repository.PersistAsync(user, command.CancellationToken);

        await RequestProvisioningAsync(command, user.ToUserPreferencesOutput());

        return response.SetData(user.ToUserPreferencesOutput());
    }

    // The User aggregate itself has no opinion on Identity credentials or a default Account —
    // those are this module's downstream neighbors, requested here (not raised as a User domain
    // event) since they're this orchestration's job, not the aggregate's. Re-requesting on an
    // already-provisioned User is intentional: it lets a retried call still nudge Identity/Accounts
    // if their own step didn't complete the first time, since both consumers are idempotent.
    private async Task RequestProvisioningAsync(CreateUserCommand command, UserPreferencesOutput user)
    {
        var name = $"{command.FirstName} {command.LastName}".Trim();

        await messageSender.QueueSendAsync(
            "IdentityProvisioning",
            new IdentityProvisioningRequested(user.Id, user.Email, name, command.PasswordHash),
            $"identity-{user.Id:N}",
            command.CancellationToken);

        await messageSender.QueueSendAsync(
            "AccountProvisioning",
            new AccountProvisioningRequested(user.Id, user.Email, name),
            $"account-{user.Id:N}",
            command.CancellationToken);
    }
}
