
using MapsterMapper;
using Microsoft.OpenApi.Models;
using SurveyBasket.BLL.Services;
using SurveyBasket.Contract.Mapping;

namespace SurveyBasket.Api;
public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services ,IConfiguration configuration)
    {
        services.AddSwaggerServices();
        services.AddCors(
            options=>options.AddPolicy("allowAll"
            ,builder=>builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader())
            );
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
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

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SurveyBasket.Api",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Type 'Bearer {token}' below."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService,QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
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
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfigurations).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
