using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;

public class UploadCompanyDocumentCommandValidator : AbstractValidator<UploadCompanyDocumentCommand>
{
    public UploadCompanyDocumentCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x).Must(x => AllowedFileTypes.IsAllowed(x.ContentType, x.FileName, x.Content.Length))
            .WithMessage("File type not allowed or exceeds the size limit. Please make sure file size is less then 10 MB.");
    }
}
