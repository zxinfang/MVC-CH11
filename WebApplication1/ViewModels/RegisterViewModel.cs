using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("大頭照:")]
        public HttpPostedFileBase MembersImage { get; set; }
        public Members newMember { get; set; }

        [DisplayName("密碼:")]
        [Required(ErrorMessage ="請輸入密碼")]
        public string Password { get; set; }

        [DisplayName("確認密碼:")]
        [Required(ErrorMessage = "請輸入密碼")]
        [Compare("Password",ErrorMessage ="兩次密碼輸入不一致")]
        public string PasswordCheck { get; set; }
    }
}