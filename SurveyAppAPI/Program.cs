using BussinessLogicLater.IService;
using Hangfire;
using SurveyAppAPI.Dependencies;
using SurveyAppAPI.Middlewares;
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

            //Register Redis
            builder.Services.RegisterRedis(builder.Configuration);

            //Register Serilog
            builder.RegisterSerilog();

            //Register Hangfire
            builder.RegisterHangfire();


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

            //Handle Hangfire
            if (app.Environment.IsDevelopment())
                app.UseDevelopmentHangfireDashboard();

            else
                app.UseSecureHangfireDashboard(builder.Configuration, app.Environment);


            //to do
            RecurringJob.AddOrUpdate<INotificationJobService>("daily-Polls-Updates-job", job
                => job.SendPollsDailyUpdateAsync(), Cron.Daily(0, 0),
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });


            app.MapControllers();

            app.Run();
        }
    }
}
