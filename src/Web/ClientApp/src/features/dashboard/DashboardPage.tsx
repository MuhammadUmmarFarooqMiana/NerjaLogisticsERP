import { useState } from 'react';
import BeachAccessOutlinedIcon from '@mui/icons-material/BeachAccessOutlined';
import DoneAllOutlinedIcon from '@mui/icons-material/DoneAllOutlined';
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import HowToRegOutlinedIcon from '@mui/icons-material/HowToRegOutlined';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import PaymentsOutlinedIcon from '@mui/icons-material/PaymentsOutlined';
import PendingActionsIcon from '@mui/icons-material/PendingActions';
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined';
import TodayOutlinedIcon from '@mui/icons-material/TodayOutlined';
import WalletOutlinedIcon from '@mui/icons-material/AccountBalanceWalletOutlined';
import { Box, Grid } from '@mui/material';
import type { ComponentType } from 'react';
import type { SvgIconProps } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useGetApiAdvancesMeQuery, useGetApiAdvancesQuery } from '../../api/advancesApi';
import { useGetApiDailyOrdersHistoryQuery } from '../../api/dailyOrdersApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useGetApiExpensesQuery } from '../../api/expensesApi';
import { useGetApiFinesMeQuery, useGetApiFinesQuery } from '../../api/finesApi';
import { useGetApiInventoryLedgerQuery } from '../../api/inventoryApi';
import { useGetApiMonthlySummariesMeQuery, useGetApiMonthlySummariesQuery } from '../../api/monthlySummariesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { StatCard } from '../../components/dashboard/StatCard';
import { EmployeeStatusChart } from '../../components/dashboard/EmployeeStatusChart';
import { MonthlyAmountTrendChart } from '../../components/dashboard/MonthlyAmountTrendChart';
import { FinancialOverviewChart, type FinancialBar } from '../../components/dashboard/FinancialOverviewChart';
import { OrdersTrendChart } from '../../components/dashboard/OrdersTrendChart';
import { ALL_PLATFORMS_VALUE, PlatformFilter } from '../../components/dashboard/PlatformFilter';
import { RidersByPlatformCard } from '../../components/dashboard/RidersByPlatformCard';
import { BRAND_ORANGE } from '../../components/theme/theme';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import {
    buildEmployeeStatusBreakdown,
    buildExpensesByMonth,
    buildOrdersTrend,
    buildRolesByPlatform,
    buildSalaryPaidByMonth,
    sumCompletedOrders,
} from './aggregate';
import { CURRENT_MONTH, CURRENT_YEAR, currentMonthRange, lastNDaysRange, lastNMonthsRange, todayRange } from './dateRanges';

const TREND_DAYS = 7;
const TREND_MONTHS = 6;

interface StatDef {
    labelKey: string;
    value: string;
    icon: ComponentType<SvgIconProps>;
    color: string;
    href?: string;
}

function StatsGrid({ stats, namespace }: { stats: StatDef[]; namespace: 'stats' | 'myStats' }) {
    const { t } = useTranslation();
    return (
        <Grid container spacing={3} sx={{ mt: 1 }}>
            {stats.map((stat) => (
                <Grid key={stat.labelKey} size={{ xs: 12, sm: 6, md: 3 }}>
                    <StatCard
                        icon={stat.icon}
                        label={t(`dashboard.${namespace}.${stat.labelKey}`)}
                        value={stat.value}
                        color={stat.color}
                        href={stat.href}
                    />
                </Grid>
            ))}
        </Grid>
    );
}

export default function DashboardPage() {
    const { t, i18n } = useTranslation('common');
    const user = useAppSelector(selectCurrentUser);
    const isFleetStaff = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Supervisor);
    const isAdministrator = !!user?.roles.some((role) => role === Roles.Administrator);
    const isFinance = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Accountant);

    const [selectedPlatformId, setSelectedPlatformId] = useState<string>(ALL_PLATFORMS_VALUE);
    const { data: platforms = [] } = useGetApiPlatformsQuery(undefined, { skip: !isFleetStaff });
    const selectedPlatform = platforms.find((p) => p.id === selectedPlatformId);

    // GetDailyOrderHistoryQuery self-scopes on the backend (Admin sees the whole
    // fleet, Supervisor sees their reports, everyone else sees only their own),
    // so the same two queries power both the fleet-wide and personal views.
    const trendRange = lastNDaysRange(TREND_DAYS);
    const { data: trendOrders = [], isLoading: trendLoading } = useGetApiDailyOrdersHistoryQuery(trendRange);
    const monthRange = currentMonthRange();
    const { data: monthOrders = [] } = useGetApiDailyOrdersHistoryQuery(monthRange);
    const { startDate: today } = todayRange();

    const { data: employees = [] } = useGetApiEmployeesQuery({}, { skip: !isFleetStaff });
    const { data: advances = [] } = useGetApiAdvancesQuery(monthRange, { skip: !isAdministrator });
    const { data: fines = [] } = useGetApiFinesQuery(monthRange, { skip: !isAdministrator });
    const { data: expenses = [] } = useGetApiExpensesQuery(monthRange, { skip: !isFinance });
    // Inventory isn't platform-scoped in this app's model (items/stock are
    // company-wide, not tied to a Platform), so unlike Expenses this total is
    // never narrowed by the platform filter.
    const { data: stockLedger = [] } = useGetApiInventoryLedgerQuery(
        { fromDate: monthRange.startDate, toDate: monthRange.endDate },
        { skip: !isFinance }
    );

    const expenseTrendRange = lastNMonthsRange(TREND_MONTHS);
    const { data: expenseTrendData = [], isLoading: expenseTrendLoading } = useGetApiExpensesQuery(
        expenseTrendRange,
        { skip: !isFinance }
    );

    // Current month's summaries (every status) split into Paid vs. not-yet-Paid below;
    // Paid-only, unfiltered by month, powers the trailing-6-months trend chart —
    // MonthlySummaries has no date-range filter (only exact Year/Month), so unlike
    // Expenses' trend this can't be narrowed to just the window it needs.
    const { data: currentMonthSalarySummaries = [] } = useGetApiMonthlySummariesQuery(
        { year: CURRENT_YEAR, month: CURRENT_MONTH },
        { skip: !isFinance }
    );
    const { data: paidSalarySummaries = [], isLoading: salaryTrendLoading } = useGetApiMonthlySummariesQuery(
        { status: 2 }, // MonthlySummaryStatus.Paid
        { skip: !isFinance }
    );

    // Platform-scoped views — "All Platforms" is a no-op filter, otherwise every
    // employee/order/advance/fine is narrowed down to riders on the selected platform.
    const platformEmployees =
        selectedPlatformId === ALL_PLATFORMS_VALUE
            ? employees
            : employees.filter((e) => e.platformName === selectedPlatform?.name);
    const platformEmployeeIds = new Set(platformEmployees.map((e) => e.id));
    const filterByPlatform = <T extends { employeeId?: string }>(items: T[]): T[] =>
        selectedPlatformId === ALL_PLATFORMS_VALUE
            ? items
            : items.filter((item) => item.employeeId && platformEmployeeIds.has(item.employeeId));

    const scopedMonthOrders = filterByPlatform(monthOrders);
    const scopedTrendOrders = filterByPlatform(trendOrders);
    const scopedAdvances = filterByPlatform(advances);
    const scopedFines = filterByPlatform(fines);
    // Expense records carry their own PlatformId directly (a company expense
    // belongs to a platform, not an employee), so they're scoped straight off
    // that field instead of going through filterByPlatform's employeeId lookup.
    const scopedExpenses =
        selectedPlatformId === ALL_PLATFORMS_VALUE ? expenses : expenses.filter((e) => e.platformId === selectedPlatformId);

    // Despite the historical name, this counts every active employee regardless of
    // role — "Total Active Employees" per the card's label. activeRiders below is
    // the one actually scoped to the Rider role.
    const totalRiders = platformEmployees.filter((e) => e.accountStatus === 'Active').length;
    const activeRiders = platformEmployees.filter(
        (e) => e.accountStatus === 'Active' && (e.roles ?? []).includes(Roles.Rider)
    ).length;
    const todaysOrders = sumCompletedOrders(scopedMonthOrders.filter((o) => o.orderDate === today));
    const monthlyCompletedOrders = sumCompletedOrders(scopedMonthOrders);
    const scopedAdvancesTotal = scopedAdvances.reduce((sum, a) => sum + Number(a.amount ?? 0), 0);
    const scopedFinesTotal = scopedFines.reduce((sum, f) => sum + Number(f.amount ?? 0), 0);
    const scopedExpensesTotal = scopedExpenses.reduce((sum, e) => sum + Number(e.amount ?? 0), 0);
    const stockOutQuantity = stockLedger
        .filter((entry) => entry.type === 'Out')
        .reduce((sum, entry) => sum + Number(entry.quantity ?? 0), 0);
    const scopedMonthSalarySummaries = filterByPlatform(currentMonthSalarySummaries);
    const scopedPaidSalaryTotal = scopedMonthSalarySummaries
        .filter((s) => s.status === 'Paid')
        .reduce((sum, s) => sum + Number(s.netSalaryPayable ?? 0), 0);
    const scopedSalaryToPayTotal = scopedMonthSalarySummaries
        .filter((s) => s.status !== 'Paid')
        .reduce((sum, s) => sum + Number(s.netSalaryPayable ?? 0), 0);

    // Financial Overview chart stays fleet-wide regardless of the platform filter.
    const totalAdvances = advances.reduce((sum, a) => sum + Number(a.amount ?? 0), 0);
    const totalFines = fines.reduce((sum, f) => sum + Number(f.amount ?? 0), 0);
    const totalExpenses = expenses.reduce((sum, e) => sum + Number(e.amount ?? 0), 0);

    const FLEET_STATS: StatDef[] = [
        { labelKey: 'todaysCompletedOrders', value: String(todaysOrders), icon: TodayOutlinedIcon, color: BRAND_ORANGE, href: '/daily-orders' },
        { labelKey: 'monthlyCompletedOrders', value: String(monthlyCompletedOrders), icon: DoneAllOutlinedIcon, color: '#5FA8D3', href: '/daily-orders' },

        {
            labelKey: 'totalAdvances',
            value: isAdministrator ? `SAR ${scopedAdvancesTotal.toLocaleString()}` : '—',
            icon: WalletOutlinedIcon,
            color: '#0288D1',
            href: isAdministrator ? '/advances' : undefined,
        },
        {
            labelKey: 'totalFines',
            value: isAdministrator ? `SAR ${scopedFinesTotal.toLocaleString()}` : '—',
            icon: GavelOutlinedIcon,
            color: '#D32F2F',
            href: isAdministrator ? '/fines' : undefined,
        },
        // Fleet staff who are also finance-facing (i.e. Administrator) get this
        // folded into the same grid as the rest of the fleet stats, instead of a
        // separate row, so it fills the trailing gap rather than leaving one.
        ...(isFinance
            ? [
                {
                    labelKey: 'totalExpenses',
                    value: `SAR ${scopedExpensesTotal.toLocaleString()}`,
                    icon: ReceiptLongOutlinedIcon,
                    color: '#7C5CFC',
                    href: '/expenses',
                },
                {
                    labelKey: 'stockOut',
                    value: stockOutQuantity.toLocaleString(),
                    icon: Inventory2OutlinedIcon,
                    color: '#C2185B',
                    href: '/reports/inventory-ledger',
                },
                {
                    labelKey: 'totalPaidSalary',
                    value: `SAR ${scopedPaidSalaryTotal.toLocaleString()}`,
                    icon: PaymentsOutlinedIcon,
                    color: '#2E7D32',
                    href: '/monthly-summaries',
                },
                {
                    labelKey: 'totalSalaryToPay',
                    value: `SAR ${scopedSalaryToPayTotal.toLocaleString()}`,
                    icon: PendingActionsIcon,
                    color: '#F9A825',
                    href: '/monthly-summaries',
                },
                { labelKey: 'totalRiders', value: String(totalRiders), icon: GroupsOutlinedIcon, color: '#1B4965', href: '/employees' },
                { labelKey: 'activeRiders', value: String(activeRiders), icon: HowToRegOutlinedIcon, color: '#2E7D32', href: '/employees' },
            ]
            : []),
    ];

    // Personal figures shown to any employee who isn't fleet-wide staff — orders,
    // salary, advances, and fines are all real self-scoped queries. Leave balance
    // has no accrual/entitlement tracking in the domain yet, so it stays a link
    // into Leave Requests rather than a fabricated number.
    const { data: myAdvances = [] } = useGetApiAdvancesMeQuery(monthRange, { skip: isFleetStaff });
    const { data: myFines = [] } = useGetApiFinesMeQuery(monthRange, { skip: isFleetStaff });
    const { data: myLatestSummary = [] } = useGetApiMonthlySummariesMeQuery(
        { pageNumber: 1, pageSize: 1 },
        { skip: isFleetStaff }
    );
    const myAdvancesTotal = myAdvances.reduce((sum, a) => sum + Number(a.amount ?? 0), 0);
    const myFinesTotal = myFines.reduce((sum, f) => sum + Number(f.amount ?? 0), 0);
    const myLatestNetSalary = myLatestSummary[0]?.netSalaryPayable;

    const MY_STATS: StatDef[] = [
        {
            labelKey: 'salary',
            value: myLatestNetSalary != null ? `SAR ${Number(myLatestNetSalary).toLocaleString()}` : '—',
            icon: PaymentsOutlinedIcon,
            color: '#1B4965',
            href: '/monthly-summaries',
        },
        { labelKey: 'advances', value: `SAR ${myAdvancesTotal.toLocaleString()}`, icon: WalletOutlinedIcon, color: '#0288D1', href: '/advances' },
        { labelKey: 'fines', value: `SAR ${myFinesTotal.toLocaleString()}`, icon: GavelOutlinedIcon, color: '#D32F2F', href: '/fines' },
        { labelKey: 'leaves', value: '—', icon: BeachAccessOutlinedIcon, color: '#2E7D32', href: '/leave-requests' },
        { labelKey: 'ordersThisMonth', value: String(sumCompletedOrders(monthOrders)), icon: LocalShippingOutlinedIcon, color: BRAND_ORANGE, href: '/daily-orders' },
    ];

    const trendData = buildOrdersTrend(isFleetStaff ? scopedTrendOrders : trendOrders, TREND_DAYS, i18n.language);
    const statusData = buildEmployeeStatusBreakdown(employees);
    const rolesByPlatform = buildRolesByPlatform(employees, platforms, t('dashboard.unassignedPlatform'));
    const expenseMonthData = buildExpensesByMonth(expenseTrendData, TREND_MONTHS, i18n.language);
    const salaryPaidMonthData = buildSalaryPaidByMonth(paidSalarySummaries, TREND_MONTHS, i18n.language);
    const financialData: FinancialBar[] = [
        { key: 'fines', label: t('dashboard.charts.fines'), amount: totalFines, color: '#D32F2F' },
        { key: 'advances', label: t('dashboard.charts.advances'), amount: totalAdvances, color: '#0288D1' },
        { key: 'expenses', label: t('dashboard.charts.expenses'), amount: totalExpenses, color: '#7C5CFC' },
    ];

    return (
        <Box>
            {isFleetStaff && (
                <Box sx={{ mt: 2 }}>
                    <PlatformFilter platforms={platforms} selected={selectedPlatformId} onChange={setSelectedPlatformId} />
                </Box>
            )}

            {isFleetStaff ? (
                <StatsGrid stats={FLEET_STATS} namespace="stats" />
            ) : (
                <StatsGrid stats={MY_STATS} namespace="myStats" />
            )}

            {/* Fleet staff (Administrator) get this stat folded into FLEET_STATS above so it
          fills the grid's trailing gap instead of starting a new, mostly-empty row.
          Accountant has no fleet grid to join, so it gets its own row here. */}
            {isFinance && !isFleetStaff && (
                <Grid container spacing={3} sx={{ mt: 3 }}>
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <StatCard
                            icon={ReceiptLongOutlinedIcon}
                            label={t('dashboard.stats.totalExpenses')}
                            value={`SAR ${scopedExpensesTotal.toLocaleString()}`}
                            color="#7C5CFC"
                            href="/expenses"
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <StatCard
                            icon={Inventory2OutlinedIcon}
                            label={t('dashboard.stats.stockOut')}
                            value={stockOutQuantity.toLocaleString()}
                            color="#C2185B"
                            href="/inventory"
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <StatCard
                            icon={PaymentsOutlinedIcon}
                            label={t('dashboard.stats.totalPaidSalary')}
                            value={`SAR ${scopedPaidSalaryTotal.toLocaleString()}`}
                            color="#2E7D32"
                            href="/monthly-summaries"
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                        <StatCard
                            icon={PendingActionsIcon}
                            label={t('dashboard.stats.totalSalaryToPay')}
                            value={`SAR ${scopedSalaryToPayTotal.toLocaleString()}`}
                            color="#F9A825"
                            href="/monthly-summaries"
                        />
                    </Grid>
                </Grid>
            )}

            <Grid container spacing={3} sx={{ mt: 3 }}>
                <Grid size={{ xs: 12, md: isFleetStaff ? 8 : 12 }}>
                    <OrdersTrendChart
                        data={trendData}
                        isLoading={trendLoading}
                        title={isFleetStaff ? t('dashboard.charts.fleetOrdersTrend') : t('dashboard.charts.myOrdersTrend')}
                    />
                </Grid>
                {isFleetStaff && (
                    <Grid size={{ xs: 12, md: 4 }}>
                        <RidersByPlatformCard data={rolesByPlatform} title={t('dashboard.charts.ridersByPlatform')} />
                    </Grid>
                )}
                {isFleetStaff && (
                    <Grid size={{ xs: 12, md: 6 }}>
                        <EmployeeStatusChart data={statusData} isLoading={false} title={t('dashboard.charts.employeeStatus')} />
                    </Grid>
                )}
                {isAdministrator && (
                    <Grid size={{ xs: 12, md: 6 }}>
                        <FinancialOverviewChart
                            data={financialData}
                            isLoading={false}
                            title={t('dashboard.charts.financialOverview')}
                        />
                    </Grid>
                )}
                {isFinance && (
                    <Grid size={{ xs: 12, md: 6 }}>
                        <MonthlyAmountTrendChart
                            data={expenseMonthData}
                            isLoading={expenseTrendLoading}
                            title={t('dashboard.charts.expensesTrend')}
                            barName={t('dashboard.charts.expenses')}
                            color="#7C5CFC"
                        />
                    </Grid>
                )}
                {isFinance && (
                    <Grid size={{ xs: 12, md: 6 }}>
                        <MonthlyAmountTrendChart
                            data={salaryPaidMonthData}
                            isLoading={salaryTrendLoading}
                            title={t('dashboard.charts.salaryPaidTrend')}
                            barName={t('dashboard.stats.totalPaidSalary')}
                            color="#2E7D32"
                        />
                    </Grid>
                )}
            </Grid>
        </Box>
    );
}
