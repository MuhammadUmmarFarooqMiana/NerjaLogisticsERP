/** "Muhammad Umar Farooq" -> "MU" — first letter of the first two words, uppercased. */
export function getInitials(fullName: string): string {
  const parts = fullName.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  const initials = parts.length === 1 ? parts[0][0] : parts[0][0] + parts[1][0];
  return initials.toUpperCase();
}

/** Deterministic fallback-avatar background color derived from the name, so the same
 * person always gets the same color instead of a random one on every render. */
export function stringToColor(value: string): string {
  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = value.charCodeAt(i) + ((hash << 5) - hash);
  }
  const hue = Math.abs(hash) % 360;
  return `hsl(${hue}, 45%, 40%)`;
}
