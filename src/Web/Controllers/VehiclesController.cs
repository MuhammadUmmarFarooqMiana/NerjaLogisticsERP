using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Vehicles.Commands.ActivateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAccidentRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAllocation;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleOilChangeRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleServiceRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleTyreReplacementRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAccidentRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAllocation;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleOilChangeRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleServiceRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleTyreReplacementRecord;
using NerjaLogisticsERP.Application.Vehicles.Queries;
using NerjaLogisticsERP.Application.Vehicles.Queries.GetMyVehicle;
using NerjaLogisticsERP.Application.Vehicles.Queries.GetVehicles;
using NerjaLogisticsERP.Application.Vehicles.Queries.GetVehiclesById;


namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiclesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<VehicleDto>>> GetAll(
        [FromQuery] bool? activeOnly, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetVehiclesQuery { ActiveOnly = activeOnly, PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetVehiclesByIdQuery { Id = id }));

    [HttpGet("me")]
    public async Task<ActionResult<VehicleDto?>> GetMine()
        => Ok(await Mediator.Send(new GetMyVehicleQuery()));

    [HttpGet("{id}/history")]
    public async Task<ActionResult<VehicleHistoryDto>> GetHistory(Guid id)
        => Ok(await Mediator.Send(new GetVehicleHistoryQuery { VehicleId = id }));

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Vehicle created.", id });
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await Mediator.Send(new ActivateVehicleCommand { Id = id });
        return Ok(new { message = "Vehicle activated.", id });
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await Mediator.Send(new DeactivateVehicleCommand { Id = id });
        return Ok(new { message = "Vehicle deactivated.", id });
    }

    [HttpPost("{id}/service")]
    public async Task<IActionResult> AddService(Guid id, AddVehicleServiceRecordCommand command)
    {
        if (id != command.VehicleId) return BadRequest();
        var recordId = await Mediator.Send(command);
        return Ok(new { message = "Service record added.", id = recordId });
    }

    [HttpPost("{id}/oil-change")]
    public async Task<IActionResult> AddOilChange(Guid id, AddVehicleOilChangeRecordCommand command)
    {
        if (id != command.VehicleId) return BadRequest();
        var recordId = await Mediator.Send(command);
        return Ok(new { message = "Oil change record added.", id = recordId });
    }

    [HttpPost("{id}/tyre-replacement")]
    public async Task<IActionResult> AddTyreReplacement(Guid id, AddVehicleTyreReplacementRecordCommand command)
    {
        if (id != command.VehicleId) return BadRequest();
        var recordId = await Mediator.Send(command);
        return Ok(new { message = "Tyre replacement record added.", id = recordId });
    }

    [HttpPost("{id}/accident")]
    public async Task<IActionResult> AddAccident(Guid id, AddVehicleAccidentRecordCommand command)
    {
        if (id != command.VehicleId) return BadRequest();
        var recordId = await Mediator.Send(command);
        return Ok(new { message = "Accident record added.", id = recordId });
    }

    [HttpPost("allocate")]
    public async Task<IActionResult> Allocate(AllocateVehicleCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Vehicle allocated.", id });
    }

    [HttpPost("return")]
    public async Task<IActionResult> Return(ReturnVehicleCommand command)
    {
        await Mediator.Send(command);
        return Ok(new { message = "Vehicle returned." });
    }

    [HttpPut("allocations/{id}")]
    public async Task<IActionResult> UpdateAllocation(Guid id, UpdateVehicleAllocationCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Allocation updated.", id });
    }

    [HttpDelete("allocations/{id}")]
    public async Task<IActionResult> DeleteAllocation(Guid id)
    {
        await Mediator.Send(new DeleteVehicleAllocationCommand { Id = id });
        return Ok(new { message = "Allocation deleted.", id });
    }

    [HttpPut("service/{id}")]
    public async Task<IActionResult> UpdateService(Guid id, UpdateVehicleServiceRecordCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Service record updated.", id });
    }

    [HttpDelete("service/{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        await Mediator.Send(new DeleteVehicleServiceRecordCommand { Id = id });
        return Ok(new { message = "Service record deleted.", id });
    }

    [HttpPut("oil-change/{id}")]
    public async Task<IActionResult> UpdateOilChange(Guid id, UpdateVehicleOilChangeRecordCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Oil change record updated.", id });
    }

    [HttpDelete("oil-change/{id}")]
    public async Task<IActionResult> DeleteOilChange(Guid id)
    {
        await Mediator.Send(new DeleteVehicleOilChangeRecordCommand { Id = id });
        return Ok(new { message = "Oil change record deleted.", id });
    }

    [HttpPut("tyre-replacement/{id}")]
    public async Task<IActionResult> UpdateTyreReplacement(Guid id, UpdateVehicleTyreReplacementRecordCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Tyre replacement record updated.", id });
    }

    [HttpDelete("tyre-replacement/{id}")]
    public async Task<IActionResult> DeleteTyreReplacement(Guid id)
    {
        await Mediator.Send(new DeleteVehicleTyreReplacementRecordCommand { Id = id });
        return Ok(new { message = "Tyre replacement record deleted.", id });
    }

    [HttpPut("accident/{id}")]
    public async Task<IActionResult> UpdateAccident(Guid id, UpdateVehicleAccidentRecordCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Accident record updated.", id });
    }

    [HttpDelete("accident/{id}")]
    public async Task<IActionResult> DeleteAccident(Guid id)
    {
        await Mediator.Send(new DeleteVehicleAccidentRecordCommand { Id = id });
        return Ok(new { message = "Accident record deleted.", id });
    }
}
