namespace Family.Budget.Application.Models.Authorization
{
    public interface ICurrentUserService
    {
        ApplicationUser User { get; }
    }
}
