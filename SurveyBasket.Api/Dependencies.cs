
namespace SurveyBasket.Api;
public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services ,IConfiguration configuration)
    {
        services.AddSwaggerServices();
        services.AddBusinessServices();
        services.AddValidationServices();
        services.AddMappingServices();
        services.AddAuth(configuration);
        services.AddConnectionString(configuration);
        return services;
    }
    public static IServiceCollection AddAuth(this IServiceCollection services ,IConfiguration configuration )
    {
        services.AddIdentity<ApplicationUser ,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
       // services.Configure<JwtOption>(configuration.GetSection(JwtOption.SectionName));
       services.AddOptions<JwtOption>().
            BindConfiguration(JwtOption.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        var jwtSetting=configuration.GetSection(JwtOption.SectionName).Get<JwtOption>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })

            .AddJwtBearer(o =>
            {
                o.SaveToken=true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSetting?.Issuer,
                    ValidAudience = jwtSetting?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting?.Key!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        return services;
    }
    public static IServiceCollection AddConnectionString (this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(ConnectionString));
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
