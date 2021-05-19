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
            return View(await _context.Login.ToListAsync());
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Login.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            return View(login);
        }

        public IActionResult Login()
        {
            //List<Login> ttolist = new Login;
            //return Json(new { Code = 1, Msg = "222" });
            return View();
        }


        // POST: Logins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AccountName,Account,Code,Now,Online,Deadline,RoleID,Enable,version,Password,RePassword")] Login login)
        {
            if (id != login.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(login);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginExists(login.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DelVerify(string   account)
        {
            var verify1 = await _context.Verify.Where(v=>v.Account==account).FirstOrDefaultAsync();
            _context.Verify.Remove(verify1);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CountDown(string account, [Bind("Account", "Code")] Verify verify)
        {
            var result = _context.WF_SendMail.Where(x => x.Type == "注册").FirstOrDefault();
            var code = Tools.SendMail.CreateRandom(5);
            var verifyAccount = _context.Verify.Where(x => x.Account == account).FirstOrDefault();
            if (verifyAccount == null)
            {
                verify.Account = account;
                verify.Code = code;
                _context.Verify.Add(verify);
                await _context.SaveChangesAsync();

                //Tools.SendMail.SendEmail1(account, result.Title, result.body, result.Email, result.SecretKey, code);
                await SendMail(account,"注册",code);

                //Thread.Sleep(60000);//休眠时间
                //var verify1 = await _context.Verify.FindAsync(verify.ID);
                //_context.Verify.Remove(verify1);
                //await _context.SaveChangesAsync();
            }
            else
            {
                ViewData["VSendMail"] = "已发送验证码";
            }
            return new NoContentResult();
        }
        // GET: Logins/Create 注册账号
        // POST: Logins/Create
        // GET: Logins/Create
        public IActionResult Create()
        {
            ViewData["VCountDown"] = 60;
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AccountName,Account,Code,Now,Online,Deadline,RoleID,Enable,version,Password,RePassword")] Login login)
        {
            if (ModelState.IsValid)
            {
                //login.RePassword = login.Password = EncryptionDecryptionUtility.MD5Encrypt(login.Password, "9");
                login.Password = login.RePassword = PasswordHasher.HashPassword(login.Password);
                login.Now = Convert.ToDateTime(DateTime.Now.ToUniversalTime().ToString());
                //ViewData["SessionCode"] = this.HttpContext.Session.GetString("SessionCode");
                var v = _context.Verify.Where(x => x.Account ==login.Account).FirstOrDefault();
                if (string.IsNullOrEmpty(v.Code) )
                {
                    throw new ArgumentNullException(nameof(v.Code));
                }
                if (v.Code == login.Code)
                {
                    _context.Add(login);
                    DelVerify(login.Account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["VSendMail"] = "验证码错误";
                }

            }
            return View(login);
        }
        public async Task<NoContentResult> SendMail(string account, string type,string  code="")
        {
            if (ModelState.IsValid)
            {
                var result =  _context.WF_SendMail.Where(x => x.Type == type).FirstOrDefault();
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
                        case"修改":
                            Tools.SendMail.SendEmail1(account, result.Title, body, result.Email, result.SecretKey, type);
                            break;
                        case "注册":
                            Tools.SendMail.SendEmail1(account, result.Title, body+code, result.Email, result.SecretKey, type, code);
                            break;
                    }
                    //Tools.SendMail.SendEmail1(account, result.Title, result.body, result.Email, result.SecretKey, code);
                }
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
                    result.IP= HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();//获取IP地址
                    //Tools.SendMail.SendEmail1("1574697082@qq.com", "3411.top","3411.top");
                    //Tools.SendMail.SendEmail1("919427340@qq.com", "3411.top", "3411.top");
                    await SendMail(account, "登录");
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


    }
}
