namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.CreateCompanyDocumentFolder;

public class CreateCompanyDocumentFolderCommandValidator : AbstractValidator<CreateCompanyDocumentFolderCommand>
{
    public CreateCompanyDocumentFolderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
