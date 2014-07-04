using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ContosoDemoWeb.Startup))]
namespace ContosoDemoWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
