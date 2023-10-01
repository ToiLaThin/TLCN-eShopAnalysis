export const environment = {
  production: true,
  BASEURL: 'http://localhost:7001',
  //ko the la ApiGateway.Ocelot (container name) vì browser ko biết đường dẫn đến container đó
  CLIENTROOT: 'http://localhost:4200',
  IDPAUTHORITY: 'http://localhost:7001/Auth/IdentityServer/',
  CLIENTID: 'client_id',
};
