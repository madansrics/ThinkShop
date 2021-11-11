using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TB.ShopBridge.Startup))]
namespace TB.ShopBridge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
