export const environment = {
  production: process.env['NODE_ENV'] === 'production',
  apiV1BaseUrl: process.env['API_V1_BASE_URL'] || 'http://localhost:6167/api/v1'
};
