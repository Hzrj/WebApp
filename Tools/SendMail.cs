using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Tools
{
    public class SendMail
    {
        /// <summary>
        /// 随机生成验证码
        /// </summary>
        /// <param name="codelengh">位数</param>
        /// <returns></returns>
        public static string CreateRandom(int codelengh)
        {
            int rep = 0;
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codelengh; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

     
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailSubject">标题</param>
        /// <param name="mailContent">内容</param>
        /// <param name="email">发送者</param>
        /// <param name="secretkey">秘钥</param>
        /// <param name="code">验证码</param>
        public static void SendEmail1(string mailTo, string mailSubject, string mailContent,string email,string secretkey,string type, string code = "")
        {
            SmtpClient mailClient = new SmtpClient("smtp.qq.com");
            mailClient.EnableSsl = true;
            mailClient.UseDefaultCredentials = false;
            //Credentials登陆SMTP服务器的身份验证.
            mailClient.Credentials = new NetworkCredential(email, secretkey);//邮箱，
            MailMessage message = new MailMessage(new MailAddress(email), new MailAddress(mailTo));//发件人，收件人
            message.IsBodyHtml = true;
            // message.Bcc.Add(new MailAddress("tst@qq.com")); //可以添加多个收件人
            switch (type)
            {
                case "登录":
                    message.Body = mailContent;//邮件内容
                    break;
                case "修改":
                    message.Body = mailContent + "验证码:" + code;//邮件内容
                    break;
                default:
                    message.Body = "您的账号正在3411.TOP上注册账号,验证码:" + code;//邮件内容
                    break;
            }
            message.Subject = mailSubject;//邮件主题
                                          //Attachment 附件
                                          //Attachment att = new Attachment(@"C:/hello.txt");
                                          //message.Attachments.Add(att);//添加附件
                                          //Console.WriteLine("Start Send Mail....");
                                          //发送....
            mailClient.Send(message); // 发送邮件
        }
    }
}
