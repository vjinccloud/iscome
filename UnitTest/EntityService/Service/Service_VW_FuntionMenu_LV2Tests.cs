using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service.Tests
{
    [TestClass()]
    public class Service_VW_FuntionMenu_LV2Tests
    {
        [TestMethod()]
        public void GetListTest()
        {
            var list = Service_VW_FuntionMenu_LV2.GetList("system");
            //查無資料
            if (list.Count == 0)
                Assert.Fail();
        }
    }
}