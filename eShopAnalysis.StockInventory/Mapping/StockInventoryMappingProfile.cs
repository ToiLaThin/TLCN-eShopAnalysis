using AutoMapper;
using eShopAnalysis.StockInventoryAPI.Dto;
namespace eShopAnalysis.StockInventoryAPI.Mapping
{
    using eShopAnalysis.StockInventory.Models; //diff from stock inventory namespace
    public class StockInventoryMappingProfile : Profile
    {
        public StockInventoryMappingProfile() {
            //Ulid có thể tự mapping ra Guid được mà không cần định nghĩa

            //CreateMap<StockInventory, StockInventoryDto>()
            //    .ForMember(dest => dest.StockInventoryId, option => option.ConvertUsing(new GuidValueConverterFromUlid(), "StockInventoryId"))
            //    .ReverseMap();
            CreateMap<StockInventory, StockInventoryDto>()
                .ReverseMap();
        }        
    }
    //public class GuidValueConverterFromUlid : IValueConverter<Ulid, Guid>
    //{
    //    public Guid Convert(Ulid sourceMember, ResolutionContext context) => sourceMember.ToGuid();
    //}
    //public class UlidValueConverterFromGuid : IValueConverter<Guid, Ulid>
    //{
    //    public Ulid Convert(Guid sourceMember, ResolutionContext context)
    //    {
    //        var result = new Ulid(sourceMember);
    //        return result;
    //    }
    //}
}
