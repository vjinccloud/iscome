using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    /// <summary>
    /// 接收註冊欄位
    /// </summary>
    public class MemberRegisterModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 區
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 申請身分別
        /// </summary>
        public string Identify { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 確認密碼
        /// </summary>
        public string ConfirmPassword { get; set; }


    }
}


/*
    model.name = $('#name').val();
         model.account = $('#account').val();
         model.area = $('#area').val();
         model.distict = $('#distict').val();
         model.type = $('#type').val();
         model.phone = $('#phone').val();
         model.password = $('#password').val();
         model.confirm_password = $('#confirm_password').val();
 */