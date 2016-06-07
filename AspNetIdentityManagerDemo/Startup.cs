using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspNetIdentityManagerDemo.Startup))]
namespace AspNetIdentityManagerDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
