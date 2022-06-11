using AutoMapper;
using XF.APP.DATA;
using XF.APP.DTO;

namespace XF.APP
{
    public static class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AllowNullCollections = true;
                config.AllowNullDestinationValues = true; 

                config.CreateMap<LookupKey, LookupKeyDto>();
                config.CreateMap<LookupKeyDto, LookupKey>();

                config.CreateMap<PurchaseOrder, PurchaseOrderDto>();
                config.CreateMap<PurchaseOrderDto, PurchaseOrder>();

                config.CreateMap<UserDetails, UserDetailsDto>();
                config.CreateMap<UserDetailsDto, UserDetails>();
            });
            return mappingConfig;
        }
    }
}
