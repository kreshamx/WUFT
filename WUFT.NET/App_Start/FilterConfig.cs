using WUFT.NET.Web.Filters;
using System.Web.Mvc;
using WUFT.NET.filters;
using WUFT.NET.Util;

namespace WUFT.NET
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RoleAuthenticationFilter());
            //filters.Add(new ISESecurity());
            filters.Add(new ClientTimeZoneAttribute());
        }
    }
}