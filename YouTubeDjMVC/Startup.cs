using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YouTubeDjMVC.Startup))]
namespace YouTubeDjMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR(new HubConfiguration() { EnableDetailedErrors = true });
        }
    }
}
