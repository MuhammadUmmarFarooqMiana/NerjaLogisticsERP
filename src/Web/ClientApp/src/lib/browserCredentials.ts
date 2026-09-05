// Wraps the Credential Management API so the browser can offer to save the
// password right after a JS-driven (non-form-submit) login — Chrome/Edge only
// show their native "save password" prompt automatically for a real <form>
// submission, so a SPA login has to ask explicitly via navigator.credentials.store().
interface PasswordCredentialData {
  id: string;
  password: string;
  name?: string;
}

type PasswordCredentialConstructor = new (data: PasswordCredentialData) => Credential;

export async function storeBrowserPasswordCredential(data: PasswordCredentialData): Promise<void> {
  if (!('credentials' in navigator)) return;

  const PasswordCredentialCtor = (window as unknown as { PasswordCredential?: PasswordCredentialConstructor })
    .PasswordCredential;
  if (!PasswordCredentialCtor) return;

  try {
    await navigator.credentials.store(new PasswordCredentialCtor(data));
  } catch {
    // Unsupported context (e.g. iframe, permissions policy) — never block the login flow over this.
  }
}
