using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_FileManagement
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static FileManagement GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<FileManagement>(x => x.ID == ID);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<FileManagement> GetList(string TableName = "", string TableID = "")
        {
            Expression<Func<FileManagement, bool>> filter = (x => x.TableID != "0");
            if (!string.IsNullOrEmpty(TableName))
            {
                filter = filter.And(x => x.TableName.ToString() == TableName.ToString());
            }
            if (!string.IsNullOrEmpty(TableID))
            {
                filter = filter.And(x => x.TableID.ToString() == TableID.ToString());
            }
            return RepositoryUtility.GetList<FileManagement>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(FileManagement data)
        {
            var baseResult = RepositoryUtility.Insert<FileManagement>(data);
            return baseResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Update(FileManagement data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<FileManagement>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.FileName = data.FileName;
            model.FilePath = data.FilePath;
            model.UploadTime = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult UpdateId(FileManagement data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<FileManagement>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.TableID = data.TableID;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            FileManagement file = GetDetail(ID);
            if (file != null)
            {
                // 刪除指定路徑檔案
                FileLogic fileLogic = new FileLogic();
                fileLogic.DeleteFile(file.FilePath);
            }

            return RepositoryUtility.Delete<FileManagement>(x => x.ID == ID);
        }
        /// <summary>
        /// 刪除
        /// </summary>
        public static void DeleteOldFiles(string TableName = "", string TableID = "",string key ="")
        {
            Expression<Func<FileManagement, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(TableName))
            {
                filter = filter.And(x => x.TableName.ToString() == TableName.ToString());
            }
            if (!string.IsNullOrEmpty(TableID))
            {
                filter = filter.And(x => x.TableID.ToString() == TableID.ToString());
            }
            if (!string.IsNullOrEmpty(key))
            {
                filter = filter.And(x => x.FilePath.Contains(key));
            }
            var files = RepositoryUtility.GetList<FileManagement>(filter);
            foreach (var file in files)
            {
                FileLogic fileLogic = new FileLogic();
                fileLogic.DeleteFile(file.FilePath);

                RepositoryUtility.Delete<FileManagement>(x => x.ID == file.ID);
            }
        }
    }
}
