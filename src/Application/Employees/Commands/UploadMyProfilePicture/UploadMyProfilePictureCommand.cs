using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.UploadMyProfilePicture;

// No Roles on [Authorize] — any authenticated user with an Employee record may set
// their own picture. Always resolved from _user.Id, never a passed-in employee id.
[Authorize]
public record UploadMyProfilePictureCommand : IRequest
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
