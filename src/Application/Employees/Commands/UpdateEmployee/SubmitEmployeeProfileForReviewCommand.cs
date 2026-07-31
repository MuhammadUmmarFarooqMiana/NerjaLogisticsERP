using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.UpdateEmployee;

[Authorize]
public record SubmitEmployeeProfileForReviewCommand : IRequest;
