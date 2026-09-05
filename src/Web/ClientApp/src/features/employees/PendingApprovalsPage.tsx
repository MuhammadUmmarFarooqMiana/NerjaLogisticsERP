import { useTranslation } from 'react-i18next';
import { EmployeeListView } from './EmployeeListView';

export default function PendingApprovalsPage() {
  const { t } = useTranslation('employees');
  return <EmployeeListView title={t('pending.title')} fixedStatus="PendingReview" emptyMessage={t('pending.empty')} />;
}
