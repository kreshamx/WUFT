using System;
using System.Reflection;
using System.Web.Mvc;

namespace WUFT.NET.Util
{
    public static class VersionHelper
    {
        /// <summary>
        ///  Return the Current Version from the AssemblyInfo.cs file
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns> 
        public static string CurrentVersion(this HtmlHelper helper)
        {
            try
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return version.Major + "." + version.Minor + "." + version.Build;
            }
            catch (Exception) { return "?.?.?"; }
        }
    }
}
