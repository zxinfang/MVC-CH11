using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class Members
    {
        [DisplayName("帳號：")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30,MinimumLength =6 ,ErrorMessage = "帳號長度介於6~30字元")]
        [Remote("AccountCheck","Members",ErrorMessage ="此帳號已被註冊過")]
        public string Account { get; set; }

        [DisplayName("名字:")]
        [Required(ErrorMessage = "請輸入名字")]
        [StringLength(20, ErrorMessage = "名字不可超過20字元")]
        public string Name { get; set; }

        public string Password { get; set; }

        [DisplayName("大頭照：")]
        public string Image { get; set; }

        [DisplayName("Email：")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email帳號長度不可超過200字元")]
        [EmailAddress(ErrorMessage ="這不是Email格式")]
        public string Email { get; set; }

        public string AuthCode { get; set; }

        public bool IsAdmin { get; set; }
    }
}