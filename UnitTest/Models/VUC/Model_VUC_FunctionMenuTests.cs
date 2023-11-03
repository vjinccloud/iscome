using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationSystem.Models.VUC;

namespace IMSystem.Models.VUC.Tests
{
    [TestClass()]
    public class Model_VUC_FunctionMenuTests
    {

        [TestMethod()]
        public void LoadDataTest()
        {
            //驗證看看有沒有抓到選單資料
            Model_VUC_FunctionMenu model = new Model_VUC_FunctionMenu();
            model.LoadData("system", "");
            //查無資料? 失敗
            if (model.funcList.Count == 0)
                Assert.Fail();
            foreach (var item in model.funcList)
            {
                //查無資料? 失敗
                if (item.list_detail.Count == 0)
                    Assert.Fail();
            }
        }
    }
}