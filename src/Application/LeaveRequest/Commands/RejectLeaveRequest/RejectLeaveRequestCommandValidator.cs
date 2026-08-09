namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

public class RejectLeaveRequestCommandValidator : AbstractValidator<RejectLeaveRequestCommand>
{
    public RejectLeaveRequestCommandValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(5000);
    }
}
