using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Serivces
{
    public class GuestbooksDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢陣列資料
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            SetMaxPaging(Paging, Search);
            DataList = GetAllDataList(Paging, Search);
            return DataList;
        }
        #endregion

        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging,string Search)
        {
            int Row = 0;
            string sql = string.Empty;
            if (!string.IsNullOrEmpty(Search))
            {
                sql = $@" select * from Guestbooks Where Content like '%{Search}%' or Reply like '%{Search}%' ";
            }
            else
            {
                sql = $@" select * from Guestbooks ";   
            }
            
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #region 搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging,string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = string.Empty;
            if (!string.IsNullOrEmpty(Search))
            {
                sql = $@" select m.*,d.Name,d.IsAdmin from (select row_number() over(order by Id) as sort,* from Guestbooks where Name like '%{Search}%' or 
                    Content like '%{Search}%' or Reply like '%{Search}%' ) m inner join Members d on m.Account = d.Account
                    Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
                
            }
            else
            {
                sql = $@" select  m.*,d.Name,d.IsAdmin from (select row_number() over(order by Id) as sort,* from Guestbooks ) m inner join Members d on m.Account = d.Account
                      Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            }
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        #endregion

        #region 新增資料
        public void InsertGuestbooks(Guestbooks newData)
        {

            string sql =$@" INSERT INTO Guestbooks(Account,Content,CreateTime)
                        VALUES ( '{newData.Account}','{newData.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' ) ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 查詢一筆資料
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();
            string sql = $@" select * from Guestbooks m inner join Members on m.Account = d.Account  where Id = {Id} ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }
                Data.Member.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 修改留言
        public void UpdateGuestbooks(Guestbooks UpdateData)
        {
            string sql = $@" update Guestbooks set Account = '{UpdateData.Account}', Content = '{UpdateData.Content}' where Id = {UpdateData.Id} ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 回覆留言
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            string sql = $@" update Guestbooks set Reply = '{ReplyData.Reply}',
                         ReplyTime = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' where Id = {ReplyData.Id} ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 檢查相關
        public bool CheckUpdate(int Id)
        {
            Guestbooks Data = GetDataById(Id);
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion

        #region 刪除資料
        public void DeleteGuestbooks(int Id)
        {
            string sql = $@" Delete from Guestbooks where Id = {Id} ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

    }
}