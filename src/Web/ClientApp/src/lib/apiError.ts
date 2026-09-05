import type { FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import i18n from '../i18n/config';

interface ProblemDetailsBody {
  detail?: string;
  title?: string;
  errors?: Record<string, string[]>;
  /** Stamped on every ProblemDetails response by AddProblemDetails() in DependencyInjection.cs. */
  traceId?: string;
}

function isFetchBaseQueryError(error: unknown): error is FetchBaseQueryError {
  return typeof error === 'object' && error !== null && 'status' in error;
}

/**
 * Extracts the actual validation/error messages from a caught RTK Query error
 * (a ProblemDetails body from the backend's ProblemDetailsExceptionHandler),
 * instead of collapsing everything to a generic "something went wrong" string.
 * Always returns at least one message — `fallback` when nothing usable is found.
 *
 * Field-level validation errors and known business-rule messages (Conflict, NotFound, ...)
 * are self-explanatory, so they're returned as-is. Only the generic, no-detail case — an
 * unrecognised exception the backend deliberately didn't describe, which is exactly the kind
 * of failure worth flagging — gets the server's traceId appended, giving the user something
 * concrete to hand to an admin without ever exposing the underlying exception itself.
 */
export function getApiErrorMessages(error: unknown, fallback: string): string[] {
  if (!isFetchBaseQueryError(error)) return [fallback];

  const body = error.data as ProblemDetailsBody | undefined;
  const reference = body?.traceId ? [i18n.t('common:errors.reference', { traceId: body.traceId })] : [];

  // ASP.NET validation ProblemDetails: { errors: { field: ["msg", ...] } }
  if (body?.errors) {
    const messages = Object.values(body.errors).flat();
    if (messages.length > 0) return messages;
  }

  // ConflictException etc. join multiple messages into `detail` with "; ".
  if (body?.detail) {
    const messages = body.detail
      .split(';')
      .map((message) => message.trim())
      .filter(Boolean);
    if (messages.length > 0) return messages;
  }

  if (body?.title) return [body.title, ...reference];

  return [fallback, ...reference];
}
