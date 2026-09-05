using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Reports.Advances;
using NerjaLogisticsERP.Application.Reports.Advances.Queries.ExportAdvancesReport;
using NerjaLogisticsERP.Application.Reports.Advances.Queries.GetAdvancesReport;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Expenses;
using NerjaLogisticsERP.Application.Reports.Expenses.Queries.ExportExpensesReport;
using NerjaLogisticsERP.Application.Reports.Expenses.Queries.GetExpensesReport;
using NerjaLogisticsERP.Application.Reports.Fines;
using NerjaLogisticsERP.Application.Reports.Fines.Queries.ExportFinesReport;
using NerjaLogisticsERP.Application.Reports.Fines.Queries.GetFinesReport;
using NerjaLogisticsERP.Application.Reports.Inventory;
using NerjaLogisticsERP.Application.Reports.Inventory.Queries.ExportInventoryLedgerReport;
using NerjaLogisticsERP.Application.Reports.Inventory.Queries.GetInventoryLedgerReport;
using NerjaLogisticsERP.Application.Reports.Leaves;
using NerjaLogisticsERP.Application.Reports.Leaves.Queries.ExportLeavesReport;
using NerjaLogisticsERP.Application.Reports.Leaves.Queries.GetLeavesReport;
using NerjaLogisticsERP.Application.Reports.Orders;
using NerjaLogisticsERP.Application.Reports.Orders.Queries.ExportOrdersReport;
using NerjaLogisticsERP.Application.Reports.Orders.Queries.GetOrdersReport;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.ExportReconciliationReport;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.GenerateReconciliationReport;
using NerjaLogisticsERP.Application.Reports.Salaries;
using NerjaLogisticsERP.Application.Reports.Salaries.Queries.ExportSalariesReport;
using NerjaLogisticsERP.Application.Reports.Salaries.Queries.GetSalariesReport;
using NerjaLogisticsERP.Application.Reports.Suppliers;
using NerjaLogisticsERP.Application.Reports.Suppliers.Queries.ExportSuppliersReport;
using NerjaLogisticsERP.Application.Reports.Suppliers.Queries.GetSuppliersReport;
using NerjaLogisticsERP.Application.Reports.Vehicles;
using NerjaLogisticsERP.Application.Reports.Vehicles.Queries.ExportVehiclesReport;
using NerjaLogisticsERP.Application.Reports.Vehicles.Queries.GetVehiclesReport;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ApiControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<OrdersReportDto>> GetOrdersReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? platformId,
        [FromQuery] Guid? employeeId)
    {
        var result = await Mediator.Send(new GetOrdersReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            PlatformId = platformId,
            EmployeeId = employeeId
        });
        return Ok(result);
    }

    [HttpGet("orders/export")]
    public async Task<IActionResult> ExportOrdersReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? platformId,
        [FromQuery] Guid? employeeId)
    {
        var file = await Mediator.Send(new ExportOrdersReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            PlatformId = platformId,
            EmployeeId = employeeId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("fines")]
    public async Task<ActionResult<FinesReportDto>> GetFinesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? employeeId)
    {
        var result = await Mediator.Send(new GetFinesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            EmployeeId = employeeId
        });
        return Ok(result);
    }

    [HttpGet("fines/export")]
    public async Task<IActionResult> ExportFinesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? employeeId)
    {
        var file = await Mediator.Send(new ExportFinesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            EmployeeId = employeeId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("advances")]
    public async Task<ActionResult<AdvancesReportDto>> GetAdvancesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? employeeId)
    {
        var result = await Mediator.Send(new GetAdvancesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            EmployeeId = employeeId
        });
        return Ok(result);
    }

    [HttpGet("advances/export")]
    public async Task<IActionResult> ExportAdvancesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? employeeId)
    {
        var file = await Mediator.Send(new ExportAdvancesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            EmployeeId = employeeId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<ExpensesReportDto>> GetExpensesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] ExpenseCategory? category,
        [FromQuery] Guid? platformId)
    {
        var result = await Mediator.Send(new GetExpensesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Category = category,
            PlatformId = platformId
        });
        return Ok(result);
    }

    [HttpGet("expenses/export")]
    public async Task<IActionResult> ExportExpensesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] ExpenseCategory? category,
        [FromQuery] Guid? platformId)
    {
        var file = await Mediator.Send(new ExportExpensesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Category = category,
            PlatformId = platformId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("leaves")]
    public async Task<ActionResult<LeavesReportDto>> GetLeavesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] LeaveStatus? status,
        [FromQuery] Guid? employeeId)
    {
        var result = await Mediator.Send(new GetLeavesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            EmployeeId = employeeId
        });
        return Ok(result);
    }

    [HttpGet("leaves/export")]
    public async Task<IActionResult> ExportLeavesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] LeaveStatus? status,
        [FromQuery] Guid? employeeId)
    {
        var file = await Mediator.Send(new ExportLeavesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            EmployeeId = employeeId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("vehicles")]
    public async Task<ActionResult<VehiclesReportDto>> GetVehiclesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? vehicleId)
    {
        var result = await Mediator.Send(new GetVehiclesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            VehicleId = vehicleId
        });
        return Ok(result);
    }

    [HttpGet("vehicles/export")]
    public async Task<IActionResult> ExportVehiclesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? vehicleId)
    {
        var file = await Mediator.Send(new ExportVehiclesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            VehicleId = vehicleId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("suppliers")]
    public async Task<ActionResult<SuppliersReportDto>> GetSuppliersReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? supplierId)
    {
        var result = await Mediator.Send(new GetSuppliersReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            SupplierId = supplierId
        });
        return Ok(result);
    }

    [HttpGet("suppliers/export")]
    public async Task<IActionResult> ExportSuppliersReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? supplierId)
    {
        var file = await Mediator.Send(new ExportSuppliersReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            SupplierId = supplierId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("inventory-ledger")]
    public async Task<ActionResult<InventoryLedgerReportDto>> GetInventoryLedgerReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? itemId)
    {
        var result = await Mediator.Send(new GetInventoryLedgerReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            ItemId = itemId
        });
        return Ok(result);
    }

    [HttpGet("inventory-ledger/export")]
    public async Task<IActionResult> ExportInventoryLedgerReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? itemId)
    {
        var file = await Mediator.Send(new ExportInventoryLedgerReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            ItemId = itemId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("salaries")]
    public async Task<ActionResult<SalariesReportDto>> GetSalariesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] MonthlySummaryStatus? status,
        [FromQuery] Guid? employeeId)
    {
        var result = await Mediator.Send(new GetSalariesReportQuery
        {
            PeriodType = periodType,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            EmployeeId = employeeId
        });
        return Ok(result);
    }

    [HttpGet("salaries/export")]
    public async Task<IActionResult> ExportSalariesReport(
        [FromQuery] ReportPeriodType periodType,
        [FromQuery] ReportFormat format,
        [FromQuery] DateOnly? date,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] MonthlySummaryStatus? status,
        [FromQuery] Guid? employeeId)
    {
        var file = await Mediator.Send(new ExportSalariesReportQuery
        {
            PeriodType = periodType,
            Format = format,
            Date = date,
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            EmployeeId = employeeId
        });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("platform-reconciliation/generate")]
    public async Task<ActionResult<PlatformReconciliationReportDto>> GenerateReconciliationReport(
        [FromForm] int year,
        [FromForm] int month,
        [FromForm] Guid platformId,
        IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var result = await Mediator.Send(new GenerateReconciliationReportCommand
        {
            Content = memoryStream.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Year = year,
            Month = month,
            PlatformId = platformId
        });
        return Ok(result);
    }

    [HttpPost("platform-reconciliation/export")]
    public async Task<IActionResult> ExportReconciliationReport(
        [FromForm] int year,
        [FromForm] int month,
        [FromForm] Guid platformId,
        [FromForm] ReportFormat format,
        IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var result = await Mediator.Send(new ExportReconciliationReportCommand
        {
            Content = memoryStream.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Year = year,
            Month = month,
            PlatformId = platformId,
            Format = format
        });
        return File(result.Content, result.ContentType, result.FileName);
    }
}
