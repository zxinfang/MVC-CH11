using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Serivces;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class BlogController : Controller
    {
        private readonly MembersDBService memberService = new MembersDBService();
        // GET: Blog
        public ActionResult Index(string Account)
        {
            BlogViewModel Data = new BlogViewModel();
            Data.Member = memberService.GetDataByAccount(Account);
            return View(Data);
        }
    }
}