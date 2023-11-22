using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Configuration;

[assembly: OwinStartup(typeof(WUFT.NET.Startup))]
namespace WUFT.NET
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString);

            //app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new RestrictiveAuthorizationFilter() }
            });
            app.UseHangfireServer();
        }

        public class RestrictiveAuthorizationFilter : IAuthorizationFilter
        {
            public bool Authorize(IDictionary<string, object> owinEnvironment)
            {
                // In case you need an OWIN context, use the next line,
                // `OwinContext` class is the part of the `Microsoft.Owin` package.
                var context = new OwinContext(owinEnvironment);

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return context.Authentication.User.Identity.IsAuthenticated;
            }
        }

    }
}
