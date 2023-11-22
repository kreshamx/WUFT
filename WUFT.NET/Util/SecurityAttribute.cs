using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace WUFT.NET.Util
{
    public class ISESecurity : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "Action", "AccessDenied" }, { "Controller", "Home" }, { "Area", "" } });
        }
    }
}