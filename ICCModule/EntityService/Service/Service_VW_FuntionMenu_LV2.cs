using ICCModule.Entity.Tables;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_VW_FuntionMenu_LV2
    {


        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<VW_FuntionMenu_LV2> GetList(string LoginID)
        {
            return RepositoryUtility.GetList<VW_FuntionMenu_LV2>(x => x.LoginID == LoginID);
        }

    }
}
