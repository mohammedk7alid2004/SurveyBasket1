
namespace SurveyBasket.Dal.Persistence.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
       builder.HasIndex(builder => new { builder.PollId, builder.UserId }).IsUnique();
    }
}
