using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class WF_SendMail
    {
        [HiddenInput(DisplayValue = false)]//会让改属性在编辑的时候不显示出来。
        public int ID { get; set; }
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$",
ErrorMessage = "请输入正确的Email格式\n示例：abc@123.com")]
        [Required(ErrorMessage = "请输入发送者邮箱")]
        [Display(Name = "发送者邮箱")]
        public string Email { get; set; }

        [Display(Name = "秘钥")]
        public string SecretKey { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "内容")]
        public string body { get; set; }

        [Display(Name = "类型")]
        public string Type { get; set; }
    }
}
