using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace FCP.MVVM.SQL
{
    public static class SQLQuery
    {
        private static string _SqlInfo = "Server=.;DataBase=OnCube;User ID=sa;Password=jvm5822511";
        private static SqlConnection conn { get; set; }
        private static SqlCommand com { get; set; }
        private static SqlDataReader dr { get; set; }
        public static void NonQuery(string command)
        {
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            com.ExecuteNonQuery();
            DisposeAndClose();
        }

        public static int GetNonQueryCountsInt(string command)
        {
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            int result = com.ExecuteNonQuery();
            DisposeAndClose();
            return result;
        }

        public static int GetFirstOneDataInt(string command)
        {
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            int result = Convert.ToInt32(com.ExecuteScalar());
            DisposeAndClose();
            return result;
        }

        public static decimal GetFirstOneDataDecimal(string command)
        {
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            decimal result = Convert.ToDecimal(com.ExecuteScalar());
            DisposeAndClose();
            return result;
        }

        public static string GetFirstOneDataString(string command)
        {
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            string result = Convert.ToString(com.ExecuteScalar());
            DisposeAndClose();
            return result;
        }

        public static List<string> GetListString(string command, string column)
        {
            List<string> list = new List<string>();
            conn = new SqlConnection(_SqlInfo);
            conn.Open();
            com = new SqlCommand(command, conn);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                list.Add(dr[column].ToString());
            }
            DisposeAndClose();
            return list;
        }

        private static void DisposeAndClose()
        {
            if (dr != null && !dr.IsClosed)
                dr.Close();
            com.Dispose();
            conn.Close();
        }
    }
}
