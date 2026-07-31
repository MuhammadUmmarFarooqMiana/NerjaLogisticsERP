using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<string?> GetUserNameAsync(Guid userId);

    Task<bool> IsInRoleAsync(string userId, string role);
    
    Task<bool> EmailExistsAsync(string email);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    [Obsolete]Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password);

    Task<(Result Result, Guid UserId)> CreateUserAsync(
    string email, string password, string phoneNumber, bool hasWhatsApp);

    Task<Result> AddToRoleAsync(Guid userId, string role);

    Task<Result> DeleteUserAsync(string userId);

    Task<Guid?> ValidateCredentialsAsync(string email, string password);

    Task<IList<string>> GetRolesAsync(Guid userId);
}
