using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZMBusService.Startup))]
namespace ZMBusService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
