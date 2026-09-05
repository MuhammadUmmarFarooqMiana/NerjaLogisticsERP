import { createTheme, type Theme } from '@mui/material/styles';
import type { ResolvedThemeMode } from './ThemeModeContext';

export type AppDirection = 'ltr' | 'rtl';

// "Azure Mono" theme (21st.dev/@serafimcloud/themes/azure-mono) — light-mode
// tokens copied verbatim from the theme's published palette; dark-mode tokens
// aren't published by the theme (light-only), so they're derived using the
// same brand blue against the theme's own Secondary (a dark navy) as the
// dark surface base, keeping the two modes visually related.
export const BRAND_ORANGE = '#FF6C35'; // Azure Mono "Destructive" — used as the theme's warm accent
const AZURE_MONO_PRIMARY = '#2E67FF';
const AZURE_MONO_SECONDARY_NAVY = '#0E142B';

export function createAppTheme(direction: AppDirection, mode: ResolvedThemeMode = 'light'): Theme {
  const isDark = mode === 'dark';

  return createTheme({
    direction,
    palette: {
      mode,
      primary: { main: AZURE_MONO_PRIMARY },
      secondary: { main: isDark ? '#5FA8D3' : AZURE_MONO_SECONDARY_NAVY },
      error: { main: BRAND_ORANGE },
      background: isDark
        ? { default: AZURE_MONO_SECONDARY_NAVY, paper: '#141B33' }
        : { default: '#FCFCFC', paper: '#FAFAFA' },
      ...(isDark && {
        text: { primary: '#F5F6F8', secondary: 'rgba(245, 246, 248, 0.7)' },
        divider: 'rgba(255, 255, 255, 0.12)',
      }),
    },
    typography: {
      fontFamily: '"Open Sans", "Segoe UI", Arial, sans-serif',
    },
    shape: {
      borderRadius: 20,
    },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          // Plain UI text (headings, labels, nav items, stat values, ...) is
          // selectable by default in a browser, so clicking it drops a text
          // caret and lets it be dragged into a selection — reads as broken
          // in an app-like dashboard. Turn it off globally, then explicitly
          // re-enable it for the elements that actually need it: real text
          // inputs/textareas and anything deliberately marked editable.
          'body': {
            userSelect: 'none',
            WebkitUserSelect: 'none',
          },
          'input, textarea, [contenteditable="true"]': {
            userSelect: 'text',
            WebkitUserSelect: 'text',
          },
          // MUI's own InputBase styles apply a hardcoded rgb(38, 103, 152) inset
          // box-shadow to autofilled inputs (its own internal autofill-detection
          // hook, unrelated to this app's theme) via `.MuiOutlinedInput-input:-webkit-autofill`
          // — a class selector, which beats a plain `input:-webkit-autofill` override
          // on specificity. Match the same two classes MUI applies to win the cascade
          // and drop the shadow outright; keep text/caret visible against whatever
          // native background the browser still paints underneath.
          '.MuiInputBase-input.MuiOutlinedInput-input:-webkit-autofill, .MuiInputBase-input.MuiOutlinedInput-input:-webkit-autofill:hover, .MuiInputBase-input.MuiOutlinedInput-input:-webkit-autofill:focus, .MuiInputBase-input.MuiOutlinedInput-input:-webkit-autofill:active':
            {
              boxShadow: 'none',
              WebkitTextFillColor: isDark ? '#F5F6F8' : 'inherit',
              caretColor: isDark ? '#F5F6F8' : 'inherit',
              transition: 'background-color 5000s ease-in-out 0s',
            },
        },
      },
      // MuiOutlinedInput inherits theme.shape.borderRadius (20px) unmultiplied — with
      // the input's default 14px horizontal padding, that curve is wider than the
      // padding, so the rounded corner bleeds into the text area and the caret/text
      // appear to sit outside the visible box. Inputs get their own smaller radius
      // instead, comfortably cleared by the default padding.
      MuiOutlinedInput: {
        styleOverrides: {
          // MUI's default hover swaps the outline border to palette.text.primary,
          // which in dark mode is a near-white at full opacity: a stark line
          // unrelated to the rest of the app. No background fill on hover either —
          // just keep the outline border fixed at its normal (rest-state) color.
          root: {
            borderRadius: 12,
            '&:hover:not(.Mui-focused):not(.Mui-error) .MuiOutlinedInput-notchedOutline': {
              borderColor: isDark ? 'rgba(255, 255, 255, 0.23)' : 'rgba(0, 0, 0, 0.23)',
            },
          },
        },
      },
    },
  });
}
