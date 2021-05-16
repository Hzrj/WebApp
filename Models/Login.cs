using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace WebApp.Models
{
    public class  Login
    {
        [HiddenInput(DisplayValue = false)]//会让改属性在编辑的时候不显示出来。
        public int ID { get; set; }
        
        [DataType(DataType.MultilineText)]// 在前台会渲染成Textarea  测试
        [Required(ErrorMessage = "请输入用户名称")]
        [Display(Name ="用户名称")]
        public string AccountName { get; set; }

        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$",
 ErrorMessage = "请输入正确的Email格式\n示例：abc@123.com")]
        [Required(ErrorMessage = "请输入用户账号")]
        [Display(Name = "用户账号")]
        public string Account { get; set; }

        //[Required(ErrorMessage = "请输入验证码")]
        [Display(Name = "验证码")]
        [StringLength(20,MinimumLength =6)]
        public string Code { get; set; }

        [Display(Name = "注册时间")]
        [DisplayFormat(DataFormatString = "{0:yyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode =true)]
        public DateTime Now { get; set; }

        [Display(Name = "是否在线")]
        [DefaultValue(false)]//默认值
        public bool Online { get; set; }

        [Display(Name = "最后上线时间")]
        public DateTime Deadline { get; set; }

        [Display(Name = "角色ID")]
        public int RoleID { get; set; }

        [Display(Name = "是否激活")]
        public bool Enable { get; set; }

        [Display(Name = "版本号")]
        public int version { get; set; }

        [DataType(DataType.Password)]// 在前台会渲染成Textarea  测试
        [Required]
        [Display(Name = "用户密码")]
        [StringLength(120, MinimumLength = 6)]
        [PasswordPropertyText]
        public string Password { get; set; }

        [DataType(DataType.Password)]// 在前台会渲染成Textarea  测试
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        [PasswordPropertyText]
        [Required]
        [Display(Name = "用户密码")]
        [StringLength(120, MinimumLength = 6)]
        public string RePassword { get; set; }

        public static explicit operator ArrayList(Login v)
        {
            throw new NotImplementedException();
        }

        //上传图片到数据库
        //https://www.cnblogs.com/stoneniqiu/p/3601893.html

    }
}
