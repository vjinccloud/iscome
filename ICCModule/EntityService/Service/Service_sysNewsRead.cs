using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    /// <summary>
    /// 公告讀取時間
    /// </summary>
    public class Service_sysNewsRead
    {
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static sysNewsRead GetDetail(string id)
        {
            return RepositoryUtility.GetDetail<sysNewsRead>(x => x.LoginID == id);
        }

        /// <summary>
        /// 紀錄讀取時間
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult RecordDate(string id)
        {
            string sErrMsg = "";
            bool rslt = false;

            BaseRepository Base = new BaseRepository();
            var checkUser = Base.QueryData<sysUserInfo>(x => x.LoginID == id).SingleOrDefault();
            if(checkUser != null)
            {
                //查詢資料
                var model = Base.QueryData<sysNewsRead>(ref sErrMsg, x => x.LoginID == id).SingleOrDefault();
                if (model == null)
                {
                    model = new sysNewsRead();
                    model.LoginID = id;
                    model.ReadDate = DateTime.Now;
                    RepositoryUtility.Insert(model);
                }
                else
                {
                    model.ReadDate = DateTime.Now;
                    //更新回資料庫
                    rslt = Base.Update(ref sErrMsg);
                }
            }

            return new BaseResult(rslt, sErrMsg);
        }
    }
}
