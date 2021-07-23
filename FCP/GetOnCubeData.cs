using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;

namespace FCP
{
    class GetOnCubeData
    {
        private string _loginInfo = "server=.;user id=sa;password=jvm5822511;database=OnCube";
        private SqlConnection conn;
        private SqlCommand com;
        private SqlDataReader dr;

        public List<string> Get_Admin_Code_For_Multi(string AdminCode)
        {
            List<string> list = new List<string>();
            Connect_Sql_Read($@"SELECT
	                                LEFT(CONVERT(VARCHAR,A.StandardTime,108), 5) AS Time
                                FROM AdminTime A,
                                (
                                    SELECT
	                                    RawID
                                    FROM AdminTime A
                                    WHERE A.AdminCode='{AdminCode}' AND DeletedYN=0 AND A.UpperAdminTimeID is null
                                ) #tmp
                                WHERE A.UpperAdminTimeID=#tmp.RawID");
            while (dr.Read())
            {
                if (dr["Time"].ToString().Trim() == "")
                    continue;
                list.Add(dr["Time"].ToString());
            }
            Dispose_All_Sql_Object();
            return list;
        }

        public bool Is_Admin_Code_For_Multi_Created(string AdminCode)
        {
            Connect_Sql($@"SELECT
	                           TOP 1 COUNT(RawID)
                           FROM AdminTime A
                           WHERE A.AdminCode='{AdminCode}' AND DeletedYN=0 AND A.UpperAdminTimeID is null");
            bool Result = Convert.ToInt32(com.ExecuteScalar().ToString()) > 0;
            Dispose_All_Sql_Object();
            return Result;
        }

        public bool Is_Admin_Code_For_Combi_Created(string AdminCode)
        {
            Connect_Sql($@"SELECT
	                           TOP 1 COUNT(RawID)
                           FROM AdminTime A
                           WHERE A.AdminCode='{AdminCode}' AND DeletedYN=0");
            bool Result = Convert.ToInt32(com.ExecuteScalar().ToString()) > 0;
            Dispose_All_Sql_Object();
            return Result;
        }

        public List<string> Get_Medicine_Code_Only_Got_Canister()
        {
            List<string> list = new List<string>();
            Connect_Sql_Read(@"SELECT
                                   A.Mnemonic
                               , ISNULL(C.MultiMnemonic, '') AS ExtraMnemonic
                               FROM Item A
                               INNER JOIN InventoryContainer B ON A.DeletedYN = 0 AND A.UseYN = 1 AND B.ContainerID IS NOT NULL AND A.RawID = B.ItemID
                               LEFT OUTER JOIN ItemMultiCode C ON A.RawID = C.ItemID AND C.DeletedYN = 0");
            while(dr.Read())
            {
                list.Add(dr["Mnemonic"].ToString());
                if (dr["ExtraMnemonic"].ToString().Length > 0)
                    list.Add(dr["ExtraMnemonic"].ToString());
            }
            Dispose_All_Sql_Object();
            return list;
        }

        public List<string> Get_All_Medicine_Code()
        {
            List<string> list = new List<string>();
            Connect_Sql_Read(@"SELEC
                                    A.Mnemonic
                                , ISNULL(B.MultiMnemonic, '') AS ExtraMnemonic
                                FROM Item A
                                LEFT OUTER JOIN ItemMultiCode B ON A.RawID = B.ItemID AND B.DeletedYN = 0
                                WHERE A.DeletedYN = 0 AND A.UseYN = 1");
            while (dr.Read())
            {
                list.Add(dr["Mnemonic"].ToString());
                if (dr["ExtraMnemonic"].ToString().Length > 0)
                    list.Add(dr["ExtraMnemonic"].ToString());
            }
            Dispose_All_Sql_Object();
            return list;
        }

        public List<string> Get_Medicine_Code_If_Weight_Is_10_Gram()
        {
            List<string> list = new List<string>();
            Connect_Sql_Read(@"SELECT
	                               B.Mnemonic
                               ,	ISNULL(D.MultiMnemonic,'') AS ExtraMnemonic
                               FROM Medicine A
                               INNER JOIN Item B ON A.RawID=B.RawID AND A.DeletedYN=0 AND A.WeightMedicine=10
                               LEFT OUTER JOIN ItemMultiCode D ON B.RawID=D.ItemID AND D.DeletedYN=0");
            while(dr.Read())
            {
                list.Add(dr["Mnemonic"].ToString());
                if (dr["ExtraMnemonic"].ToString().Length > 0)
                    list.Add(dr["ExtraMnemonic"].ToString());
            }
            Dispose_All_Sql_Object();
            return list;
        }

        private void Dispose_All_Sql_Object()
        {
            if (dr != null)
                dr.Close();
            com.Dispose();
            conn.Close();
        }

        private void Connect_Sql(string execute)
        {
            conn = new SqlConnection(_loginInfo);
            conn.Open();
            com = new SqlCommand(execute, conn);
        }

        private void Connect_Sql_Read(string execute)
        {
            conn = new SqlConnection(_loginInfo);
            conn.Open();
            com = new SqlCommand(execute, conn);
            dr = com.ExecuteReader();
        }
    }
}