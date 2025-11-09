
namespace SurveyBasket.Dal.Persistence;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Poll>Polls{get;set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
