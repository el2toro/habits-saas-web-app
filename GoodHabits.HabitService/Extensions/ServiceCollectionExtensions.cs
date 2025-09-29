using GoodHabits.Database;
using Microsoft.EntityFrameworkCore;
namespace GoodHabits.HabitService.Extensions;

public static class ServiceCollectionExtensions
{
    //The key point to understand from the preceding code is that the database connection string is set
    //    on a per-tenant basis, and that the tenants’ database is updated per the latest migrations when that
    //    tenant logs in to the app.
    //    This system takes much of the overhead of maintaining many databases away from the administrators.
    //    It is all handled automatically!
    public static IServiceCollection
        AddAndMigrateDatabases(this IServiceCollection services, IConfiguration config)
    {
        var options = services.GetOptions<TenantSettings>(nameof(TenantSettings));
        var defaultConnectionString = options.DefaultConnectionString;
        services.AddDbContext<GoodHabitsDbContext>(m =>
            m.UseSqlServer(e => e.MigrationsAssembly(
                typeof(GoodHabitsDbContext).Assembly.FullName)));
        var tenants = options.Tenants;
        foreach (var tenant in tenants)
        {
            string connectionString;
            if (string.IsNullOrEmpty(
                    tenant.ConnectionString))
            {
                connectionString = defaultConnectionString;
            }
            else
            {
                connectionString = tenant.ConnectionString;
            }
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GoodHabitsDbContext>();
            dbContext.Database.SetConnectionString(connectionString);
            if (dbContext.Database.GetMigrations().Count() > 0)
            {
                dbContext.Database.Migrate();
            }
        }
        return services;
    }
    public static T GetOptions<T>(this IServiceCollection
        services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);
        return options;
    }
}