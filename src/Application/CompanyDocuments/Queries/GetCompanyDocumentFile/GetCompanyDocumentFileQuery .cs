namespace NerjaLogisticsERP.Application.Common.Models;


public record GetCompanyDocumentFileQuery : IRequest<DocumentFileResult>
{
    public Guid Id { get; init; }
}
