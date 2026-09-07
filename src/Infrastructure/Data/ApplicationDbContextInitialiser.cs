using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Infrastructure.Identity;

namespace NerjaLogisticsERP.Infrastructure.Data;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    private static readonly string[] AllRoles =
    {
        Roles.Administrator,
        Roles.SoftwareEngineer,
        Roles.Supervisor,
        Roles.Accountant,
        Roles.Rider
    };

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            //await _context.Database.EnsureDeletedAsync();
            //await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    /// <summary>
    /// Creates the app's fixed set of roles, if they don't already exist. Idempotent and
    /// carries no secrets — safe to call in every environment, and role-based [Authorize]
    /// checks can't pass for anyone until these rows exist.
    /// </summary>
    public async Task SeedRolesAsync()
    {
        try
        {
            foreach (var roleName in AllRoles)
            {
                if (_roleManager.Roles.All(r => r.Name != roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding roles.");
            throw;
        }
    }

    /// <summary>
    /// Creates the built-in Administrator account with a hardcoded, publicly-known password.
    /// Must only ever be wired up to run in Development — see
    /// InitialiserExtensions.SeedDevelopmentAdministratorAsync's remarks.
    /// </summary>
    public async Task SeedDevelopmentAdministratorAsync()
    {
        try
        {
            await TrySeedDevelopmentAdministratorAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the development administrator account.");
            throw;
        }
    }

    private async Task TrySeedDevelopmentAdministratorAsync()
    {
        var administrator = new ApplicationUser { UserName = "administrator@nerja", Email = "administrator@nerja.com", PhoneNumber = "+966000000000", EmailConfirmed = true };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "@Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator });

            // Seeded accounts should be immediately usable — create the Employee
            // profile and approve it in the same step, rather than leaving the
            // built-in Administrator stuck in PendingApproval like a normal
            // self-registered user.
            var employee = Employee.Create(administrator.Id, "System Administrator");
            employee.SubmitProfileForReview(
                iqamaNumber: "ADMIN-0000000001",
                platformIdNumber: null,
                idExpiryDate: null,
                iqamaExpiryDate: null,
                drivingLicenseExpiryDate: null,
                insuranceExpiryDate: null);

            employee.Approve();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
    }
}
