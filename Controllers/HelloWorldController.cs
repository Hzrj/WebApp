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
        public IActionResult Index()
        {
            return View();
        }

        //https://localhost:{port}/HelloWorld/welcome?name=ZRJ&numtimes=9
        public IActionResult Welcome(string name,int NumTimes=1)
        {
            ViewData["Message"] = "Hello " + name+"Welcome my Blog";
            ViewData["NumTimes"] = NumTimes;
            return View();
        }
    }
}
