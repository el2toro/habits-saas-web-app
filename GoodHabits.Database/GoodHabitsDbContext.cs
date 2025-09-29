using Microsoft.EntityFrameworkCore;
using GoodHabits.Database.Entities;
using GoodHabits.Database.Interfaces;

namespace GoodHabits.Database;

public class GoodHabitsDbContext : DbContext
{
    private readonly ITenantService _tenantService;
    public GoodHabitsDbContext(DbContextOptions options, ITenantService service) : base(options) =>
      _tenantService = service;

    public string TenantName
    {
        get => _tenantService.GetTenant()?.TenantName ?? String.Empty;
    }
    public DbSet<Habit>? Habits { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Get the connection string from the tenant service. During startup (migrations)
        // the tenant service may return the default connection string if no tenant is set.
        var tenantConnectionString = _tenantService?.GetConnectionString();
        if (!string.IsNullOrEmpty(tenantConnectionString))
        {
            optionsBuilder.UseSqlServer(tenantConnectionString);
        }
    }

    //Another important change to note is that we add a query filter at the context level. This ensures that 
    //only the correct tenant can read their data, which is a very important security consideration.
    protected override void OnModelCreating(ModelBuilder
      modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Habit>()
            .HasQueryFilter(a => a.TenantName == TenantName);

        SeedData.Seed(modelBuilder);
    }

    //Finally, we have overridden the SaveChangesAsync method. This allows us to set the tenant 
    //name here and not have to consider it in any of our other implementation code. This cleans up the 
    //rest of our code considerably.
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ChangeTracker.Entries<IHasTenant>()
            .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified).ToList()
            .ForEach(entry => entry.Entity.TenantName = TenantName);

        return await base.SaveChangesAsync(cancellationToken);
    }
}