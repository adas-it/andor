namespace Family.Budget.Domain.Entities.Registrations.Repository;

using Family.Budget.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRegistrationRepository : IRepository<Registration>
{
    Task<Registration?> GetByEmail(string email, CancellationToken cancellationToken);
    Task<List<Registration>> GetOldRegistrations(DateTimeOffset dateTimeOffset, CancellationToken cancellationToken);
}