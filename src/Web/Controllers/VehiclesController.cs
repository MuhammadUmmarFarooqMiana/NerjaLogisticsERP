using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Vehicles.Commands.ActivateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;
using NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;
using NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;
using NerjaLogisticsERP.Application.Vehicles.Queries;
using NerjaLogisticsERP.Application.Vehicles.Queries.GetVehicles;
using NerjaLogisticsERP.Application.Vehicles.Queries.GetVehiclesById;


namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiclesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<VehicleDto>>> GetAll([FromQuery] bool? activeOnly)
        => Ok(await Mediator.Send(new GetVehiclesQuery { ActiveOnly = activeOnly }));

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetVehiclesByIdQuery { Id = id }));

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
}
