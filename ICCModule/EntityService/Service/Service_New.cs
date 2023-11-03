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
    public class Service_New
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static New GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<New>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <param name="KeyWord">主旨關鍵字</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<New> GetList(string Code = "", bool? Show = null, string KeyWord = "")
        {
            Expression<Func<New, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(Code))
            {
                filter = filter.And(x => x.Code == Code);
            }
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }
            if (!string.IsNullOrEmpty(KeyWord))
            {
                filter = filter.And(x => x.Title.Contains(KeyWord));
            }

            return RepositoryUtility.GetList<New>(filter);
        }
        
        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <param name="KeyWord">主旨關鍵字</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<New> GetList(Expression<Func<New, bool>> filter = null)
        {
            return RepositoryUtility.GetList<New>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(New data)
        {
            if (data.Show && data.PopupShow) CheckPopup();
            return RepositoryUtility.Insert<New>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(New data)
        {
            string sErrMsg = "";

            if (data.Show && data.PopupShow) CheckPopup();
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<New>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Title = data.Title;
            model.Code = data.Code;
            model.Context = data.Context;
            model.Show = data.Show;
            model.PopupShow = data.PopupShow;
            model.StartDate = data.StartDate;
            model.EndDate = data.EndDate;
            model.UpdatedAt = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        public static void CheckPopup()
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var popupShowData = Base.QueryData<New>(x => x.PopupShow).ToList();

            foreach (var item in popupShowData)
            {
                item.Show = false;
                item.PopupShow = true;
                item.UpdatedAt = DateTime.Now;
            }
            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdateClicks(int ID)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<New>(ref sErrMsg, x => x.ID == ID)
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
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            //string sql = @"SELECT * FROM FileManagement WHERE TableName = @TableName AND TableID = @TableID";
            //List<RepositoryUtility.InputDBParam> input = new List<RepositoryUtility.InputDBParam>();
            //input.Add(new RepositoryUtility.InputDBParam()
            //{
            //    Name = "TableName",
            //    type = System.Data.SqlDbType.VarChar,
            //    Value = "News"
            //});
            //input.Add(new RepositoryUtility.InputDBParam()
            //{
            //    Name = "TableID",
            //    type = System.Data.SqlDbType.VarChar,
            //    Value = "13"
            //});
            //List<FileManagement> files =  RepositoryUtility.EXEC_SQL_toList<FileManagement>(sql, input);

            List<FileManagement> files = Service_FileManagement.GetList("News", ID.ToString());
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

            return RepositoryUtility.Delete<New>(x => x.ID == ID);
        }
    }
}
