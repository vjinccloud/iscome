using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Home
{
    public class QuestionManagementViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        public Dictionary<string, List<QuestionManagement>> List { get; set; }

    }
}