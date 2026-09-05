/** @type {import('@rtk-query/codegen-openapi').ConfigFile} */
const config = {
  schemaFile: '../wwwroot/openapi/v1.json',
  apiFile: './src/api/baseApi.ts',
  apiImport: 'baseApi',
  outputFile: './src/api/generated/apiSlice.ts',
  exportName: 'apiSlice',
  hooks: true,
};

export default config;
