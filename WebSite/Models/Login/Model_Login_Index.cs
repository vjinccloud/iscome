using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using ICCModule.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace Website.Models.Login
{
    public enum LoginState
    {
        OK = 1,
        NG = 2,
        RESET = 3,
        DISABLED = 4,
        LOCK = 5,
    }
    public class Model_Login_Index
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string LoginID { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string LoginPass { get; set; }
        /// <summary>
        /// 圖形驗證碼
        /// </summary>
        public string reCAPTCHA { get; set; }
        /// <summary>
        /// 記住我
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 登入驗證 不含圖形驗證碼判斷
        /// </summary>
        /// <returns>OK:通過,NG:驗證失敗,RESET:需要重設密碼,D:停用,L:鎖定</returns>
        public LoginState LoginCheck(bool isDebug, ref string msg)
        {
            //先把帳號做正規化處理 避免異常
            this.LoginID = this.LoginID ?? "";
            this.LoginID = this.LoginID.ToLower().Trim();

            //判斷登入模式            
            //帳密驗證
            var usr = Service_MemberInfo.GetAccountDetail(this.LoginID);
            if (usr == null)
            {
                msg = "帳密驗證失敗";
                return LoginState.NG;
            }

            //判斷目前帳號是否已經被停用或鎖定? 如果是就不用驗證了
            switch (usr.Status)
            {
                case "D":
                    msg = "帳號已被停用";
                    return LoginState.DISABLED;
                case "L":
                    msg = "帳號已被鎖定";
                    return LoginState.LOCK;
            }
            //帳密驗證 偵錯模式打開的情況下 只要帳號存在就無條件通過
            if (isDebug)
            {
                msg = "登入成功";
                return LoginState.OK;
            }
            //帳密驗證 偵錯模式關掉才做
            bool match = false;
            match = usr.CheckPassword(this.LoginPass);
            //驗證失敗
            if (match == false)
            {
                msg = "帳密驗證失敗";
                return LoginState.NG;
            }
            msg = "登入成功";
            return LoginState.OK;
        }

        /// <summary>AD驗證
        /// 當要取 entry.Properties.Count 會Exception 表示失敗 反之 成功
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="LoginPass"></param>
        /// <returns></returns>
        public static bool CheckLoginUserToADServer(string userId, string LoginPass, out string ErrorMsg)
        {
            ErrorMsg = "";
            string LoginCheckADIP = ConfigurationManager.AppSettings["LoginCheckADIP"];
            if (string.IsNullOrEmpty(LoginCheckADIP)) return false;
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + LoginCheckADIP, userId, LoginPass);
            try { return entry.Properties.Count > 0 ? true : false; }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ErrorLog.SaveErrorLog(ex);
                return false;
            }
        }
        /// <summary>
        /// 登入紀錄
        /// </summary>
        public bool SaveLog(string LoginIP, LoginState state, string msg)
        {
            //紀錄Log
            sysLogin_log log = new sysLogin_log();
            log.LoginID = this.LoginID;
            log.LoginIP = LoginIP;
            log.CDate = DateTime.Now;
            if (state == LoginState.OK)
                log.Record = msg;
            else
                log.Record = "登入失敗，" + msg;
            return Service_sysLogin_log.Insert(log).result;
        }

    }
}