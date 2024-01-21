namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Entities.Accounts.DomainEvents;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.SeedWork;
using System;

public class Invite : Entity
{
    public string Email { get; private set; }
    public InviteStatus Status { get; private set; }
    public AccountId AccountId { get; private set; }
    public Account Account { get; private set; }
    private Invite()
    {
    }

    private Invite(Guid id, string email, InviteStatus status, AccountId accountId, Account account)
    {
        Id = id;
        Email = email;
        Status = status;
        AccountId = accountId;
        Account = account;
    }

    public static Invite New(string email, InviteStatus status, Account account)
    {
        var entity = new Invite(Guid.NewGuid(), email, status, account.Id, account);

        entity.RaiseDomainEvent(new InviteCreatedDomainEvent(entity));

        return entity;
    }

    public void SetStatus(InviteStatus status)
    {
        Status = status;
    }
}
