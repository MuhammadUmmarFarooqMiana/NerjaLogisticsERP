using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.RemoveMyProfilePicture;

[Authorize]
public record RemoveMyProfilePictureCommand : IRequest;
