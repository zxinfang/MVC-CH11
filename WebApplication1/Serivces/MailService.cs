using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApplication1.Serivces
{
    public class MailService
    {
        private string gmail_account = "xinfang084";
        private string gmail_mail = "xinfang084@gmail.com";
        private string gmail_password = "fang0804";

        #region 取得驗證碼
        public string GetAuthCode()
        {
            string[] Code = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                              "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                              "1","2","3","4","5","6","7","8","9"};
            string AuthCode = string.Empty;
            Random rd = new Random();
            for(int i = 0; i < 10; i++)
            {
                AuthCode += Code[rd.Next(Code.Count())];
            }
            return AuthCode;
        }
        #endregion

        #region 驗證信內容
        public string GetMailBody(string Temp,string Account,string ValidateUrl)
        {
            Temp = Temp.Replace("{{UserName}}", Account);
            Temp = Temp.Replace("{{ValidateUrl}}", ValidateUrl);
            return Temp;
        }
        #endregion

        #region 寄信
        public void SendMail(string MailBody,string ToEmail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            smtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject="會員註冊驗證信";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            smtpServer.Send(mail);
        }
        #endregion 
    }
}