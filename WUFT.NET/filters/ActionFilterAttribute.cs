using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace WUFT.NET.filters
{
    public class RoleAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //allow users w/ all WUFT roles
            string[] roles = System.Web.Security.Roles.GetRolesForUser(filterContext.HttpContext.User.Identity.Name);
            if (!(roles.Contains(@"AMR\ISE Logistics_WUFT_QRE") || roles.Contains(@"AMR\ISE Logistics_WUFT_Operator") || roles.Contains(@"AMR\ISE Logistics_WUFT_Admin") || roles.Contains(@"AMR\ISE Logistics_ISE_Developer")))
                new ApplicationException("You do not have access.");
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }
    }

    public class ClientTimeZoneAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["timeZoneOffset"];

            Int16 n = 0;

            var tzo = (cookie != null && Int16.TryParse(cookie.Value, out n) ? n : 0);

            filterContext.HttpContext.Session["tzo"] = tzo;

            base.OnActionExecuting(filterContext);
        }
    }
}