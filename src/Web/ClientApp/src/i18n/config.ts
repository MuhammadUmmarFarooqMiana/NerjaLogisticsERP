import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import enCommon from './locales/en/common.json';
import arCommon from './locales/ar/common.json';
import enAuth from './locales/en/auth.json';
import arAuth from './locales/ar/auth.json';
import enEmployees from './locales/en/employees.json';
import arEmployees from './locales/ar/employees.json';
import enDailyOrders from './locales/en/dailyOrders.json';
import arDailyOrders from './locales/ar/dailyOrders.json';
import enFines from './locales/en/fines.json';
import arFines from './locales/ar/fines.json';
import enAdvances from './locales/en/advances.json';
import arAdvances from './locales/ar/advances.json';
import enExpenses from './locales/en/expenses.json';
import arExpenses from './locales/ar/expenses.json';
import enSalaryFormulas from './locales/en/salaryFormulas.json';
import arSalaryFormulas from './locales/ar/salaryFormulas.json';
import enMonthlySummaries from './locales/en/monthlySummaries.json';
import arMonthlySummaries from './locales/ar/monthlySummaries.json';
import enVehicles from './locales/en/vehicles.json';
import arVehicles from './locales/ar/vehicles.json';
import enInventory from './locales/en/inventory.json';
import arInventory from './locales/ar/inventory.json';
import enSuppliers from './locales/en/suppliers.json';
import arSuppliers from './locales/ar/suppliers.json';
import enMechanics from './locales/en/mechanics.json';
import arMechanics from './locales/ar/mechanics.json';
import enLeaveRequests from './locales/en/leaveRequests.json';
import arLeaveRequests from './locales/ar/leaveRequests.json';
import enNotifications from './locales/en/notifications.json';
import arNotifications from './locales/ar/notifications.json';
import enCompanyDocuments from './locales/en/companyDocuments.json';
import arCompanyDocuments from './locales/ar/companyDocuments.json';
import enUsers from './locales/en/users.json';
import arUsers from './locales/ar/users.json';
import enReports from './locales/en/reports.json';
import arReports from './locales/ar/reports.json';
import enAuditInvoice from './locales/en/auditInvoice.json';
import arAuditInvoice from './locales/ar/auditInvoice.json';

export const defaultNamespace = 'common';

const LANGUAGE_STORAGE_KEY = 'nerja-language';

function getStoredLanguage(): string {
  try {
    return localStorage.getItem(LANGUAGE_STORAGE_KEY) ?? 'en';
  } catch {
    return 'en';
  }
}

i18n.on('languageChanged', (lng) => {
  try {
    localStorage.setItem(LANGUAGE_STORAGE_KEY, lng);
  } catch {
    // localStorage unavailable (private browsing, etc.) — language just won't persist.
  }
});

void i18n.use(initReactI18next).init({
  resources: {
    en: {
      common: enCommon,
      auth: enAuth,
      employees: enEmployees,
      dailyOrders: enDailyOrders,
      fines: enFines,
      advances: enAdvances,
      expenses: enExpenses,
      salaryFormulas: enSalaryFormulas,
      monthlySummaries: enMonthlySummaries,
      vehicles: enVehicles,
      inventory: enInventory,
      suppliers: enSuppliers,
      mechanics: enMechanics,
      leaveRequests: enLeaveRequests,
      notifications: enNotifications,
      companyDocuments: enCompanyDocuments,
      users: enUsers,
      reports: enReports,
      auditInvoice: enAuditInvoice,
    },
    ar: {
      common: arCommon,
      auth: arAuth,
      employees: arEmployees,
      dailyOrders: arDailyOrders,
      fines: arFines,
      advances: arAdvances,
      expenses: arExpenses,
      salaryFormulas: arSalaryFormulas,
      monthlySummaries: arMonthlySummaries,
      vehicles: arVehicles,
      inventory: arInventory,
      suppliers: arSuppliers,
      mechanics: arMechanics,
      leaveRequests: arLeaveRequests,
      notifications: arNotifications,
      companyDocuments: arCompanyDocuments,
      users: arUsers,
      reports: arReports,
      auditInvoice: arAuditInvoice,
    },
  },
  ns: [
    'common',
    'auth',
    'employees',
    'dailyOrders',
    'fines',
    'advances',
    'expenses',
    'salaryFormulas',
    'monthlySummaries',
    'vehicles',
    'inventory',
    'suppliers',
    'mechanics',
    'leaveRequests',
    'notifications',
    'companyDocuments',
    'users',
    'reports',
    'auditInvoice',
  ],
  lng: getStoredLanguage(),
  fallbackLng: 'en',
  defaultNS: defaultNamespace,
  interpolation: { escapeValue: false },
});

export default i18n;
