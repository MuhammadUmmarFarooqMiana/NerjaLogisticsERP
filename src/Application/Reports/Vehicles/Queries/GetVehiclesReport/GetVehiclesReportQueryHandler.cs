using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Vehicles.Queries.GetVehiclesReport;

public class GetVehiclesReportQueryHandler : IRequestHandler<GetVehiclesReportQuery, VehiclesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetVehiclesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<VehiclesReportDto> Handle(GetVehiclesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        bool InRange(DateOnly d) => d >= period.Start && d <= period.End;

        var allocationsQuery = _context.VehicleAllocationHistories.Include(a => a.Vehicle).Include(a => a.Employee)
            .Where(a => a.AssignedDate >= period.Start && a.AssignedDate <= period.End
                     || (a.ReturnedDate != null && a.ReturnedDate >= period.Start && a.ReturnedDate <= period.End));
        var serviceQuery = _context.VehicleServiceHistories.Include(s => s.Vehicle)
            .Where(s => s.ServiceDate >= period.Start && s.ServiceDate <= period.End);
        var oilChangeQuery = _context.VehicleOilChangeHistories.Include(o => o.Vehicle)
            .Where(o => o.ChangeDate >= period.Start && o.ChangeDate <= period.End);
        var tyreQuery = _context.VehicleTyreReplacementHistories.Include(t => t.Vehicle)
            .Where(t => t.ReplacementDate >= period.Start && t.ReplacementDate <= period.End);
        var accidentQuery = _context.VehicleAccidentHistories.Include(a => a.Vehicle)
            .Where(a => a.AccidentDate >= period.Start && a.AccidentDate <= period.End);

        if (request.VehicleId.HasValue)
        {
            allocationsQuery = allocationsQuery.Where(a => a.VehicleId == request.VehicleId);
            serviceQuery = serviceQuery.Where(s => s.VehicleId == request.VehicleId);
            oilChangeQuery = oilChangeQuery.Where(o => o.VehicleId == request.VehicleId);
            tyreQuery = tyreQuery.Where(t => t.VehicleId == request.VehicleId);
            accidentQuery = accidentQuery.Where(a => a.VehicleId == request.VehicleId);
        }

        var allocations = await allocationsQuery.ToListAsync(cancellationToken);
        var services = await serviceQuery.ToListAsync(cancellationToken);
        var oilChanges = await oilChangeQuery.ToListAsync(cancellationToken);
        var tyreReplacements = await tyreQuery.ToListAsync(cancellationToken);
        var accidents = await accidentQuery.ToListAsync(cancellationToken);

        var rows = new List<VehiclesReportRowDto>();

        foreach (var a in allocations)
        {
            if (InRange(a.AssignedDate))
            {
                rows.Add(new VehiclesReportRowDto
                {
                    VehicleRegistration = a.Vehicle.RegistrationNumber,
                    RecordType = "Allocation",
                    Date = a.AssignedDate,
                    Description = $"Assigned to {a.Employee.FullName}",
                    Cost = null
                });
            }
            if (a.ReturnedDate.HasValue && InRange(a.ReturnedDate.Value))
            {
                rows.Add(new VehiclesReportRowDto
                {
                    VehicleRegistration = a.Vehicle.RegistrationNumber,
                    RecordType = "Return",
                    Date = a.ReturnedDate.Value,
                    Description = $"Returned by {a.Employee.FullName}",
                    Cost = null
                });
            }
        }

        rows.AddRange(services.Select(s => new VehiclesReportRowDto
        {
            VehicleRegistration = s.Vehicle.RegistrationNumber,
            RecordType = "Service",
            Date = s.ServiceDate,
            Description = s.Description,
            Cost = s.Cost
        }));

        rows.AddRange(oilChanges.Select(o => new VehiclesReportRowDto
        {
            VehicleRegistration = o.Vehicle.RegistrationNumber,
            RecordType = "OilChange",
            Date = o.ChangeDate,
            Description = $"Odometer {o.Odometer}",
            Cost = o.Cost
        }));

        rows.AddRange(tyreReplacements.Select(t => new VehiclesReportRowDto
        {
            VehicleRegistration = t.Vehicle.RegistrationNumber,
            RecordType = "TyreReplacement",
            Date = t.ReplacementDate,
            Description = $"{t.NumberOfTyres} tyre(s)",
            Cost = t.Cost
        }));

        rows.AddRange(accidents.Select(a => new VehiclesReportRowDto
        {
            VehicleRegistration = a.Vehicle.RegistrationNumber,
            RecordType = "Accident",
            Date = a.AccidentDate,
            Description = a.Description,
            Cost = a.RepairCost
        }));

        rows = rows.OrderBy(r => r.Date).ThenBy(r => r.VehicleRegistration).ToList();

        var vehiclesQuery = _context.Vehicles.AsQueryable();
        if (request.VehicleId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(v => v.Id == request.VehicleId);

        var totalVehicles = await vehiclesQuery.CountAsync(cancellationToken);
        var activeVehicles = await vehiclesQuery.CountAsync(v => v.IsActive, cancellationToken);

        return new VehiclesReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalVehicles = totalVehicles,
            ActiveVehicles = activeVehicles,
            TotalMaintenanceCost = rows.Sum(r => r.Cost ?? 0),
            Rows = rows
        };
    }
}
