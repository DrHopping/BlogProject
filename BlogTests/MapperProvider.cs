using AutoMapper;
using BLL.Mappings;

namespace BlogTests
{
    internal class MapperProvider
    {
        public static IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return new Mapper(configuration);
        }
    }
}