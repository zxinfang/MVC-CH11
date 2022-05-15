using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Serivces;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ArticleDBService articleService = new ArticleDBService();
        private readonly MessageDBService messageService = new MessageDBService();

        // GET: Article

        #region 起始頁
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 文章列表
        public ActionResult List(string Search, string Account, int Page = 1)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.Account = Account;
            Data.DataList = articleService.GetDataList(Data.Paging, Data.Search,Data.Account);
            return PartialView(Data);
        }
        #endregion

        #region 文章頁面
        public ActionResult Article(int A_Id)
        {
            ArticleViewModel Data = new ArticleViewModel();
            articleService.AddWatch(A_Id);
            Data.article = articleService.GetArticleDataById(A_Id);
            ForPaging paging = new ForPaging(0);
            Data.DataList = messageService.GetDataList(paging, A_Id);
            return View(Data);
        }
        #endregion

        #region 新增文章
        [Authorize]
        public ActionResult Create()
        {
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include ="Title,Content")]Article Data)
        {
            Data.Account = User.Identity.Name;
            articleService.InsertArticle(Data);
            return RedirectToAction("Index","Blog",new { Account = User.Identity.Name});
        }
        #endregion

        #region 修改文章
        [Authorize]
        public ActionResult EditPage(int A_Id)
        {
            Article Data = new Article();
            Data = articleService.GetArticleDataById(A_Id);
            return PartialView(Data);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditPage(int A_Id,Article Data)
        {
            if (articleService.CheckUpdate(A_Id))
            {
                articleService.UpdateArticle(Data);
            }
            return RedirectToAction("Article",new { A_Id = A_Id});
        }
        #endregion

        #region 刪除文章
        public ActionResult Delete(int A_Id)
        {
            articleService.DeleteArticle(A_Id);
            return RedirectToAction("Index", "Blog", new { Account = User.Identity.Name });
        }
        #endregion

        #region 顯示人氣
        public ActionResult ShowPopularity(string Account)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            Data.DataList = articleService.GetPopularList(Account);
            return View(Data);
        }
        #endregion
    }
}