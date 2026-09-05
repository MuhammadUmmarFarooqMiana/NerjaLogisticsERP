import { Grid } from '@mui/material';
import { useTranslation } from 'react-i18next';
import type { EmployeeDetailDto } from '../../api/generated/apiSlice';
import { DetailField } from '../../components/shared/DetailField';
import { PerformanceIndicator } from '../../components/shared/PerformanceIndicator';
import { formatDate, formatDateTime } from '../../lib/formatDate';

interface EmployeeProfileFieldsProps {
  employee: EmployeeDetailDto;
}

// Shared read-only field grid: used by the admin/supervisor EmployeeDetailPage
// and the rider's own MyProfilePage, so the two views never drift apart.
export function EmployeeProfileFields({ employee }: EmployeeProfileFieldsProps) {
  const { t, i18n } = useTranslation('employees');
  const yesNo = (value: boolean) => (value ? t('detail.yes') : t('detail.no'));

  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.email')} value={employee.email} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.phoneNumber')} value={employee.phoneNumber} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.hasWhatsApp')} value={yesNo(!!employee.hasWhatsApp)} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.emailConfirmed')} value={yesNo(!!employee.emailConfirmed)} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.performanceStatus')}>
          <PerformanceIndicator status={employee.performanceStatus} />
        </DetailField>
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.platform')} value={employee.platformName} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.vehicle')} value={employee.vehicleRegistrationNumber} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.supervisor')} value={employee.supervisorName} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.iqamaNumber')} value={employee.iqamaNumber} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.platformIdNumber')} value={employee.platformIdNumber} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.joiningDate')} value={formatDate(employee.joiningDate, i18n.language)} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField label={t('detail.fields.idExpiryDate')} value={formatDate(employee.idExpiryDate, i18n.language)} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField
          label={t('detail.fields.iqamaExpiryDate')}
          value={formatDate(employee.iqamaExpiryDate, i18n.language)}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField
          label={t('detail.fields.drivingLicenseExpiryDate')}
          value={formatDate(employee.drivingLicenseExpiryDate, i18n.language)}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField
          label={t('detail.fields.insuranceExpiryDate')}
          value={formatDate(employee.insuranceExpiryDate, i18n.language)}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <DetailField
          label={t('detail.fields.profileSubmittedAt')}
          value={formatDateTime(employee.profileSubmittedAt, i18n.language)}
        />
      </Grid>
      {employee.accountStatus === 'Rejected' && employee.rejectionReason && (
        <Grid size={12}>
          <DetailField label={t('detail.fields.rejectionReason')} value={employee.rejectionReason} />
        </Grid>
      )}
    </Grid>
  );
}
