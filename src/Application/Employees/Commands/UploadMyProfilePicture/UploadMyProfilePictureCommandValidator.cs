using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.UploadMyProfilePicture;

public class UploadMyProfilePictureCommandValidator : AbstractValidator<UploadMyProfilePictureCommand>
{
    public UploadMyProfilePictureCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        // A profile picture specifically means a photo — narrower than the general
        // document upload's allowed set (which also permits PDF/Word/Excel).
        RuleFor(x => x).Must(x =>
                (x.ContentType is "image/jpeg" or "image/png") &&
                AllowedFileTypes.IsAllowed(x.ContentType, x.FileName, x.Content.Length))
            .WithMessage("Only JPEG or PNG images up to 5 MB are allowed.");
    }
}
