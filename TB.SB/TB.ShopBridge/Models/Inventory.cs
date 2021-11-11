using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Npgsql;
using System.Configuration;
using System.Text;
using NpgsqlTypes;

namespace TB.ShopBridge.Models
{
    public class Inventory
    {
        //variables
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal Itemprice { get; set; }
        public decimal Itemquantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string ItemPurchaseDate { get; set; }

        public string Message { get; set; }

        public int Status { get; set; }
        public string Button { get; set; } = "Save";

        public List<Inventory> listSavedInventory = new List<Inventory>();


        NpgsqlConnection con = new NpgsqlConnection();



        /// <summary>
        ///Fetch the saved records
        /// </summary>
        /// <param name="objinv"></param>
        /// <returns></returns>
        public Inventory GetInventoryDetails(Inventory objinv)
        {
            PGSqlConnection objCon = new PGSqlConnection(ConfigurationManager.AppSettings["dbpwd"]);
            DataTable dt = new DataTable();        
            NpgsqlCommand cmd = new NpgsqlCommand();
            try
            {
                cmd = new NpgsqlCommand("fetch_inventory_grid");
                cmd.Parameters.AddWithValue("itemid", objinv.ItemId);
                dt = objCon.FetchDataTable(cmd);
                if (dt.Rows.Count > 0 && objinv.ItemId != 0)
                {
                    objinv.ItemId = Convert.ToInt32(dt.Rows[0]["t_it_id"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["t_it_id"]);
                    objinv.ItemName = Convert.ToString(dt.Rows[0]["t_it_name"]) == null ? "" : Convert.ToString(dt.Rows[0]["t_it_name"]);
                    objinv.Itemquantity = Convert.ToDecimal(dt.Rows[0]["t_it_quantity"]) == 0 ? 0 : Convert.ToDecimal(dt.Rows[0]["t_it_quantity"]);
                    objinv.Itemprice = Convert.ToDecimal(dt.Rows[0]["t_it_price"]) == 0 ? 0 : Convert.ToDecimal(dt.Rows[0]["t_it_price"]);
                    objinv.ItemDescription = Convert.ToString(dt.Rows[0]["t_it_description"]) == null ? "" : Convert.ToString(dt.Rows[0]["t_it_description"]);
                    objinv.ItemPurchaseDate = Convert.ToString(dt.Rows[0]["t_it_purchase_date"]) == null ? "" : Convert.ToString(dt.Rows[0]["t_it_purchase_date"]);
                    objinv.TotalAmount = Convert.ToDecimal(Convert.ToInt32(dt.Rows[0]["t_it_quantity"]) * Convert.ToDecimal(dt.Rows[0]["t_it_price"]));
                    objinv.Button = "Update";

                }
                else
                {
                    objinv.listSavedInventory = (from dr in dt.AsEnumerable()
                                                   select new Inventory()
                                                   {
                                                       ItemId = Convert.ToInt32(dr["t_it_id"]),
                                                       ItemName = Convert.ToString(dr["t_it_name"]),
                                                       Itemquantity = Convert.ToDecimal(dr["t_it_quantity"]),
                                                       Itemprice = Convert.ToDecimal(dr["t_it_price"]),
                                                       ItemDescription = Convert.ToString(dr["t_it_description"]),
                                                       ItemPurchaseDate = Convert.ToString(dr["t_it_purchase_date"]),
                                                       TotalAmount = Convert.ToDecimal(Convert.ToInt32(dr["t_it_quantity"]) * Convert.ToDecimal(dr["t_it_price"]))
                                                   }).ToList();
                }



                return objinv;
            }
            catch (Exception ex)
            {
                objinv.Status = -1;
                objinv.Message = "Warning:Error Occured...Something wrong!!!";
                throw ex;
            }
            finally
            {
                con.Close();
            }

        }





        /// <summary>
        /// To Save Inventory details
        /// </summary>
        /// <param name="objinv"></param>
        /// <returns></returns>
        public Inventory SaveInventoryDetails(Inventory objinv)
        {
            PGSqlConnection objCon = new PGSqlConnection(ConfigurationManager.AppSettings["dbpwd"]);
            NpgsqlCommand cmd = new NpgsqlCommand();
            string[] iReturn = new string[2];
            try
            {
                objCon.BeginTransaction();
                cmd = new NpgsqlCommand("sp_save_inventory_details");
                cmd.Parameters.AddWithValue("t_it_id", Convert.ToInt32(objinv.ItemId)==0?0: Convert.ToInt32(objinv.ItemId));
                cmd.Parameters.AddWithValue("t_it_name", Convert.ToString(objinv.ItemName) == null ? "" : Convert.ToString(objinv.ItemName));
                cmd.Parameters.AddWithValue("t_it_quantity", Convert.ToDecimal(objinv.Itemquantity) == 0 ? 0 : Convert.ToDecimal(objinv.Itemquantity));
                cmd.Parameters.AddWithValue("t_it_price", Convert.ToDecimal(objinv.Itemprice) == 0 ? 0 : Convert.ToDecimal(objinv.Itemprice));
                cmd.Parameters.AddWithValue("t_it_description", Convert.ToString(objinv.ItemDescription) == null ? "" : Convert.ToString(objinv.ItemDescription));
                cmd.Parameters.AddWithValue("t_it_purchase_date", Convert.ToString(objinv.ItemPurchaseDate) == null ? "" : Convert.ToString(objinv.ItemPurchaseDate));
                cmd.Parameters.Add("statusid", NpgsqlDbType.Integer);
                cmd.Parameters.Add("status", NpgsqlDbType.Text);
                cmd.Parameters["statusid"].Direction = ParameterDirection.Output;
                cmd.Parameters["status"].Direction = ParameterDirection.Output;
                iReturn[0] = "statusid";
                iReturn[1] = "status";
                iReturn = objCon.Execute(cmd, iReturn, 2);

                objinv.Status = Convert.ToInt32(iReturn[0]);
                objinv.Message = Convert.ToString(iReturn[1]);

                objCon.CommitTransaction();
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                objinv.Status = -1;
                objinv.Message = "Warning:Error Occured...Something wrong!!!";
                
                throw ex;
            }
            return objinv;
        }










        /// <summary>
        /// Remove The Item by itemid (Pkid);
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public Inventory RemoveItem(Inventory objinv)
        {
            PGSqlConnection objCon = new PGSqlConnection(ConfigurationManager.AppSettings["dbpwd"]);
            NpgsqlCommand cmd = new NpgsqlCommand();
            string[] iReturn = new string[2];
            try
            {
                cmd = new NpgsqlCommand("sp_delete_inventory_details");
                cmd.Parameters.AddWithValue("t_it_id", Convert.ToInt32(objinv.ItemId) == 0 ? 0 : Convert.ToInt32(objinv.ItemId));
                cmd.Parameters.Add("statusid", NpgsqlDbType.Text);
                cmd.Parameters["statusid"].Direction = ParameterDirection.Output;
                iReturn[0] = "statusid";
                iReturn = objCon.Execute(cmd, iReturn, 1);
            }
            catch (Exception ex)
            {
                objinv.ItemId = -1;
                objinv.Message = "Warning:Error Occured...Something wrong!!!";
              
                throw ex;
            }

            return objinv;
        }

    }

    public class PGSqlConnection
    {

        NpgsqlConnection con = new NpgsqlConnection();
        NpgsqlTransaction trans;
        bool blTransaction = false;
        public PGSqlConnection(string Password)
        {
            try
            {
                string PassWord = string.Empty;

                string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + Decrypt(Password);
                con.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=DBNAME;User Id=postgres;Password ="";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Decrypt(string pwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(pwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }


        public DataTable FetchDataTable(NpgsqlCommand cmd)
        {
            NpgsqlDataReader dreader = null;
            try
            {
                DataTable dtable = new DataTable();
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                dreader = cmd.ExecuteReader();
                dtable.Load(dreader);
                
                dreader.Close();
                if (blTransaction == false)
                {
                    con.Close();
                }
                return dtable;
            }
            catch (Exception ex)
            {
                con.Close();
                dreader.Close();
                throw ex;
            }
        }


        public NpgsqlTransaction BeginTransaction()
        {
            if (con.State == ConnectionState.Open)
            {
                if (blTransaction == false)
                {
                    trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    blTransaction = true;
                }

            }
            return trans;
        }
        public void CommitTransaction()
        {
            if (con.State == ConnectionState.Open)
            {
                if (blTransaction == true)
                    trans.Commit();
                con.Close();
            }
        }
        public void RollBackTrans()
        {
            if (con.State == ConnectionState.Open)
            {
                if (blTransaction == true)
                    trans.Rollback();
                con.Close();
            }
        }


        public string[] Execute(NpgsqlCommand cmd, string[] strArray, int n)
        {
            try
            {
                string[] strResult = new string[n];
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.ExecuteNonQuery();
                for (int i = 0; i < n; i++)
                {
                    strResult[i] = Convert.ToString(cmd.Parameters[strArray[i]].Value);
                }
                if (blTransaction == false)
                {
                    con.Close();
                }
                return strResult;
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }

        }


    }

}