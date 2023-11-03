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
    public class Service_Link
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Link GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<Link>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<Link> GetList(bool? Show = null)
        {
            Expression<Func<Link, bool>> filter = (x => true);
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }

            return RepositoryUtility.GetList<Link>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(Link data)
        {
            return RepositoryUtility.Insert<Link>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(Link data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<Link>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Title = data.Title;
            model.Url = data.Url;
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
        public static BaseResult Delete(int ID)
        {
            List<FileManagement> files = Service_FileManagement.GetList("Links", ID.ToString());
            var fileLogic = new FileLogic();
            // 所有相關聯的檔案進行刪除
            foreach (var file in files)
            {
                // 需確認檔案是否存在
                if (fileLogic.IsFileExisted(file.FilePath))
                {
                    fileLogic.DeleteFile(file.FilePath);
                }
            }
            return RepositoryUtility.Delete<Link>(x => x.ID == ID);
        }
    }
}
