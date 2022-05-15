using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Serivces
{
    public class ArticleDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢一筆資料
        public Article GetArticleDataById(int A_Id)
        {
            Article Data = new Article();
            string sql = $@" select * from Article m inner join Members d on m.Account = d.Account where m.A_Id = '{A_Id}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Title = dr["Title"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.Member.Image = dr["Image"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Watch = Convert.ToInt32(dr["Watch"]);
            }
            catch(Exception e)
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

        #region 查詢陣列資料
        public List<Article> GetDataList(ForPaging Paging,string Search,string Account)
        {
            List<Article> DataList = new List<Article>();
            SetMaxPaging(Paging, Search,Account);
            DataList = GetAllDataList(Paging, Search,Account);
            return DataList;
        }
        #endregion

        #region 搜尋資料
        public List<Article> GetAllDataList(ForPaging paging, string Search,string Account)
        {
            string sql;
            List<Article> DataList = new List<Article>();
            if (!string.IsNullOrWhiteSpace(Search))
            {
                sql = $@" select m.*,d.Name from (select row_number() over(order by A_Id) as sort,* from Article where (Title like '%{Search}%'
                          or Content like '%{Search}%' ) and Account ='{Account}') m inner join Members d on m.Account = d.Account 
                          where m.sort between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum};";
            }
            else
            {
                sql = $@" select m.*,d.Name from(select row_number() over(order by A_Id) as sort,* from Article where Account ='{Account}') m
                        inner join Members d on m.Account = d.Account where m.sort between { (paging.NowPage - 1) * paging.ItemNum + 1}
                        and { paging.NowPage * paging.ItemNum}";
            }
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();   
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
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

        #region 設定最大頁數
        public void SetMaxPaging(ForPaging paging,string Search,string Account)
        {
            string sql;
            int row = 0;
            if (!string.IsNullOrEmpty(Search))
            {
                sql = $@" select * from Article m inner join Members d on m.Account = d.Account where ( Title like '%{Search}%' or Content like '%{Search}%' ) and m.Account = '{Account}'";
            }
            else
            {
                sql = $@" select * from Article  m inner join Members d on m.Account = d.Account where m.Account = '{Account}'";
            }
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
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
            paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / paging.ItemNum));
            paging.SetRightPage();
        }
        #endregion

        #region 新增文章
        public void InsertArticle(Article newData)
        {
            newData.A_Id = LastArticleFinder();
            string sql = $@" insert into Article(A_Id,Title,Content,Account,CreateTime,Watch) 
                           values('{newData.A_Id}','{newData.Title}','{newData.Content}','{newData.Account}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','0')";
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

        #region 最新文章A_Id
        public int LastArticleFinder()
        {
            int Id;
            string sql = $@" select top 1 * from Article order by A_Id desc";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Id"]);
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

        #region 修改文章
        public void UpdateArticle(Article Data)
        {
            string sql = $@" update Article set Content = '{Data.Content}' where A_Id = '{Data.A_Id}'";
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

        #region 刪除文章
         public void DeleteArticle(int A_Id)
         {
            string DeleteMessage = $@" delete from Message where A_Id = '{A_Id}'";
            string DeleteArticle = $@" delete from Article where A_Id = '{A_Id}'";
            string CombineSql = DeleteMessage + DeleteArticle;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(CombineSql, conn);
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

        #region 修改檢查
        public bool CheckUpdate(int A_Id)
        {
            Article Data = GetArticleDataById(A_Id);
            int MessageCount = 0;
            string sql = $@" select * from Message where A_Id = '{A_Id}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MessageCount++;
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
            return (Data != null && MessageCount == 0);
        }
        #endregion

        #region 人氣查詢
        public List<Article> GetPopularList(string Account)
        {
            List<Article> popularList = new List<Article>();
            string sql = $@" select top 5 * from Article m inner join Members d on m.Account = d.Account where m.Account ='{Account}' order by Watch desc";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    Data.Member.Name = dr["Name"].ToString();
                    popularList.Add(Data);
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
            return popularList;
        }
        #endregion

        #region 增加觀看人數
        public void AddWatch(int A_Id)
        {
            string sql = $@" update Article set Watch = Watch + 1 where A_Id = '{A_Id}'";
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