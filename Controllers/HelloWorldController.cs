using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class HelloWorldController : Controller
    {
        //public IActionResult Index() 
        //{
        //    return View();
        //}
         
        public string index() 
        {
            return "This is my default action...";
        }

        public string Welcome(string name,int ID=1)
        {
            return HtmlEncoder.Default.Encode($"Hello {name},NumTimes is {ID}");
        }
    }
}
