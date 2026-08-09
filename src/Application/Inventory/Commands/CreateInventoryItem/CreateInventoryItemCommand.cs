using System;
using System.Collections.Generic;
using System.Text;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Inventory.Commands.CreateInventoryItem;

public record CreateInventoryItemCommand : IRequest<Guid>
{
    public string ItemName { get; init; } = string.Empty;
    public string Unit { get; init; } = "pcs";
    public int ReorderLevel { get; init; }
}
