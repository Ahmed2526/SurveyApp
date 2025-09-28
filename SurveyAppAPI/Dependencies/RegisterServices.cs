using BussinessLogicLater.IService;
using BussinessLogicLater.MappingProfile;
using BussinessLogicLater.Service;
using DataAccessLayer.Data;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            services.AddIdentity<ApplicationUser, IdentityRole>()
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

        public static void RegisterCustomServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPollsService, PollsService>();
        }


    }
}
