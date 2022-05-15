using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication1.Security;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_OnPostAuthenticateRequest(object sender,EventArgs e)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"];
            string cookieName = WebConfigurationManager.AppSettings["CookieName"];
            if (httpRequest.Cookies[cookieName] != null)
            {
                JwtObject jwtObject = JWT.Decode<JwtObject>(httpRequest.Cookies[cookieName].Value, Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
                string[] role = jwtObject.Role.Split(new char[] { ',' });
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,jwtObject.Account)
                };
                var claimsIdentity = new ClaimsIdentity(claims, cookieName);
                claimsIdentity.AddClaim(new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                HttpContext.Current.User = new GenericPrincipal(claimsIdentity, role);
            }
        }
    }
}
