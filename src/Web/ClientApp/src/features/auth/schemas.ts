import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors LoginCommandValidator (Application/Auth/Commands/Login) and
// RegisterEmployeeCommandValidator (Application/Employees/Commands/RegisterEmployee) —
// keep in sync with those FluentValidation rules.
export function buildLoginSchema(t: TFunction<'auth'>) {
  return z.object({
    email: z.string().min(1, t('validation.required')).email(t('validation.invalidEmail')),
    password: z.string().min(6, t('validation.passwordMinLogin')),
  });
}

export function buildRegisterSchema(t: TFunction<'auth'>) {
  return z.object({
    fullName: z.string().min(1, t('validation.required')).max(150, t('validation.fullNameMax')),
    email: z.string().min(1, t('validation.required')).email(t('validation.invalidEmail')),
    password: z
      .string()
      .min(8, t('validation.passwordMinRegister'))
      .regex(/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])/, t('validation.passwordComplexity')),
    phoneNumber: z
      .string()
      .min(1, t('validation.required'))
      .regex(/^\+?[0-9]{9,15}$/, t('validation.phonePattern')),
    hasWhatsApp: z.boolean(),
  });
}

// Mirrors ChangePasswordCommandValidator (Application/Auth/Commands/ChangePassword).
export function buildChangePasswordSchema(t: TFunction<'auth'>) {
  return z
    .object({
      currentPassword: z.string().min(1, t('validation.required')),
      newPassword: z
        .string()
        .min(8, t('validation.passwordMinRegister'))
        .regex(/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])/, t('validation.passwordComplexity')),
      confirmNewPassword: z.string().min(1, t('validation.required')),
    })
    .refine((values) => values.newPassword === values.confirmNewPassword, {
      message: t('validation.passwordMismatch'),
      path: ['confirmNewPassword'],
    })
    .refine((values) => values.newPassword !== values.currentPassword, {
      message: t('validation.samePassword'),
      path: ['newPassword'],
    });
}

export type LoginFormValues = z.infer<ReturnType<typeof buildLoginSchema>>;
export type RegisterFormValues = z.infer<ReturnType<typeof buildRegisterSchema>>;
export type ChangePasswordFormValues = z.infer<ReturnType<typeof buildChangePasswordSchema>>;
