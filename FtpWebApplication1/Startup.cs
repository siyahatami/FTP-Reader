using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FtpWebApplication1.Startup))]
namespace FtpWebApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
