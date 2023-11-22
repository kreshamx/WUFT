using Glimpse.Mvc.AlternateType;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

public class Class1 : ActionFilterAttribute
{
	public Class1()
	{

	}

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        string actionName = filterContext.ActionDescriptor.ActionName.ToString();
        DateTime dateTime = DateTime.Now;
        //System.IO.TextWriter textWriter = new StreamWriter(filterContext.HttpContext.Server.MapPath("~/log.txt"));
        //textWriter.WriteLine("Started action " + actionName + " time " + dateTime.ToString());
        string text = "Started action " + actionName + " time " + dateTime.ToString();
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"),text + Environment.NewLine);
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), "--------" + Environment.NewLine);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        string actionName = filterContext.ActionDescriptor.ActionName.ToString();
        DateTime dateTime = DateTime.Now;
        //System.IO.TextWriter textWriter = new StreamWriter(filterContext.HttpContext.Server.MapPath("~/log.txt"));
        //textWriter.WriteLine("Ended action " + actionName + " time " + dateTime.ToString());
        string text = "Ended action " + actionName + " time " + dateTime.ToString();
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), text + Environment.NewLine);
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), "--------" + Environment.NewLine);
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        string actionName = "Result started";
        DateTime dateTime = DateTime.Now;
        //System.IO.TextWriter textWriter = new StreamWriter(filterContext.HttpContext.Server.MapPath("~/log.txt"));
        //textWriter.WriteLine("Ended action " + actionName + " time " + dateTime.ToString());
        string text = "Ended Result " + actionName + " time " + dateTime.ToString();
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), text + Environment.NewLine);
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), "--------" + Environment.NewLine);
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        string actionName = "Result Ended";
        DateTime dateTime = DateTime.Now;
        //System.IO.TextWriter textWriter = new StreamWriter(filterContext.HttpContext.Server.MapPath("~/log.txt"));
        //textWriter.WriteLine("Ended action " + actionName + " time " + dateTime.ToString());
        string text = "Ended Result " + actionName + " time " + dateTime.ToString();
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), text + Environment.NewLine);
        System.IO.File.AppendAllText(filterContext.HttpContext.Server.MapPath("~/log.txt"), "--------" + Environment.NewLine);
    }
}
