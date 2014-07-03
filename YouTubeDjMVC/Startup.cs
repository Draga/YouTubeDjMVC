using System.Data.Entity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using YouTubeDjMVC.Migrations;
using YouTubeDjMVC.Models;

[assembly: OwinStartupAttribute(typeof(YouTubeDjMVC.Startup))]
namespace YouTubeDjMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<VideoDbContext, Configuration>()); 

            ConfigureAuth(app);

            app.MapSignalR(new HubConfiguration() { EnableDetailedErrors = true });
        }
    }
}
