namespace SurveyBasket.Api;
public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddSwaggerServices();
        services.AddBusinessServices();
        services.AddValidationServices();
        services.AddMappingServices();
        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        return services;
    }

    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(SurveyBasket.Contract.AssemblyMarker).Assembly);
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        return services;
    }

    public static IServiceCollection AddMappingServices(this IServiceCollection services)
    {
        services.AddMapster();
        return services;
    }
}
