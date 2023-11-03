using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Member
{
    public class SignUpVerifyViewModel
    {
        /// <summary>
        /// 驗證用手機號
        /// </summary>
        public string Mobile { get; set; }
        
        /// <summary>
        /// 手機號進行遮蔽
        /// </summary>
        public string MaskMobile
        {
            get
            {
                var list = Mobile.Split().ToList();
                string mobile = "";
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 3 && i < 7)
                    {
                        mobile += "○";
                    }
                    else
                    {
                        mobile += list[i];
                    }
                }
                return mobile;
            }
        }

        /// <summary>
        /// 驗證用信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 簡訊驗證碼
        /// </summary>
        public string SmsVerifyCode { get; set; }

        /// <summary>
        /// 頁面驗證碼
        /// </summary>
        public string VerifyCode { get; set; }
    }
}