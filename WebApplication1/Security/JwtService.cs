using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace WebApplication1.Security
{
    public class JwtService
    {
        #region 製作token
        public string GenerateToken(string Account,string Role)
        {
            JwtObject jwtObject = new JwtObject()
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinute"])).ToString()
            };
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"];
            var payload = jwtObject;
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
            return token;
        }
        #endregion
    }
}