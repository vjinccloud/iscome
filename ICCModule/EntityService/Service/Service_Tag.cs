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
    public class Service_Tag
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public static Tag GetDetail(string KeyName)
        {
            return RepositoryUtility.GetDetail<Tag>(x => x.KeyName == KeyName);
        }
        
        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<Tag> GetList(bool? Show = null)
        {
            Expression<Func<Tag, bool>> filter = (x => true);
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }

            return RepositoryUtility.GetList<Tag>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(Tag data)
        {
            return RepositoryUtility.Insert<Tag>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(Tag data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<Tag>(ref sErrMsg, x => x.KeyName == data.KeyName)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Searches = data.Searches;
            model.Show = data.Show;
            model.UpdatedAt = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(string KeyName)
        {
            return RepositoryUtility.Delete<Tag>(x => x.KeyName == KeyName);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<Tag> GetList_Range(bool? Show = null)
        {
            Expression<Func<Tag, bool>> filter = (x => true);
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }

            String Error = "";
            PagerInfo page = new PagerInfo();
            page.m_iPageCount = 7;
            return RepositoryUtility.GetList_Range<Tag,Int32>(ref Error, page, filter,x=> x.Searches, false);
        }
    }
}
