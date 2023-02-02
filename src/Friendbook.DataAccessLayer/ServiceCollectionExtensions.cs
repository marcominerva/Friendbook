using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Friendbook.DataAccessLayer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, Action<ApplicationDbContextOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var applicationOptions = new ApplicationDbContextOptions();
        configuration.Invoke(applicationOptions);

        services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
        {
            options.UseSqlServer(applicationOptions.ConnectionString);
        });

        return services;
    }
}
