export function truncate(text: string, maxLength = 30): string {
  return text.length > maxLength ? `${text.slice(0, maxLength)}...` : text;
}
