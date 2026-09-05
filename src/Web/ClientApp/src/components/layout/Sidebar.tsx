import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import CalculateOutlinedIcon from '@mui/icons-material/CalculateOutlined';
import DashboardIcon from '@mui/icons-material/Dashboard';
import DirectionsCarFilledOutlinedIcon from '@mui/icons-material/DirectionsCarFilledOutlined';
import EventBusyOutlinedIcon from '@mui/icons-material/EventBusyOutlined';
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined';
import FolderOutlinedIcon from '@mui/icons-material/FolderOutlined';
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined';
import HandymanOutlinedIcon from '@mui/icons-material/HandymanOutlined';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import PeopleIcon from '@mui/icons-material/People';
import PendingActionsIcon from '@mui/icons-material/PendingActions';
import PersonIcon from '@mui/icons-material/Person';
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined';
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined';
import SummarizeOutlinedIcon from '@mui/icons-material/SummarizeOutlined';
import WalletOutlinedIcon from '@mui/icons-material/WalletOutlined';
import {
    Badge,
    Box,
    Drawer,
    IconButton,
    List,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Tooltip,
    Toolbar,
    useTheme,
} from '@mui/material';
import type { SvgIconProps } from '@mui/material';
import type { ComponentType } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesMeQuery, useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useGetPendingDailyOrderApprovalsCountQuery } from '../../api/dailyOrdersApi';
import { useAppSelector } from '../../app/hooks';
import { Logo } from '../branding/Logo';
import { selectCurrentUser } from '../../features/auth/authSlice';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { DRAWER_WIDTH, DRAWER_WIDTH_COLLAPSED } from './layout.constants';

const PENDING_APPROVALS_PATH = '/employees/pending';
const DAILY_ORDERS_PATH = '/daily-orders';
const PROFILE_PATH = '/profile';

// Mirrors RequireActiveProfile's gating exactly — staff roles run the platform
// rather than going through the rider profile approval workflow, so they're
// never restricted to just My Profile.
const EXEMPT_ROLES: string[] = [Roles.Administrator, Roles.SoftwareEngineer];
const RESTRICTED_STATUSES = new Set(['Incomplete', 'PendingReview', 'Rejected']);

interface NavItem {
    labelKey: string;
    path: string;
    icon: ComponentType<SvgIconProps>;
    /** Omit to show for every authenticated role. */
    roles?: string[];
}

// Grows as each module in FRONTEND.md's build order ships — only routes that
// actually exist belong here.
const navItems: NavItem[] = [
    { labelKey: 'nav.dashboard', path: '/dashboard', icon: DashboardIcon },
    {
        labelKey: 'nav.dailyOrders',
        path: DAILY_ORDERS_PATH,
        icon: LocalShippingOutlinedIcon,
        roles: [Roles.Administrator, Roles.Supervisor, Roles.Rider],
    },
    {
        labelKey: 'nav.pendingApprovals',
        path: PENDING_APPROVALS_PATH,
        icon: PendingActionsIcon,
        roles: [Roles.Administrator, Roles.Supervisor],
    },
    { labelKey: 'nav.myProfile', path: '/profile', icon: PersonIcon, roles: [Roles.Rider] },
    {
        labelKey: 'nav.expenses',
        path: '/expenses',
        icon: ReceiptLongOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
    {
        labelKey: 'nav.fines',
        path: '/fines',
        icon: GavelOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant, Roles.Rider],
    },
    {
        labelKey: 'nav.advances',
        path: '/advances',
        icon: WalletOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant, Roles.Rider],
    },
    {
        labelKey: 'nav.vehicles',
        path: '/vehicles',
        icon: DirectionsCarFilledOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant, Roles.Rider],
    }, {
        labelKey: 'nav.inventory',
        path: '/inventory',
        icon: Inventory2OutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
    
    {
        labelKey: 'nav.monthlySummaries',
        path: '/monthly-summaries',
        icon: SummarizeOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant, Roles.Rider],
    },
    {
        labelKey: 'nav.salaryFormulas',
        path: '/salary-formulas',
        icon: CalculateOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
    
    { labelKey: 'nav.leaveRequests', path: '/leave-requests', icon: EventBusyOutlinedIcon },
    {
        labelKey: 'nav.employees',
        path: '/employees',
        icon: PeopleIcon,
        roles: [Roles.Administrator, Roles.Supervisor],
    },
    {
        labelKey: 'nav.reports',
        path: '/reports',
        icon: AssessmentOutlinedIcon,
        roles: [Roles.Administrator, Roles.Supervisor, Roles.Accountant],
    },
    {
        labelKey: 'nav.suppliers',
        path: '/suppliers',
        icon: StorefrontOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
    {
        labelKey: 'nav.mechanics',
        path: '/mechanics',
        icon: HandymanOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
    {
        labelKey: 'nav.companyDocuments',
        path: '/company-documents',
        icon: FolderOutlinedIcon,
        roles: [Roles.Administrator, Roles.Supervisor, Roles.Accountant],
    },
    {
        labelKey: 'nav.platformReconciliation',
        path: '/platform-reconciliation',
        icon: FactCheckOutlinedIcon,
        roles: [Roles.Administrator, Roles.Accountant],
    },
];

// Keeps the sidebar scrollable while hiding the scrollbar chrome across
// browsers — Firefox (scrollbarWidth), old Edge/IE (-ms-overflow-style), and
// WebKit/Blink (::-webkit-scrollbar), which needs its own selector since it
// doesn't respect the standard properties.
const hiddenScrollbarSx = {
    scrollbarWidth: 'none',
    msOverflowStyle: 'none',
    '&::-webkit-scrollbar': { display: 'none' },
} as const;

interface SidebarProps {
    mobileOpen: boolean;
    onClose: () => void;
    collapsed: boolean;
    onToggleCollapsed: () => void;
}

export function Sidebar({ mobileOpen, onClose, collapsed, onToggleCollapsed }: SidebarProps) {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();
    const user = useAppSelector(selectCurrentUser);
    const theme = useTheme();

    const roleFilteredItems = navItems.filter(
        (item) => !item.roles || (user && item.roles.some((role) => user.roles.includes(role)))
    );

    // While a profile is Incomplete/PendingReview, every tab but My Profile is
    // hidden — mirrors RequireActiveProfile, which already bounces any direct
    // navigation to those routes back to /profile.
    const isExempt = !!user?.roles.some((role) => EXEMPT_ROLES.includes(role));
    const { data: employee } = useGetApiEmployeesMeQuery(undefined, { skip: isExempt });
    const isRestricted = !isExempt && !!employee?.accountStatus && RESTRICTED_STATUSES.has(employee.accountStatus);

    const visibleItems = isRestricted
        ? roleFilteredItems.filter((item) => item.path === PROFILE_PATH)
        : roleFilteredItems;

    // Only fetched for roles that can actually see the Pending Approvals item — pageSize: 1 keeps
    // this cheap, since the X-Pagination header's totalCount is all that's needed. Polling mirrors
    // NotificationBell's cadence; the 'Employee' cache tag also refetches this the moment an
    // approval/rejection happens anywhere in the app, so it doesn't rely on polling alone.
    const canSeePendingApprovals = visibleItems.some((item) => item.path === PENDING_APPROVALS_PATH);
    const { data: pendingApprovalsData } = useGetApiEmployeesQuery(
        { status: 'PendingReview', pageNumber: 1, pageSize: 1 },
        { skip: !canSeePendingApprovals, pollingInterval: 30000 }
    );
    const pendingApprovalsCount = getPaginationMeta(pendingApprovalsData)?.totalCount ?? 0;

    // Same reasoning as Pending Approvals above — a Rider sees Daily Orders too
    // (their own card), but only Administrator/Supervisor ever review anyone
    // else's, so the backend itself 403s a Rider here; skip fetching for them.
    const canSeeDailyOrderApprovals =
        visibleItems.some((item) => item.path === DAILY_ORDERS_PATH) &&
        !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Supervisor);
    const { data: dailyOrderApprovalsCount = 0 } = useGetPendingDailyOrderApprovalsCountQuery(undefined, {
        skip: !canSeeDailyOrderApprovals,
        pollingInterval: 30000,
    });

    // Nav paths can overlap (/employees is a prefix of /employees/pending), so a
    // plain startsWith would select both. Only the single longest (most specific) match wins.
    const activeItem = visibleItems.reduce<NavItem | null>((best, item) => {
        const matches = location.pathname === item.path || location.pathname.startsWith(`${item.path}/`);
        if (!matches) return best;
        if (!best || item.path.length > best.path.length) return item;
        return best;
    }, null);

    const handleNavigate = (path: string) => {
        navigate(path);
        onClose();
    };

    const navBadgeCount = (path: string): number => {
        if (path === PENDING_APPROVALS_PATH) return pendingApprovalsCount;
        if (path === DAILY_ORDERS_PATH) return dailyOrderApprovalsCount;
        return 0;
    };

    const renderNavList = (isCollapsed: boolean) => (
        <List sx={{ px: 1 }}>
            {visibleItems.map((item) => {
                const selected = item === activeItem;
                const button = (
                    <ListItemButton
                        key={item.path}
                        selected={selected}
                        onClick={() => handleNavigate(item.path)}
                        sx={{
                            borderRadius: 2,
                            mb: 0.5,
                            justifyContent: isCollapsed ? 'center' : 'flex-start',
                            px: isCollapsed ? 1.5 : 2,
                            transition: theme.transitions.create(['background-color', 'color'], { duration: 150 }),
                            '&:hover': { bgcolor: selected ? undefined : 'action.hover' },
                        }}
                        style={
                            selected
                                ? { backgroundColor: theme.palette.primary.main, color: theme.palette.primary.contrastText }
                                : undefined
                        }
                    >
                        <ListItemIcon sx={{ minWidth: 0, mr: isCollapsed ? 0 : 2, justifyContent: 'center' }}>
                            {navBadgeCount(item.path) > 0 ? (
                                <Badge badgeContent={navBadgeCount(item.path)} color="error" max={99}>
                                    <item.icon />
                                </Badge>
                            ) : (
                                <item.icon />
                            )}
                        </ListItemIcon>
                        {!isCollapsed && <ListItemText primary={t(item.labelKey)} />}
                    </ListItemButton>
                );

                return isCollapsed ? (
                    <Tooltip key={item.path} title={t(item.labelKey)} placement="right">
                        <span>{button}</span>
                    </Tooltip>
                ) : (
                    button
                );
            })}
        </List>
    );

    const brandHeader = (isCollapsed: boolean) => (
        <Toolbar sx={{ display: 'flex', alignItems: 'center', justifyContent: isCollapsed ? 'center' : 'flex-start' }}>
            {!isCollapsed && (
                <Box
                    role="button"
                    tabIndex={0}
                    aria-label={t('nav.dashboard')}
                    onClick={() => handleNavigate('/dashboard')}
                    onKeyDown={(event) => {
                        if (event.key === 'Enter' || event.key === ' ') {
                            event.preventDefault();
                            handleNavigate('/dashboard');
                        }
                    }}
                    sx={{ display: 'flex', alignItems: 'center', cursor: 'pointer' }}
                >
                    <Logo height={22} variant={theme.palette.mode === 'dark' ? 'white' : 'black'} />
                </Box>
            )}
        </Toolbar>
    );

    const collapseToggle = (
        <Box
            sx={{
                display: { xs: 'none', md: 'flex' },
                justifyContent: collapsed ? 'center' : 'flex-end',
                p: 1,
                borderTop: '1px solid',
                borderColor: 'divider',
            }}
        >
            <Tooltip title={collapsed ? t('actions.expandSidebar') : t('actions.collapseSidebar')} placement="right">
                <IconButton
                    onClick={onToggleCollapsed}
                    size="small"
                    aria-label={collapsed ? t('actions.expandSidebar') : t('actions.collapseSidebar')}
                    sx={{
                        border: '1px solid',
                        borderColor: 'divider',
                        transition: theme.transitions.create('transform', { duration: 200 }),
                        '&:hover': { bgcolor: 'action.hover', transform: 'scale(1.08)' },
                    }}
                >
                    {collapsed ? <ChevronRightIcon fontSize="small" /> : <ChevronLeftIcon fontSize="small" />}
                </IconButton>
            </Tooltip>
        </Box>
    );

    return (
        <>
            {/* Mobile/tablet: overlay drawer, closes on nav or backdrop click — never collapses */}
            <Drawer
                variant="temporary"
                open={mobileOpen}
                onClose={onClose}
                ModalProps={{ keepMounted: true }}
                slotProps={{
                    paper: { sx: { width: DRAWER_WIDTH, boxSizing: 'border-box', overflowX: 'hidden', ...hiddenScrollbarSx } },
                }}
                sx={{ display: { xs: 'block', md: 'none' } }}
            >
                {brandHeader(false)}
                {renderNavList(false)}
            </Drawer>

            {/* Desktop: permanent, collapsible icon rail. `key` forces a remount on toggle —
          this MUI version's Paper slot doesn't reliably re-apply a changed `style` width
          on an already-mounted instance, only on (re)mount, so we force that path. */}
            <Drawer
                key={collapsed ? 'collapsed' : 'expanded'}
                variant="permanent"
                slotProps={{
                    paper: {
                        style: {
                            width: collapsed ? DRAWER_WIDTH_COLLAPSED : DRAWER_WIDTH,
                            right: 'auto',
                            transition: theme.transitions.create('width', { duration: 200 }),
                        },
                        sx: {
                            boxSizing: 'border-box',
                            overflowX: 'hidden',
                            display: 'flex',
                            flexDirection: 'column',
                            ...hiddenScrollbarSx,
                        },
                    },
                }}
                style={{
                    width: collapsed ? DRAWER_WIDTH_COLLAPSED : DRAWER_WIDTH,
                    transition: theme.transitions.create('width', { duration: 200 }),
                }}
                sx={{
                    display: { xs: 'none', md: 'block' },
                    flexShrink: 0,
                    whiteSpace: 'nowrap',
                }}
            >
                {brandHeader(collapsed)}
                <Box sx={{ flexGrow: 1, overflowY: 'auto', overflowX: 'hidden', ...hiddenScrollbarSx }}>
                    {renderNavList(collapsed)}
                </Box>
                {collapseToggle}
            </Drawer>
        </>
    );
}
