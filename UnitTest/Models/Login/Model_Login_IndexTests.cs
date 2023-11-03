using Microsoft.VisualStudio.TestTools.UnitTesting;
using InformationSystem.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Models.Login.Tests
{
    [TestClass()]
    public class Model_Login_IndexTests
    {
        /// <summary>
        /// 登入功能測試
        /// </summary>
        [TestMethod()]
        public void LoginCheckTest()
        {
            string msg = "";
            Model_Login_Index model = new Model_Login_Index();
            //什麼都不打,應該是不會過
            LoginState s = model.LoginCheck(true, ref msg);
            if (s != LoginState.NG)
            {
                Assert.Fail();
            }
            //給帳號和錯誤的密碼
            model.LoginID = "system";
            model.LoginPass = "adminXXXXX";
            //開發用版本 只要帳號對就OK,應該回傳OK
            s = model.LoginCheck(true, ref msg);
            if (s != LoginState.OK)
            {
                Assert.Fail();
            }
            //正式用版本 應該回傳NG
            s = model.LoginCheck(false, ref msg);
            if (s != LoginState.NG)
            {
                Assert.Fail();
            }

            //給帳號和正確的密碼
            model.LoginID = "system";
            model.LoginPass = "admin";
            //正式用版本 應該回傳OK
            s = model.LoginCheck(true, ref msg);
            if (s != LoginState.OK)
            {
                Assert.Fail();
            }

            //通過檢查
        }
    }
}