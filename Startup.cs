using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(hotel.Startup))]
namespace hotel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
