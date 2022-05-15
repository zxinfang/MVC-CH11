using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Serivces
{
    public class MessageDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢留言陣列
        public List<Message> GetDataList(ForPaging Paging,int A_Id)
        {
            List<Message> DataList = new List<Message>();
            SetMaxPaging(Paging, A_Id);
            DataList = GetAllDataList(Paging, A_Id);
            return DataList;
        }
        #endregion

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging Paging,int A_Id)
        {
            int row = 0;
            string sql = $@" select * from Message m inner join Members d on m.Account = d.Account where A_Id = '{A_Id}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    row++;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row)) / Paging.ItemNum);
            Paging.SetRightPage();
        }
        #endregion

        #region 取得Message資料
        public List<Message> GetAllDataList(ForPaging Paging,int A_Id)
        {
            List<Message> DataList = new List<Message>();
            string sql = $@" select m.*,d.Name from ( select row_number() over (order by M_Id) as sort, * from Message where A_Id = '{A_Id}') m
                           inner join Members d on m.Account = d.Account where m.sort between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch(Exception e)
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

        #region 新增留言
        public void InsertMessage(Message Data)
        {
            Data.M_Id = LastMessageFinder(Data.A_Id);
            string sql = $@" insert into Message(A_Id,M_Id,Account,Content,CreateTime) values('{Data.A_Id}','{Data.M_Id}',
                            '{Data.Account}','{Data.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}')";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 計算目前留言最新一筆M_Id
        public int LastMessageFinder(int A_Id)
        {
            int Id;
            string sql = $@" select top 1 * from Message where A_Id = '{A_Id}' order by M_Id desc";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["M_Id"]);
            }
            catch(Exception e)
            {
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 修改留言
        public void UpdateMessage(Message Data)
        {
            string sql = $@" update Message set Content ='{Data.Content}' where A_Id = '{Data.A_Id}' and M_Id = '{Data.M_Id}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 刪除留言
        public void DeleteMessage(int A_Id,int M_Id)
        {
            string sql = $@" delete from Message where A_Id = '{A_Id}' and M_Id = '{M_Id}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
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