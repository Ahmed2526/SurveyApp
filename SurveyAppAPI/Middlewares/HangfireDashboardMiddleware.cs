using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;

namespace SurveyAppAPI.Middlewares
{
    public static class HangfireDashboardMiddleware
    {
        public static IApplicationBuilder UseSecureHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
        {
            // In production, require SSL
            bool requireSsl = !env.IsDevelopment();

            var options = new DashboardOptions
            {
                DashboardTitle = "SurveyApp Background Jobs",
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(
                        new BasicAuthAuthorizationFilterOptions
                        {
                            RequireSsl = requireSsl,
                            SslRedirect = requireSsl,
                            LoginCaseSensitive = false,
                            Users = new[]
                            {
                                new BasicAuthAuthorizationUser
                                {
                                    Login = configuration["Hangfire:Username"],
                                    PasswordClear = configuration["Hangfire:DashboardPass"]
                                }
                            }
                        })
                }
            };

            app.UseHangfireDashboard("/hangfire", options);

            return app;
        }

        public static IApplicationBuilder UseDevelopmentHangfireDashboard(this IApplicationBuilder app)
        {
            var options = new DashboardOptions
            {
                DashboardTitle = "SurveyApp Development Jobs",
                IsReadOnlyFunc = _ => false, // full control in dev               
            };

            app.UseHangfireDashboard("/h", options);
            return app;
        }



    }

}
