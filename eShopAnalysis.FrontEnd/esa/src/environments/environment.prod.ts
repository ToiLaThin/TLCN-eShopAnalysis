export const environment = {
  production: true,
  BASEURL: 'https://localhost:7001',
  //ko the la ApiGateway.Ocelot (container name) vì browser ko biết đường dẫn đến container đó
  CLIENTROOT: 'http://localhost:4200',
  IDPAUTHORITY: 'https://localhost:7002/Auth/IdentityServer/',
  CLIENTID: 'client_id',
};
