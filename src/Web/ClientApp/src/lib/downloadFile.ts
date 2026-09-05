/**
 * Fetches a file from an [Authorize]-protected API endpoint. A plain <a href>/window.open
 * navigation can't carry the Bearer access token (it's only ever attached via fetchBaseQuery's
 * prepareHeaders), so every helper in this file fetches the bytes with the token first.
 */
async function fetchAuthorized(url: string, accessToken: string | null, init?: RequestInit): Promise<Response> {
  const response = await fetch(url, {
    credentials: 'include',
    ...init,
    headers: accessToken ? { ...init?.headers, Authorization: `Bearer ${accessToken}` } : init?.headers,
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }

  return response;
}

/** Downloads a file by saving it via a throwaway object URL. `init` defaults to a plain GET —
 * pass `{ method: 'POST', body: formData }` for endpoints that take an uploaded file. */
export async function downloadFile(
  url: string,
  accessToken: string | null,
  fallbackFileName: string,
  init?: RequestInit
) {
  const response = await fetchAuthorized(url, accessToken, init);

  const disposition = response.headers.get('Content-Disposition');
  const match = disposition?.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i);
  const fileName = match?.[1] ? decodeURIComponent(match[1]) : fallbackFileName;

  const blob = await response.blob();
  const objectUrl = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = objectUrl;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(objectUrl);
}

/**
 * Opens a file (typically a PDF) in a new tab using the browser's native viewer — which already
 * has zoom, page navigation, print, and download controls, so this doubles as the "preview"
 * experience without building a custom renderer.
 *
 * The tab is opened synchronously, before the `await`, so it's still tied to the click that
 * triggered this call — opening it only after the fetch resolves would get treated as an
 * unsolicited popup and blocked by most browsers.
 */
export async function previewFile(url: string, accessToken: string | null, init?: RequestInit) {
  const newTab = window.open('', '_blank');
  try {
    const response = await fetchAuthorized(url, accessToken, init);
    const blob = await response.blob();
    const objectUrl = URL.createObjectURL(blob);
    if (newTab) {
      newTab.location.href = objectUrl;
    } else {
      // Popup blocked despite the synchronous open (e.g. browser setting) — fall back
      // to replacing the current tab rather than silently doing nothing.
      window.location.href = objectUrl;
    }
  } catch (err) {
    newTab?.close();
    throw err;
  }
}

/**
 * Fetches a file and sends it straight to the browser's print dialog via a hidden iframe,
 * without navigating away from the current page.
 */
export async function printFile(url: string, accessToken: string | null, init?: RequestInit) {
  const response = await fetchAuthorized(url, accessToken, init);
  const blob = await response.blob();
  const objectUrl = URL.createObjectURL(blob);

  const iframe = document.createElement('iframe');
  iframe.style.position = 'fixed';
  iframe.style.right = '0';
  iframe.style.bottom = '0';
  iframe.style.width = '0';
  iframe.style.height = '0';
  iframe.style.border = '0';
  iframe.src = objectUrl;

  const cleanup = () => {
    iframe.remove();
    URL.revokeObjectURL(objectUrl);
  };

  iframe.onload = () => {
    // No cross-browser event fires when the print dialog itself closes, only 'afterprint' on the
    // frame's own window once printing starts/is cancelled — that's the best signal available.
    iframe.contentWindow?.addEventListener('afterprint', cleanup);
    iframe.contentWindow?.print();
    // Safety net in case 'afterprint' never fires (e.g. the user never interacts with the dialog).
    setTimeout(cleanup, 60000);
  };

  document.body.appendChild(iframe);
}
