using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Verify
    {
        public int ID { get; set; }

        [Display(Name = "用户账号")]
        public string Account { get; set; }

        [Display(Name = "验证码")]
        public string Code { get; set; }
    }
}
