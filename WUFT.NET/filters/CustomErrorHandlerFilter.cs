using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WUFT.NET.Web.Filters
{
    public class CustomErrorHandlerFilter : HandleErrorAttribute
    {
//        public override void OnException(ExceptionContext filterContext)
//        {
//            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://isewebservices.intel.com/api/nlog/save");
//            httpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
//            httpWebRequest.PreAuthenticate = true;
//            httpWebRequest.ContentType = "application/json";
//            httpWebRequest.Method = "POST";

//            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
//            {
//                string json = new JavaScriptSerializer().Serialize(new
//                {
//                    ApplicationName = "WUFT.NET",
//                    DateLogged = DateTime.UtcNow,
//#if DEBUG
//                    Level = "Debug",
//#else
//                    Level = "Error",
//#endif

//                    HostName = filterContext.HttpContext.Server.MachineName,
//                    User = filterContext.HttpContext.User.Identity.Name,
//                    LoggerName = "Lager", // Still displays as NLog :(
//                    Message = filterContext.Exception.ToString().Length.ToString(),
//                    Metadata = "",
//                    Source = filterContext.Exception.Source,
//                    TypeName = "N/A",
//                    StackTrace = filterContext.Exception.StackTrace,
//                    Error = filterContext.Exception.ToString() // "Error = filterContext.Exception" causes circular reference?
//                });

//                streamWriter.Write(json);
//            }

//            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
//            {
//                var result = streamReader.ReadToEnd();
//            }
//        }
    }
}