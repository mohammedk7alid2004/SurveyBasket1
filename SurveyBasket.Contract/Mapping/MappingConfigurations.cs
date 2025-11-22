using Mapster;
using SurveyBasket.Contract.Contracts.Questions;
using SurveyBasket.Entities.Entities;

namespace SurveyBasket.Contract.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
     .Map(dest => dest.Answers,
          src => src.Answers.Select(ans => new Answer
          {
              Content = ans,
              IsActive = true
          }));

    }
}
