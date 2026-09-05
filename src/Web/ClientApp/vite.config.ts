import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Aspire injects these env vars with the webapi resource's actual (dynamic)
// port. Running `npm start` directly against a VS-launched Web project (no
// Aspire) falls back to that project's launchSettings.json "https" profile.
const target =
  process.env['services__webapi__https__0'] ||
  process.env['services__webapi__http__0'] ||
  'https://localhost:7000';

const proxyOptions = { target, secure: false, changeOrigin: true };

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT || '5173'),
    proxy: {
      '/api': proxyOptions,
      '/openapi': proxyOptions,
      '/scalar': proxyOptions,
    },
  },
  build: {
    outDir: 'build',
  },
});
