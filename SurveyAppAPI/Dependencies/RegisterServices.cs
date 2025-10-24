using BussinessLogicLater.IService;
using BussinessLogicLater.MappingProfile;
using BussinessLogicLater.Service;
using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace SurveyAppAPI.Dependencies
{
    public static class RegisterServices
    {
        public static void RegisterBuiltInService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public static void RegisterContext(this IServiceCollection services, ConfigurationManager Configuration)
        {
            //Adding DBContext 
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration
                    .GetConnectionString("DefaultConnection")));

            //Add Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void RegisterMapster(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            MapsterConfig.RegisterMappings(config);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
        }

        public static void RegisterJwtBearer(this IServiceCollection services, ConfigurationManager Configuration)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };

            });
        }

        public static void RegisterRedis(this IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // your Redis server
            });
        }

        public static void RegisterSerilog(this WebApplicationBuilder builder)
        {
            // Read Serilog configuration from appsettings.json
            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(builder.Configuration)
                 .Enrich.FromLogContext()
                 .CreateLogger();

            // Replace default logging
            builder.Host.UseSerilog();
        }

        public static void RegisterHangfire(this WebApplicationBuilder builder)
        {
            var hangfireConn = builder.Configuration.GetConnectionString("HangfireConnection");

            builder.Services.AddHangfire(config => config
                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(hangfireConn, new SqlServerStorageOptions
                  {
                      CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                      SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                      QueuePollInterval = TimeSpan.FromSeconds(15),
                      UseRecommendedIsolationLevel = true,
                      DisableGlobalLocks = true
                  }
            ));

            // Add Hangfire server (use Worker process in prod if desired)
            builder.Services.AddHangfireServer();

        }

        public static void RegisterCustomServices(this IServiceCollection services, ConfigurationManager Configuration)
        {
            services.Configure<JwtSettings>(
            Configuration.GetSection("JwtSettings"));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IVotesRepository, VotesRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPollsService, PollsService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVotesService, VotesService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<INotificationJobService, NotificationJobService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddTransient<IEmailService, SendGridEmailService>();

        }


    }
}
