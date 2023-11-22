using Hangfire;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.MODEL;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork uow;
        private ISEPrincipal _user;

        public HomeController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public ActionResult Index()
        {
            _user = (User as ISEPrincipal);

            if (_user.IsQRE() || _user.IsDeveloper() || System.Configuration.ConfigurationManager.AppSettings["TestRole"] == "QRE")
                return RedirectToAction("Index", "QRE");
            else if (_user.IsOperator() || System.Configuration.ConfigurationManager.AppSettings["TestRole"] == "Operator")
                return RedirectToAction("Index", "Operator");
            else
                return RedirectToAction("AccessDenied", "Home");
        }

        public ActionResult AccessDenied()
        {
            ViewBag.Roles = HttpContext.Items["Roles"];
            return View();
        }
    }
}