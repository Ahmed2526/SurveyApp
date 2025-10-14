using Serilog;
using SurveyAppAPI.Dependencies;
using SurveyAppAPI.MiddleWares;

namespace SurveyAppAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.RegisterBuiltInService();

            //Adding DBContext & Identity
            builder.Services.RegisterContext(builder.Configuration);

            //Register Mapster & Mapster configuration
            builder.Services.RegisterMapster();

            //Register Custom Services
            builder.Services.RegisterCustomServices(builder.Configuration);

            // Add Authentication
            builder.Services.RegisterJwtBearer(builder.Configuration);
            builder.Services.AddAuthorization();


            // Read Serilog configuration from appsettings.json
            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(builder.Configuration)
                 .Enrich.FromLogContext()
                 .CreateLogger();

            // Replace default logging
            builder.Host.UseSerilog();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
