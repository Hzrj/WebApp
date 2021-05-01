using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Welcom()
        {
            return "name is ZRJ";
        }
    }
}
