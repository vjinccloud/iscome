using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICCModule.Entity.Tables;

namespace ICCModule.EntityService.Service.Tests
{
    [TestClass()]
    public class Service_sysUserInfoTests
    {
        [TestMethod()]
        public void GetDetailTest()
        {
            //測試Service_sysUserInfo.GetDetail
            //抓系統管理者看看
            var data = Service_sysUserInfo.GetDetail("system");
            if (data == null)
            {   //抓不到? 回傳測試失敗
                Assert.Fail();
            }
            //通過測試 後面不用任何指令
        }

        [TestMethod()]
        public void GetListTest()
        {
            //測試Service_sysUserInfo.GetDetail
            //抓系統管理者看看
            var dataList = Service_sysUserInfo.GetList();
            if (dataList.Count == 0)
            {   //抓不到? 回傳測試失敗
                Assert.Fail();
            }
            //通過測試 後面不用任何指令
        }

        [TestMethod()]
        public void InsertTest()
        {
            string LoginID = "##TESTDATA_ABCD123456789";
            //在那之前先刪除資料 避免出錯
            Service_sysUserInfo.Delete(LoginID);
            //建立測試資料
            var data = CreateTESTData();
            data.LoginID = LoginID;
            var rs = Service_sysUserInfo.Insert(data);
            if (rs.result == false)
            {
                Assert.Fail("sysUserInfo寫入失敗:" + rs.Msg);
            }
            //抓看看
            data = Service_sysUserInfo.GetDetail(LoginID);
            if (data == null)
            {
                Assert.Fail("sysUserInfo寫入失敗:無法讀取LoginID");
            }

            //刪除資料 避免出錯
            Service_sysUserInfo.Delete(LoginID);
        }
        /// <summary>
        /// 產生測試用的資料
        /// </summary>
        /// <returns></returns>
        public static sysUserInfo CreateTESTData()
        {
            sysUserInfo data = new sysUserInfo();
            data.AddDate = DateTime.Now;
            data.EditDate = DateTime.Now;
            data.LoginPass = "";
            data.UserName = "";
            data.RoleID = "";
            data.State = "";
            data.ResetTokenID = "";
            data.LastSessionID = "";
            data.EmailAddress = "";
            return data;
        }
    }
}