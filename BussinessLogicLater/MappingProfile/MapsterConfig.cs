using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;

namespace BussinessLogicLater.MappingProfile
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            //Polls Mapping
            config.NewConfig<Poll, PollDto>().TwoWays();

            config.NewConfig<Poll, PollCreateDto>().TwoWays();

        }
    }
}
