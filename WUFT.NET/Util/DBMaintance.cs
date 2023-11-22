using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WUFT.DATA;

namespace WUFT.NET.Util
{
    public static class DBMaintance
    {
        public static void DeleteOldUploads()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString;
            SqlCommand sql = new SqlCommand("DELETE FROM QRELoads WHERE QRELoadDateTime <= DATEADD(HOUR,-1,GETUTCDATE())", conn);
            sql.CommandType = System.Data.CommandType.Text;
            sql.CommandTimeout = 60;
            conn.Open();
            sql.ExecuteNonQuery();
            conn.Close();
        }
    }
}