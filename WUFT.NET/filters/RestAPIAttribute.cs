using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WUFT.NET.filters
{
    public class RestAPIAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContextBase httpContext = filterContext.HttpContext;
            if (httpContext.CurrentNotification != RequestNotification.AuthenticateRequest)
            {

                if (httpContext.Request.InputStream == null || httpContext.Request.InputStream.Length == 0)
                {
                    throw new InvalidOperationException("Invaild POST received.");
                }

                Stream httpBodyStream = httpContext.Request.InputStream;

                if (httpBodyStream.Length > int.MaxValue)
                {
                    throw new ArgumentException("HTTP InputStream too large.");
                }

                int streamLength = Convert.ToInt32(httpBodyStream.Length);
                byte[] byteArray = new byte[streamLength];
                const int startAt = 0;

                /*
                 * Copies the stream into a byte array
                 */
                httpBodyStream.Read(byteArray, startAt, streamLength);

                /*
                 * Convert the byte array into a string
                 */
                string xmlBody = Encoding.UTF8.GetString(byteArray);

                //Pass the xmlBody to the controller via the ViewBag
                filterContext.Controller.ViewBag.XMLBody = xmlBody;


                //Sends XML Data To Model so it could be available on the ActionResult
                base.OnActionExecuting(filterContext);
            }
        }
    }
}