using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts;

namespace Andor.Accounts.Application;

internal static class AccountMapperExtensions
{
    public static AccountOutput? ToAccountOutput(this Account? entity)
    {
        if (entity == null)
            return null;

        return new AccountOutput()
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Deleted = entity.IsDeleted,
            Participants = entity.Members
                .Select(m => new ParticipantOutput() { Id = m.UserId.ToString() }).ToList()
        };
    }
}
