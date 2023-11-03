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
    public class Service_VW_Search
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<VW_Search> GetList(string KeyName = "")
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<Tag>(ref sErrMsg, x => x.KeyName == KeyName)
                .SingleOrDefault();
            if (model == null)
            {
                Tag TagAdd = new Tag();
                TagAdd.KeyName = KeyName;
                TagAdd.Searches = 1;
                TagAdd.Show = true;
                TagAdd.CreatedAt = DateTime.Now;
                RepositoryUtility.Insert<Tag>(TagAdd);
            }
            else
            {
                model.Searches = model.Searches + 1;
                model.UpdatedAt = DateTime.Now;

                // TODO: 未實作，保存關聯檔案，需有 MD5

                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }


            Expression<Func<VW_Search, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(KeyName))
            {
                filter = filter.And(x => x.Context.Contains(KeyName));
            }

            return RepositoryUtility.GetList<VW_Search>(filter);
        }

        //public BaseResult TagUpdate(string KeyName = "")
        //{
        //    string sErrMsg = "";

        //    BaseRepository Base = new BaseRepository();
        //    //查詢資料
        //    var model = Base
        //        .QueryData<Tag>(ref sErrMsg, x => x.KeyName == KeyName)
        //        .SingleOrDefault();
        //    if (model == null)
        //    {
        //        Tag TagAdd = new Tag();
        //        TagAdd.KeyName = KeyName;
        //        TagAdd.Searches = 1;
        //        TagAdd.Show = true;
        //        TagAdd.CreatedAt = DateTime.Now;
        //        return RepositoryUtility.Insert<Tag>(TagAdd);
        //    }

        //    model.Searches = model.Searches + 1;
        //    model.UpdatedAt = DateTime.Now;

        //    // TODO: 未實作，保存關聯檔案，需有 MD5

        //    //更新回資料庫
        //    bool rslt = Base.Update(ref sErrMsg);
        //    return new BaseResult(rslt, sErrMsg);
        //}
    }
}
