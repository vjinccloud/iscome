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
    public class Service_BusinessForm
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static BusinessForm GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<BusinessForm>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Code"></param>
        /// <param name="Show">上、下架</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<BusinessForm> GetList(List<string> Codes = null, bool? Show = null)
        {
            Expression<Func<BusinessForm, bool>> filter = (x => true);
            if (Codes != null)
            {
                int i = 0;
                foreach (string code in Codes)
                {
                    if (i == 0)
                    {
                        filter = filter.And(x => x.Codes.Contains(code));
                    }
                    else
                    {
                        filter = filter.Or(x => x.Codes.Contains(code));
                    }
                }
            } 
            if (Show != null)
            {
                filter = filter.And(x => x.Show == Show);
            }

            return RepositoryUtility.GetList<BusinessForm>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(BusinessForm data)
        {
            return RepositoryUtility.Insert<BusinessForm>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(BusinessForm data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<BusinessForm>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Codes = data.Codes;
            model.FileName = data.FileName;
            model.Show = data.Show;
            if (data.FilePath != null)
            {
                model.FilePath = data.FilePath;
            }
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
            List<FileManagement> files = Service_FileManagement.GetList("BusinessForms", ID.ToString());
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

            return RepositoryUtility.Delete<BusinessForm>(x => x.ID == ID);
        }
    }
}
