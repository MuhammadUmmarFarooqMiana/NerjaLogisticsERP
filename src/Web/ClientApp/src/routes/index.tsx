import { Navigate, Route, Routes } from 'react-router-dom';
import LoginPage from '../features/auth/LoginPage';
import RegisterPage from '../features/auth/RegisterPage';
import DailyOrdersPage from '../features/daily-orders/DailyOrdersPage';
import DashboardPage from '../features/dashboard/DashboardPage';
import FinesPage from '../features/fines/FinesPage';
import AdvancesPage from '../features/advances/AdvancesPage';
import ExpensesPage from '../features/expenses/ExpensesPage';
import SalaryFormulasPage from '../features/salary-formulas/SalaryFormulasPage';
import MonthlySummariesPage from '../features/monthly-summaries/MonthlySummariesPage';
import MonthlySummaryDetailPage from '../features/monthly-summaries/MonthlySummaryDetailPage';
import VehiclesPage from '../features/vehicles/VehiclesPage';
import VehicleDetailPage from '../features/vehicles/VehicleDetailPage';
import InventoryPage from '../features/inventory/InventoryPage';
import SuppliersPage from '../features/suppliers/SuppliersPage';
import MechanicsPage from '../features/mechanics/MechanicsPage';
import EmployeeListPage from '../features/employees/EmployeeListPage';
import PendingApprovalsPage from '../features/employees/PendingApprovalsPage';
import EmployeeDetailPage from '../features/employees/EmployeeDetailPage';
import MyProfilePage from '../features/employees/MyProfilePage';
import LeaveRequestsPage from '../features/leave-requests/LeaveRequestsPage';
import CompanyDocumentsPage from '../features/documents/CompanyDocumentsPage';
import ReportsHomePage from '../features/reports/ReportsHomePage';
import OrdersReportPage from '../features/reports/OrdersReportPage';
import FinesReportPage from '../features/reports/FinesReportPage';
import AdvancesReportPage from '../features/reports/AdvancesReportPage';
import ExpensesReportPage from '../features/reports/ExpensesReportPage';
import LeavesReportPage from '../features/reports/LeavesReportPage';
import VehiclesReportPage from '../features/reports/VehiclesReportPage';
import SuppliersReportPage from '../features/reports/SuppliersReportPage';
import InventoryLedgerReportPage from '../features/reports/InventoryLedgerReportPage';
import SalariesReportPage from '../features/reports/SalariesReportPage';
import AuditInvoicePage from '../features/audit-invoice/AuditInvoicePage';
import AppShell from '../components/layout/AppShell';
import ProtectedRoute from './ProtectedRoute';
import RequireActiveProfile from './RequireActiveProfile';
import RoleGate from './RoleGate';
import { Roles } from '../lib/roles';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<AppShell />}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="profile" element={<MyProfilePage />} />

          <Route element={<RequireActiveProfile />}>
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="leave-requests" element={<LeaveRequestsPage />} />

            <Route
              element={<RoleGate roles={[Roles.Administrator, Roles.Supervisor, Roles.Rider]} />}
            >
              <Route path="daily-orders" element={<DailyOrdersPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Supervisor]} />}>
              <Route path="employees" element={<EmployeeListPage />} />
              <Route path="employees/pending" element={<PendingApprovalsPage />} />
              <Route path="employees/:id" element={<EmployeeDetailPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant, Roles.Rider]} />}>
              <Route path="fines" element={<FinesPage />} />
              <Route path="advances" element={<AdvancesPage />} />
              <Route path="monthly-summaries" element={<MonthlySummariesPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant]} />}>
              <Route path="expenses" element={<ExpensesPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator]} />}>
              <Route path="salary-formulas" element={<SalaryFormulasPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Supervisor, Roles.Accountant]} />}>
              <Route path="company-documents" element={<CompanyDocumentsPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant, Roles.Rider]} />}>
              <Route path="vehicles" element={<VehiclesPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant]} />}>
              <Route path="vehicles/:id" element={<VehicleDetailPage />} />
              <Route path="inventory" element={<InventoryPage />} />
              <Route path="suppliers" element={<SuppliersPage />} />
              <Route path="mechanics" element={<MechanicsPage />} />
              <Route path="monthly-summaries/:id" element={<MonthlySummaryDetailPage />} />
            </Route>

            {/* Reports home is shared by every role that can reach at least one report type —
                individual report routes below are gated to match that report's own backend
                authorization, and ReportsHomePage filters its cards by role to match. */}
            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Supervisor, Roles.Accountant]} />}>
              <Route path="reports" element={<ReportsHomePage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Supervisor]} />}>
              <Route path="reports/orders" element={<OrdersReportPage />} />
              <Route path="reports/leaves" element={<LeavesReportPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant]} />}>
              <Route path="reports/fines" element={<FinesReportPage />} />
              <Route path="reports/advances" element={<AdvancesReportPage />} />
              <Route path="reports/expenses" element={<ExpensesReportPage />} />
              <Route path="reports/vehicles" element={<VehiclesReportPage />} />
              <Route path="reports/suppliers" element={<SuppliersReportPage />} />
              <Route path="reports/inventory-ledger" element={<InventoryLedgerReportPage />} />
              <Route path="reports/salaries" element={<SalariesReportPage />} />
            </Route>

            <Route element={<RoleGate roles={[Roles.Administrator, Roles.Accountant]} />}>
              <Route path="audit-invoice" element={<AuditInvoicePage />} />
            </Route>
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
