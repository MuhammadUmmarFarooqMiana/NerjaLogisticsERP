using Microsoft.AspNetCore.Identity;

namespace NerjaLogisticsERP.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
    }

    private ApplicationUser(
        string phoneNumber,
        bool hasWhatsApp)
    {
        PhoneNumber = phoneNumber;
        HasWhatsApp = hasWhatsApp;
    }

    public bool HasWhatsApp { get; private set; } = true;

    public static ApplicationUser Create(
        string phoneNumber,
        bool hasWhatsApp = true)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException(
                "Phone number is required.",
                nameof(phoneNumber));

        return new ApplicationUser(
            phoneNumber,
            hasWhatsApp);
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException(
                "Phone number is required.",
                nameof(phoneNumber));

        PhoneNumber = phoneNumber;
    }


    public void EnableWhatsApp()
    {
        HasWhatsApp = true;
    }

    public void DisableWhatsApp()
    {
        HasWhatsApp = false;
    }

}
