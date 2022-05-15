using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Serivces;

namespace WebApplication1.ViewModels
{
    public class MessageViewModel
    {
        public List<Message> DataList { get; set; }

        public ForPaging Paging { get; set; }

        public int A_Id { get; set; }
    }
}