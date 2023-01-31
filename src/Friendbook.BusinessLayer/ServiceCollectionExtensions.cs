using Friendbook.BusinessLayer.Services;
using Friendbook.BusinessLayer.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Friendbook.BusinessLayer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ISecurityService, SecurityService>();

        return services;
    }
}
