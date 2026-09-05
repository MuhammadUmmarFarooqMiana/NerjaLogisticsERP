export interface DecodedAccessToken {
  userId: string;
  email: string;
  fullName: string;
  roles: string[];
}

function base64UrlDecode(segment: string): string {
  const padded = segment.replace(/-/g, '+').replace(/_/g, '/').padEnd(Math.ceil(segment.length / 4) * 4, '=');
  return decodeURIComponent(
    atob(padded)
      .split('')
      .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
      .join('')
  );
}

/**
 * ASP.NET Core's JwtSecurityTokenHandler maps ClaimTypes.* URIs to short JWT
 * names on write (nameid/email/role) — read both forms defensively since that
 * mapping is an implementation detail, not a documented contract.
 */
export function decodeAccessToken(token: string): DecodedAccessToken | null {
  try {
    const payload = JSON.parse(base64UrlDecode(token.split('.')[1])) as Record<string, unknown>;

    const userId = (payload['nameid'] ??
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
      payload['sub']) as string | undefined;
    const email = (payload['email'] ??
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']) as string | undefined;
    const fullName = payload['fullName'] as string | undefined;
    const rawRoles = (payload['role'] ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']) as string | string[] | undefined;

    if (!userId || !email) return null;

    const roles = Array.isArray(rawRoles) ? rawRoles : rawRoles ? [rawRoles] : [];

    return { userId, email, fullName: fullName ?? email, roles };
  } catch {
    return null;
  }
}
