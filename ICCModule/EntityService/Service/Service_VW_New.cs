using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_VW_New
    {    
        /// <summary>讀取清單</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<VW_New> GetList_Range(String Code)
        {
            Expression<Func<VW_New, bool>> filter = (x => true);
            filter = filter.And(x => x.Code == Code);

            String Error = "";
            PagerInfo page = new PagerInfo();
            page.m_iPageCount = 7;
            return RepositoryUtility.GetList_Range<VW_New, DateTime>(ref Error, page, filter, x => x.StartDate, false);
        }
    }
}