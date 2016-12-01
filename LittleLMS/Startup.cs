using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LittleLMS.Startup))]
namespace LittleLMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
