using ICCModule;
using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Models.Sample
{
    /// <summary>
    /// 網頁元件用 後端模型
    /// </summary>
    public class Model_Sample_Pager
    {
        public Model_Sample_Pager()
        {
            this.list_Log = new List<sysAccess_log>();
            this.page = new PagerInfo();
        }


        public List<sysAccess_log> list_Log { get; set; }

        /// <summary>
        /// 分頁參數 
        /// 這邊會搭載RepositoryUtility/BaseRepository模組示範實作
        /// </summary>
        public PagerInfo page { get; set; }

        /// <summary>
        /// 讀取資料
        /// </summary>
        public void Load()
        {
            DateTime beg = Convert.ToDateTime("2021-01-01");
            DateTime end = DateTime.Now.Date.AddDays(1);
            this.list_Log = Service_sysAccess_log.GetList(beg, end, this.page);
        }
    }
}