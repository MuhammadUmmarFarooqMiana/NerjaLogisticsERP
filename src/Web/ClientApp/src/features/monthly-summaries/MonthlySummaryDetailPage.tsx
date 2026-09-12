import { useNavigate, useParams } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Alert, Box, Button, CircularProgress, Grid, Paper, Stack, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useGetApiMonthlySummariesByIdQuery } from '../../api/monthlySummariesApi';
import { DetailField } from '../../components/shared/DetailField';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { formatDateTime } from '../../lib/formatDate';

// This theme deliberately remaps palette.error.main to the brand orange (see theme.ts),
// so a literal red is used for deduction amounts instead — 'error.main' wouldn't render as red.
const DEDUCTION_COLOR = '#D32F2F';

export default function MonthlySummaryDetailPage() {
  const { t, i18n } = useTranslation('monthlySummaries');
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: summary, isLoading, error } = useGetApiMonthlySummariesByIdQuery({ id: id ?? '' }, { skip: !id });

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !summary) {
    return <Alert severity="error">{t('detail.genericError')}</Alert>;
  }

  const money = (value: unknown) => `SAR ${Number(value).toFixed(2)}`;
  const period = new Date(Number(summary.year), Number(summary.month) - 1, 1).toLocaleDateString(i18n.language, {
    year: 'numeric',
    month: 'long',
  });

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Box>
          <Typography variant="h4">{summary.employeeName}</Typography>
          <Typography variant="body2" color="text.secondary">
            {t('detail.fields.period')}: {period}
          </Typography>
        </Box>
        <StatusBadge domain="monthlySummaryStatus" status={summary.status ?? ''} />
      </Stack>

      <Paper variant="outlined" sx={{ p: 3, mb: 2 }}>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.orders')} value={String(summary.totalCompletedOrders ?? '')} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.generatedOn')} value={formatDateTime(summary.created, i18n.language)} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }} />
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.totalSalary')}>
              <Typography variant="body1" sx={{ color: 'success.main', fontWeight: 600 }}>
                {money(summary.totalSalary)}
              </Typography>
            </DetailField>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.totalAdvances')}>
              <Typography variant="body1" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
                {money(summary.totalAdvances)}
              </Typography>
            </DetailField>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.totalFines')}>
              <Typography variant="body1" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
                {money(summary.totalFines)}
              </Typography>
            </DetailField>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <DetailField label={t('detail.fields.netPayable')}>
              <Typography variant="h6">{money(summary.netSalaryPayable)}</Typography>
            </DetailField>
          </Grid>
        </Grid>
      </Paper>

      {summary.verifiedAt && (
        <Paper variant="outlined" sx={{ p: 3, mb: 2 }}>
          <Typography variant="h6" gutterBottom>
            {t('detail.verification.title')}
          </Typography>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <DetailField label={t('detail.verification.verifiedBy')} value={summary.verifiedByName} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <DetailField
                label={t('detail.verification.verifiedAt')}
                value={formatDateTime(summary.verifiedAt, i18n.language)}
              />
            </Grid>
          </Grid>
        </Paper>
      )}

      {summary.paidAt && (
        <Paper variant="outlined" sx={{ p: 3, mb: 2, borderColor: 'success.main' }}>
          <Typography variant="h6" gutterBottom>
            {t('detail.payment.title')}
          </Typography>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <DetailField label={t('detail.payment.paidBy')} value={summary.paidByName} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <DetailField label={t('detail.payment.paidAt')} value={formatDateTime(summary.paidAt, i18n.language)} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <DetailField label={t('detail.payment.paymentReference')}>
                <Box
                  sx={{
                    display: 'inline-block',
                    bgcolor: 'action.hover',
                    borderRadius: 1,
                    px: 1.5,
                    py: 0.5,
                    mt: 0.5,
                  }}
                >
                  <Typography variant="h6" sx={{ fontFamily: 'monospace', wordBreak: 'break-all' }}>
                    {summary.paymentReference || t('detail.notProvided')}
                  </Typography>
                </Box>
              </DetailField>
            </Grid>
          </Grid>
        </Paper>
      )}

      <Button variant="text" startIcon={<ArrowBackIcon />} onClick={() => navigate(-1)}>
        {t('detail.backToList')}
      </Button>
    </Box>
  );
}
