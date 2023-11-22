using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WUFT.NET.Util;
using WUFT.DATA;
using WUFT.EF;
using WUFT.NET.Mappers;

namespace WUFT.NET
{
    public class MvcApplication: System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer(new WUFT.EF.WUFTDbContextSeedData());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthorizeRequest(object sender, EventArgs E)
        {
            if (User.Identity.IsAuthenticated)
            {
                Context.User = new ISEPrincipal(HttpContext.Current.User.Identity, new UnitOfWork());
            }
        }
        protected void Session_END(object sender, EventArgs E)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}
