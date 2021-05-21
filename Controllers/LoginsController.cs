using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Tools;
using static System.Collections.Specialized.BitVector32;
using System.Runtime;
using System.Threading;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebApp.Controllers
{
    public class LoginsController : Controller
    {
        private readonly MvcMovieContext _context;

        public LoginsController(MvcMovieContext context)
        {
            _context = context;
        }


        // GET: Logins 
        public async Task<IActionResult> Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("account")))
            {
                return View(await _context.Login.ToListAsync());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        // GET: Logins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Login
                .FirstOrDefaultAsync(m => m.ID == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // GET: Logins/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var login = await _context.Login.FindAsync(id);
        //    if (login == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(login);
        //}

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Account, string Password, string Code, [Bind("ID,Account,Code,Password,RePassword")] Login login)
        {
            //var linqLogin = _context.Login.Where(x => x.Account == account).FirstOrDefault();
            //login.AccountName = linqLogin.AccountName;
            var loginAccount = _context.Login.Where(x => x.Account == Account).FirstOrDefault();

            if (Code == loginAccount.Code)
            {
                loginAccount.Password = loginAccount.RePassword = PasswordHasher.HashPassword(Password);
                loginAccount.Code = null;
                _context.Login.Update(loginAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            else if (string.IsNullOrEmpty(Code))
            {
                ViewData["VSendMail"] = "请输入验证码";
            }
            else
            {
                ViewData["VSendMail"] = "验证码有误";
            }

            return View(login);
        }

        // GET: Logins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Login
                .FirstOrDefaultAsync(m => m.ID == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var login = await _context.Login.FindAsync(id);
            _context.Login.Remove(login);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginExists(int id)
        {
            return _context.Login.Any(e => e.ID == id);
        }

        public  void  DelVerify(string account)
        {
            var verify1 =  _context.Verify.Where(v => v.Account == account).FirstOrDefault();
             _context.Verify.Remove(verify1);
             _context.SaveChanges();
            //return new NoContentResult();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CountDown(string account, string Title, [Bind("Code")] Login login, [Bind("Account", "Code")] Verify verify)
        {
            string type = Title;
            switch (type.Trim())
            {
                case "Update":
                    type = "修改";
                    break;
                case "Register":
                    type = "注册";
                    break;
                case "Login":
                    type = "登录";
                    break;
            }
            var result = _context.WF_SendMail.Where(x => x.Type == type).FirstOrDefault();
            var code = Tools.SendMail.CreateRandom(5);
            var UpdateAccount = _context.Login.Where(x => x.Account == account).FirstOrDefault();
            if (UpdateAccount == null)
            {
                verify.Code = code;
                verify.Account = account;
                _context.Verify.Add(verify);
                await _context.SaveChangesAsync();
                await SendMail(account, type, code);
            }
            else
            {
                UpdateAccount.Code = code;
                _context.Login.Update(UpdateAccount);
                await _context.SaveChangesAsync();
                await SendMail(account, type, code);

                //Thread.Sleep(60000);//休眠时间
                //var verify1 = await _context.Verify.FindAsync(verify.ID);
                //_context.Verify.Remove(verify1);
                //await _context.SaveChangesAsync();
            }
            //else
            //{
            //    ViewData["VSendMail"] = "已发送验证码";
            //}

            return new NoContentResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AccountName,Account,Code,Now,Online,Deadline,RoleID,Enable,version,Password,RePassword")] Login login)
        {
            if (ModelState.IsValid)
            {
                login.Password = login.RePassword = PasswordHasher.HashPassword(login.Password);
                login.Now = Convert.ToDateTime(DateTime.Now.ToUniversalTime().ToString());
                var v = _context.Verify.Where(x => x.Account == login.Account).FirstOrDefault();
                if (string.IsNullOrEmpty(v.Code))
                {
                    throw new ArgumentNullException(nameof(v.Code));
                }
                if (v.Code == login.Code)
                {
                    _context.Add(login);
                    DelVerify(login.Account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(login));
                }
                else
                {
                    ViewData["VSendMail"] = "验证码错误";
                }

            }
            return View(login);
        }

        public async Task<NoContentResult> SendMail(string account, string type, string code = "")
        {

            var result = _context.WF_SendMail.Where(x => x.Type == type).FirstOrDefault();
            if (result == null)
            {
                ViewData["VSendMail"] = "没有此这类型";
            }
            else
            {
                var body = result.body;
                switch (type)
                {
                    case "登录":
                        Tools.SendMail.SendEmail1(account, result.Title, body, result.Email, result.SecretKey, type);
                        break;
                    case "修改":
                        Tools.SendMail.SendEmail1(account, result.Title, body, result.Email, result.SecretKey, type, code);
                        break;
                    case "注册":
                        Tools.SendMail.SendEmail1(account, result.Title, body + code, result.Email, result.SecretKey, type, code);
                        break;
                }
                //Tools.SendMail.SendEmail1(account, result.Title, result.body, result.Email, result.SecretKey, code);
            }
            return new NoContentResult();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账号邮箱</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string account, string password)
        {
            //var ClientIP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if (ModelState.IsValid)
            {
                var result = _context.Login.Where(x => x.Account == account).FirstOrDefault();
                if (result == null)
                {
                    ViewData["Repeat"] = "邮箱不存在";
                    return View();
                }
                if (PasswordHasher.VerifyHashedPassword(password, result.Password))
                {
                    result.Deadline = Convert.ToDateTime(DateTime.Now.ToUniversalTime().ToString());
                    result.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();//获取IP地址
                                                                                              //Tools.SendMail.SendEmail1("1574697082@qq.com", "3411.top","3411.top");
                                                                                              //Tools.SendMail.SendEmail1("919427340@qq.com", "3411.top", "3411.top");
                    await SendMail(account, "登录");
                    HttpContext.Session.SetString("account", account);
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Sid,result.ID.ToString()),
                    //    new Claim(ClaimTypes.Name,result.AccountName),
                    //    new Claim(ClaimTypes.Role,result.RoleID.ToString())
                    // };
                    //var identity = new ClaimsIdentity(claims, account);
                    //var userPrincipal = new ClaimsPrincipal(identity);
                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                    //{
                    //    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    //    IsPersistent = false,
                    //    AllowRefresh = false
                    //});
                    return Redirect("~/Logins/Index");
                }
                else
                {
                    ViewData["Repeat"] = "密码错误";
                    return View();
                }
            }
            return RedirectToAction(nameof(Login));

        }
        //[HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetString("account", "");
            return RedirectToAction(nameof(Login));
        }
        //public async Task<IActionResult> Logout()
        //{
        //    //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction(nameof(Login));
        //}

    }
}
