using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate
{
    public class Address : ValueObject
    {
        public string Country { get; private set; }
        public string CityOrProvinceOrPlace { get; private set; }
        public string DistrictOrLocality { get; private set; }
        public string PostalCode { get; private set; }
        public string Street { get; private set; }
        public string FullAddressName { get; set; }

        public Address(string country,
            string cityOrProvinceOrPlace,
            string districtOrLocality,
            string postalCode,
            string street,
            string fullAddressName)
        {
            Country = country ?? throw new ArgumentNullException($"{nameof(country)} is required");
            CityOrProvinceOrPlace = cityOrProvinceOrPlace ?? throw new ArgumentNullException($"{nameof(cityOrProvinceOrPlace)} is required");
            DistrictOrLocality = districtOrLocality ?? throw new ArgumentNullException($"{nameof(districtOrLocality)} is required");
            PostalCode = postalCode ?? throw new ArgumentNullException($"{nameof(postalCode)} is required");
            Street = street ?? throw new ArgumentNullException($"{nameof(street)} is required");
            FullAddressName = fullAddressName ?? throw new ArgumentNullException($"{nameof(fullAddressName)} is required");
        }

        




        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Country;
            yield return CityOrProvinceOrPlace;
            yield return DistrictOrLocality;
            yield return PostalCode;
            yield return Street;
            yield return FullAddressName;
        }
    }
}
