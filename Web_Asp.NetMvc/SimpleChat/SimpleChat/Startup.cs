using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimpleChat.Startup))]
namespace SimpleChat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
