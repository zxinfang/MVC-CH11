using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Serivces
{
    public class MembersDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 註冊
        public void Register(Members newMember)
        {
            newMember.Password = HashPassword(newMember.Password);
            string sql = $@" insert into Members(Account,Password,Name,Image,Email,AuthCode,IsAdmin)
                           values('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Image}','{newMember.Email}','{newMember.AuthCode}','0')";
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

        #region Hash密碼
        public string HashPassword(string Password)
        {
            string salt = "1q2w3e4r5t6y7u8i9o";
            string saltAndPassword = string.Concat(salt, Password);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] PassData = Encoding.UTF8.GetBytes(saltAndPassword);
            byte[] Hash = sha256.ComputeHash(PassData);
            string HashResult = Convert.ToBase64String(Hash);
            return HashResult;
        }
        #endregion

        #region 帳號確認
        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            return (Data == null);
        }
        #endregion

        #region 查詢一筆資料
        public Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"select * from Members where Account = '{Account}' ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.Image = dr["Image"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
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

        #region 查詢一筆公開性資料
        public Members GetDatByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"select * from Members where Account = '{Account}' ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Image = dr["Image"].ToString();
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

        #region 信箱驗證
        public string EmailValidate(string Account,string AuthCode)
        {
            Members Data = GetDataByAccount(Account);
            if (Data != null)
            {
                if (Data.AuthCode == AuthCode)
                {
                    string sql = $@"update Members set AuthCode ='{string.Empty}' where Account ='{Account}'";
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
                    return "信箱驗證成功，可以去登入了";
                }
                else
                {
                    return "驗證碼傳送錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                return "資料傳送錯誤，請重新確認或再註冊";
            }
        }
        #endregion

        #region 登入確認
        public string LoginCheck(string Account,string Password)
        {
            Members Data = GetDataByAccount(Account);
            if (Data != null)
            {
                if (string.IsNullOrWhiteSpace(Data.AuthCode))
                {
                    if (PasswordCheck(Data, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過信箱驗證，請去收信";
                }
            }
            else
            {
                return "查無此會員資料";
            }
        }
        #endregion

        #region 密碼確認
        public bool PasswordCheck(Members members,string Password)
        {
            return (members.Password.Equals(HashPassword(Password)));
        }
        #endregion

        #region 取得角色
        public string GetRole(string Account)
        {
            Members Data = GetDataByAccount(Account);
            string role = "User";
            if (Data.IsAdmin)
            {
                role += ",Admin";
            }
            return role;
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImage(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion

        #region 修改密碼
        public string ChangePassword(string Account,string Password,string newPassword)
        {
            Members Data = GetDataByAccount(Account);
            if (PasswordCheck(Data, Password))
            {
                Data.Password = HashPassword(newPassword);
                string sql = $@"update Members set Password = '{Data.Password}' where Account='{Account}'";
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
                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion
    }
}