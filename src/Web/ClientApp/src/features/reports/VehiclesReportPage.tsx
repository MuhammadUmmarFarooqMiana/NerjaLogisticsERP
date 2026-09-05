import { useState } from 'react';
import { Alert, Box, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiVehiclesQuery } from '../../api/vehiclesApi';
import type { VehiclesReportRowDto } from '../../api/generated/apiSlice';
import { useGetApiReportsVehiclesQuery, vehiclesReportExportUrl } from '../../api/reportsApi';
import { DataTable } from '../../components/shared/DataTable';
import { formatDate } from '../../lib/formatDate';
import { CURRENT_MONTH, CURRENT_YEAR, TODAY } from './reportConstants';
import { ReportActionButtons } from './ReportActionButtons';
import { ReportPeriodFilters } from './ReportPeriodFilters';

const CUSTOM = 3;

function SummaryTile({ label, value }: { label: string; value: string }) {
  return (
    <Grid size={{ xs: 12, sm: 4 }}>
      <Paper variant="outlined" sx={{ p: 2.5 }}>
        <Typography variant="body2" color="text.secondary" noWrap>
          {label}
        </Typography>
        <Typography variant="h5" sx={{ fontWeight: 700, mt: 0.5 }}>
          {value}
        </Typography>
      </Paper>
    </Grid>
  );
}

export default function VehiclesReportPage() {
  const { t, i18n } = useTranslation('reports');

  const [periodType, setPeriodType] = useState<number>(0);
  const [date, setDate] = useState(TODAY);
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [vehicleId, setVehicleId] = useState('');

  const { data: vehicles } = useGetApiVehiclesQuery({});

  const isCustom = periodType === CUSTOM;
  const customReady = !isCustom || (!!startDate && !!endDate);

  const periodArgs = {
    periodType,
    date: periodType === 0 || periodType === 1 ? date : undefined,
    year: periodType === 2 ? year : undefined,
    month: periodType === 2 ? month : undefined,
    startDate: isCustom ? startDate : undefined,
    endDate: isCustom ? endDate : undefined,
    vehicleId: vehicleId || undefined,
  };

  const { data: report, isLoading, error } = useGetApiReportsVehiclesQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<VehiclesReportRowDto, unknown>[] = [
    { accessorKey: 'vehicleRegistration', header: t('vehicles.columns.vehicle') },
    { accessorKey: 'recordType', header: t('vehicles.columns.recordType') },
    {
      accessorKey: 'date',
      header: t('columns.orderDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'description',
      header: t('columns.description'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'cost',
      header: t('vehicles.columns.cost'),
      cell: (info) => {
        const value = info.getValue() as number | null;
        return value != null ? `SAR ${Number(value).toFixed(2)}` : '—';
      },
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('vehicles.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('vehicles.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => vehiclesReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="vehicles-report"
        />
      </Stack>

      <Stack direction="row" spacing={2} sx={{ my: 2, flexWrap: 'wrap' }}>
        <ReportPeriodFilters
          periodType={periodType}
          onPeriodTypeChange={setPeriodType}
          date={date}
          onDateChange={setDate}
          year={year}
          onYearChange={setYear}
          month={month}
          onMonthChange={setMonth}
          startDate={startDate}
          onStartDateChange={setStartDate}
          endDate={endDate}
          onEndDateChange={setEndDate}
        />

        <TextField
          select
          size="small"
          label={t('vehicles.filters.vehicle')}
          value={vehicleId}
          onChange={(event) => setVehicleId(event.target.value)}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('vehicles.filters.allVehicles')}</MenuItem>
          {(vehicles ?? []).map((vehicle) => (
            <MenuItem key={vehicle.id} value={vehicle.id}>
              {vehicle.registrationNumber}
            </MenuItem>
          ))}
        </TextField>
      </Stack>

      {isCustom && !customReady && <Alert severity="info" sx={{ mb: 2 }}>{t('customDateHint')}</Alert>}

      {report && (
        <>
          <Typography variant="subtitle1" sx={{ mb: 2 }}>
            {report.periodLabel}
          </Typography>
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <SummaryTile label={t('vehicles.summary.totalVehicles')} value={String(report.totalVehicles)} />
            <SummaryTile label={t('vehicles.summary.activeVehicles')} value={String(report.activeVehicles)} />
            <SummaryTile
              label={t('vehicles.summary.totalMaintenanceCost')}
              value={`SAR ${Number(report.totalMaintenanceCost).toFixed(2)}`}
            />
          </Grid>
        </>
      )}

      <DataTable
        columns={columns}
        data={report?.rows ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
      />
    </Box>
  );
}
