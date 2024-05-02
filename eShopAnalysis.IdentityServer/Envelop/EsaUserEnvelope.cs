using eShopAnalysis.IdentityServer.Dto;

namespace eShopAnalysis.IdentityServer.Envelop
{
    //send from client to server with userId wrap, we do not want to add userId to EsaUserDto since it is read model (in client)
    public record EsaUserEnvelope(EsaUserDto EsaUserDto, string UserId);
}
