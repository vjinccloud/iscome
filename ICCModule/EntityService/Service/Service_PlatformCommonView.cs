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
    public class Service_PlatformCommonView
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static PlatformCommonView GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<PlatformCommonView>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="GroupName"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<PlatformCommonView> GetList(string GroupName = "", bool? Show = null)
        {
            Expression<Func<PlatformCommonView, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(GroupName))
            {
                filter = filter.And(x => x.GroupName == GroupName);
            }
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }

            return RepositoryUtility.GetList<PlatformCommonView>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(PlatformCommonView data)
        {
            return RepositoryUtility.Insert<PlatformCommonView>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(PlatformCommonView data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<PlatformCommonView>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Title = data.Title;
            model.Context = data.Context;
            model.Show = data.Show;
            model.LastEditorName = data.LastEditorName;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        public static BaseResult UpdateClicks(int ID)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<PlatformCommonView>(ref sErrMsg, x => x.ID == ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Clicks += 1;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdateClick(long id )
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<PlatformCommonView>(ref sErrMsg, x => x.ID == id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Clicks += 1;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<PlatformCommonView>(x => x.ID == ID);
        }
    }
}
