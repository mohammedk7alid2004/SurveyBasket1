namespace SurveyBasket.Dal.Persistence.EntitiesConfigurations;

public class UserConfiguration:IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(r => r.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");
        builder
            .Property(u => u.FirstName)
            .HasMaxLength(50);
        builder
            .Property(u => u.LastName)
            .HasMaxLength(50);
    }

}
