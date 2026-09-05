using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetMyProfilePicture;

[Authorize]
public record GetMyProfilePictureQuery : IRequest<DocumentFileResult?>;
