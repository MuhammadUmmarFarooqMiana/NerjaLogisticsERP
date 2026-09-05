namespace NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;

public class RefreshCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
