
namespace SurveyBasket.Dal.Persistence.EntitiesConfigurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder
            .HasIndex(p => p.Title)
            .IsUnique();
        builder
            .Property(p => p.Title)
            .HasMaxLength(100);
        builder
          .Property(p => p.Description)
          .HasMaxLength(1500);

    } 
}
