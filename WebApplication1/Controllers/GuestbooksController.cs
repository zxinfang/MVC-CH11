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
    public class GuestbooksController : Controller
    {
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(string Search, int Page = 1)
        {
            GuestbooksViewModel Data = new GuestbooksViewModel();
            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.DataList = GuestbookService.GetDataList(Data.Paging, Data.Search);
            return PartialView(Data);
        }
        [HttpPost]
        public ActionResult GetDataList([Bind(Include ="Search")] GuestbooksViewModel Data)
        {
            return RedirectToAction("GetDataList", new { Search = Data.Search });
        }

        #region 新增留言
        public ActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "Content")]Guestbooks Data)
        {
            Data.Account = User.Identity.Name;
            GuestbookService.InsertGuestbooks(Data);
            return RedirectToAction("Index");
        }
        #endregion

        #region 修改留言
        public ActionResult Edit(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Edit(int Id, [Bind(Include = "Content")]Guestbooks UpdateData)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                UpdateData.Id = Id;
                UpdateData.Account = User.Identity.Name;
                GuestbookService.UpdateGuestbooks(UpdateData);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 回覆留言
        public ActionResult Reply(int Id)
        {
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Reply(int Id, [Bind(Include = "Reply,ReplyTime")]Guestbooks ReplyData)
        {
            if (GuestbookService.CheckUpdate(Id))
            {
                ReplyData.Id = Id;
                GuestbookService.ReplyGuestbooks(ReplyData);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 刪除留言
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            GuestbookService.DeleteGuestbooks(Id);
            return RedirectToAction("Index");
        }
        #endregion

    }
}