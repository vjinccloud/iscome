using Microsoft.VisualStudio.TestTools.UnitTesting;
using InformationSystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Controllers.Tests
{
    [TestClass()]
    public class AccessControlFilterTests
    {
        [TestMethod()]
        public void _PharseURLTest()
        {
            string output = AccessControlFilter_Helper._PharseURL("/test");
            if (AccessControlFilter_Helper._PharseURL("/test") != "/test/index")
                Assert.Fail();
            if (AccessControlFilter_Helper._PharseURL("/test/") != "/test/index")
                Assert.Fail();
            if (AccessControlFilter_Helper._PharseURL("/test/index") != "/test/index")
                Assert.Fail();
            if (AccessControlFilter_Helper._PharseURL("/test/index/") != "/test/index")
                Assert.Fail();
            if (AccessControlFilter_Helper._PharseURL("/TEST/index/") != "/test/index")
                Assert.Fail();
        }
    }
}