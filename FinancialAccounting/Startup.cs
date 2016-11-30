using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancialAccounting.Startup))]
namespace FinancialAccounting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
