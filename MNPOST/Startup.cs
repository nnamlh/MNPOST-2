using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MNPOST.Startup))]
namespace MNPOST
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
