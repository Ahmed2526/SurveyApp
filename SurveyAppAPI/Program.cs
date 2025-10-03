using SurveyAppAPI.Dependencies;

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


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
