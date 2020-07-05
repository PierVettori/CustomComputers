using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CustomComputers.Startup))]
namespace CustomComputers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
