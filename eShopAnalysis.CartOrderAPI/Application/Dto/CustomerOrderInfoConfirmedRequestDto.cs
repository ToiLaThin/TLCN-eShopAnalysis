using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Application.Dto
{
    public class CustomerOrderGeometryDto
    {
        public double Lat { get; }

        public double Lng { get; }
    }

    public class CustomerOrderInfoConfirmedRequestDto
    {
        public Guid OrderId { get; set; }
        public CustomerOrderGeometryDto Geometry { get; set; }

        public Address Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}
