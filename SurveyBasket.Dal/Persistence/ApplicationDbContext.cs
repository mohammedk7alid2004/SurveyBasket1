

namespace SurveyBasket.Dal.Persistence;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext>options , IHttpContextAccessor httpContext) :IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContext = httpContext;

    public DbSet<Poll>Polls{get;set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync (CancellationToken cancellationToken=default)
    {
        var userId = _httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var entries = ChangeTracker.Entries<AuditableEntity>();
        foreach(var entity in entries)
        {
            if ((entity.State)==EntityState.Added)
            {
                entity.Property(x => x.CreatedById).CurrentValue = userId;

            }
            if (entity.State==EntityState.Modified)
            {
                entity.Property(x => x.UpdatedById).CurrentValue = userId;
                entity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync (cancellationToken);
    }
}
