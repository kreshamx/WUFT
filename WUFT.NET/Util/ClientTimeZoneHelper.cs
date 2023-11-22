using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.Util
{
    public class ClientTimeZoneHelper
    {
        public static string ConvertToLocalTimeAndFormat(DateTime dt, string format)
        {
            var o = HttpContext.Current.Session["tzo"];

            var tzo = o == null ? 0 : Convert.ToDouble(o);

            string s = String.Empty;

            if (dt != DateTime.MinValue)
            {
                dt = dt.AddMinutes(-1 * tzo);

                s = dt.ToString(format);

                if (tzo == 0)
                    s += " GMT";
            }

            return s;
        }
    }
}