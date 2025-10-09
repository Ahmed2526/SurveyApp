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

            config.NewConfig<Question, QuestionResponse>().Map(e => e.Answers, e => e.Answers.Select(e => e.Content));

            config.NewConfig<QuestionRequest, Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(a => new Answer { Content = a }).ToList());

        }
    }
}
