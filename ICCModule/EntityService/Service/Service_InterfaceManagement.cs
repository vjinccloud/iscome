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
    public class Service_InterfaceManagement
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static InterfaceManagement GetDetail(string Name)
        {
            return RepositoryUtility.GetDetail<InterfaceManagement>(x => x.Name == Name);
        }

        /// <summary>讀取清單</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<InterfaceManagement> GetList()
        {
            Expression<Func<InterfaceManagement, bool>> filter = (x => true);

            return RepositoryUtility.GetList<InterfaceManagement>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(InterfaceManagement data)
        {
            return RepositoryUtility.Insert<InterfaceManagement>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(InterfaceManagement data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<InterfaceManagement>(ref sErrMsg, x => x.Name == data.Name)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.UnitName = data.UnitName;
            model.Url = data.Url;
            model.UpdatedAt = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string Name)
        {
            return RepositoryUtility.Delete<InterfaceManagement>(x => x.Name == Name);
        }
    }
}
