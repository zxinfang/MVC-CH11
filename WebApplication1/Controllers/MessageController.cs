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
    public class MessageController : Controller
    {
        private readonly MessageDBService messageService = new MessageDBService();
        // GET: Message

        #region 留言頁面
        public ActionResult Index(int A_Id = 1)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        #endregion

        #region 留言陣列
        public ActionResult MessageList(int A_Id, int Page = 1)
        {
            MessageViewModel Data = new MessageViewModel();
            Data.Paging = new ForPaging(Page);
            Data.A_Id = A_Id;
            Data.DataList = messageService.GetDataList(Data.Paging, Data.A_Id);
            return PartialView(Data);
        }
        #endregion

        #region 新增留言
        [Authorize]
        public ActionResult Create(int A_Id)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Add(int A_Id, [Bind(Include = "Content")]Message Data)
        {
            Data.A_Id = A_Id;
            Data.Account = User.Identity.Name;
            messageService.InsertMessage(Data);
            return RedirectToAction("MessageList", new { A_Id = Data.A_Id });
        }
        #endregion

        #region 修改留言
        [Authorize]
        public ActionResult UpdateMessage(int A_Id,int M_Id,string Content)
        {
            Message message = new Message();
            message.A_Id = A_Id;
            message.M_Id = M_Id;
            message.Content = Content;
            messageService.UpdateMessage(message);
            return RedirectToAction("Article", "Article", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除留言
        [Authorize]
        public ActionResult DeleteMessage(int A_Id, int M_Id)
        {
            messageService.DeleteMessage(A_Id, M_Id);
            return RedirectToAction("Article", "Article", new { A_Id = A_Id });
        } 
        #endregion
    }
}