using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommandValidator : AbstractValidator<UploadEmployeeDocumentCommand>
{
    public UploadEmployeeDocumentCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x).Must(x => AllowedFileTypes.IsAllowed(x.ContentType, x.FileName, x.Content.Length))
            .WithMessage("File type not allowed or exceeds the size limit. Please make sure file size is less then 10 MB.");
    }
}
