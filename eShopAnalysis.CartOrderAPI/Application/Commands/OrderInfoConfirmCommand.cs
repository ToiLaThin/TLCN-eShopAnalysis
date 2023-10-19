using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;
using MediatR;
using System.Net;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class OrderInfoConfirmCommand: IRequest<Order>
    {
        public Guid OrderId { get; set; }

        public Address Address { get; set; }

        public string PhoneNumber { get; set; }

        public OrderInfoConfirmCommand(Guid? orderId, string country, string cityOrProvinceOrPlace, string districtOrLocality, string postalCode, string street, string fullName, string phoneNumber)  
        {
            //validate all the input or not or we can change parameter to only take in the address
            this.OrderId = orderId ?? throw new ArgumentNullException($"{nameof(orderId)} is required in validation process");
            this.Address = new Address(country, cityOrProvinceOrPlace, districtOrLocality, postalCode, street, fullName);
            this.PhoneNumber = phoneNumber;
        }

        public OrderInfoConfirmCommand(Guid? orderId, Address address, string phoneNumber)
        {
            //validate all the input or not or we can change parameter to only take in the address
            this.OrderId = orderId ?? throw new ArgumentNullException($"{nameof(orderId)} is required in validation process");
            this.Address = address ?? throw new ArgumentNullException($"{nameof(address)} is required in validation process");
            this.PhoneNumber = phoneNumber;
        }


    }
}
