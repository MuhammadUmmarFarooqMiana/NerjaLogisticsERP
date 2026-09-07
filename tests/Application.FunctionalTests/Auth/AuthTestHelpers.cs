using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.Auth;

internal static class AuthTestHelpers
{
    public const string DefaultPassword = "Testing1234!";

    /// <summary>
    /// Creates a real Identity user (via TestApp.RunAsUserAsync, so login can authenticate
    /// against a real password hash) plus a linked Employee row in the given AccountStatus —
    /// LoginCommandHandler/RefreshCommandHandler both look the Employee up by UserId, so a
    /// bare Identity user with no Employee row is a distinct case, not covered by this helper.
    /// Returns the login email (== the generated username) and the Employee's own Id, for
    /// callers that need to fetch and further mutate the employee later in the test (e.g.
    /// terminating them partway through, after they've already logged in).
    /// </summary>
    public static async Task<(string Email, Guid EmployeeId)> CreateLoginableEmployeeAsync(AccountStatus status)
    {
        var email = $"{Guid.NewGuid():N}@test.local";
        var userId = await TestApp.RunAsUserAsync(email, DefaultPassword, []);

        var employee = Employee.Create(Guid.Parse(userId), "Test Employee");

        if (status is not AccountStatus.Incomplete)
        {
            employee.SubmitProfileForReview(
                iqamaNumber: "TEST-0000000001",
                platformIdNumber: null,
                idExpiryDate: null,
                iqamaExpiryDate: null,
                drivingLicenseExpiryDate: null,
                insuranceExpiryDate: null);
        }

        switch (status)
        {
            case AccountStatus.Incomplete:
            case AccountStatus.PendingReview:
                break; // already at the right status after Create()/SubmitProfileForReview()
            case AccountStatus.Active:
                employee.Approve();
                break;
            case AccountStatus.Suspended:
                employee.Approve();
                employee.Suspend();
                break;
            case AccountStatus.Terminated:
                employee.Terminate();
                break;
            case AccountStatus.Rejected:
                employee.Reject("Test rejection reason.");
                break;
        }

        await TestApp.AddAsync(employee);

        return (email, employee.Id);
    }
}
