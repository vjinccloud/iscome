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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ICCModule.EntityService.Service
{
    public class Service_SolarTermsPush
    {
        /// <summary>讀取清單</summary>
        /// <returns>
        /// 
        /// </returns>
        public static List<SolarTermsPush> GetList(Expression<Func<SolarTermsPush, bool>> filter = null)
        {
            if (filter == null) filter = (x => true);
            return RepositoryUtility.GetList<SolarTermsPush>(filter);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static SolarTermsPush GetDetail(int Id)
        {
            return RepositoryUtility.GetDetail<SolarTermsPush>(x => x.Id == Id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(SolarTermsPush data)
        {
            data.CreateDate = DateTime.Now;
            data.UpdateDate = DateTime.Now;
            return RepositoryUtility.Insert<SolarTermsPush>(data);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(SolarTermsPush data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<SolarTermsPush>(ref sErrMsg, x => x.Id == data.Id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.SolarTermsCode = data.SolarTermsCode;
            model.SolarTermsName = data.SolarTermsName;
            model.PushDate = data.PushDate;
            model.IsNeedPush = data.IsNeedPush;
            model.CropName = data.CropName;
            model.PestDisease = data.PestDisease;
            model.PushContents = data.PushContents;
            model.DataSubject = data.DataSubject;
            model.ReleaseDate = data.ReleaseDate;
            model.DataContents = data.DataContents;

            model.UpdateDate = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int Id)
        {
            List<FileManagement> files = Service_FileManagement.GetList("SolarTermsPushs", Id.ToString());
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
            return RepositoryUtility.Delete<SolarTermsPush>(x => x.Id == Id);
        }


        /// <summary>
        /// 匯入
        /// </summary>
        /// <returns></returns>
        public static BaseResult Import(List<SolarTermsPush> importData)
        {
            var errList = new List<string>();
            int iCount = 0, eCount = 0;
            foreach (var item in importData)
            {
                var newData = Insert(item);
                if (newData.result) iCount++;
                else
                {
                    errList.Add(newData.Msg);
                    eCount++;
                }
            }
            return new BaseResult(true, $"匯入成功，新增{iCount}筆，錯誤{eCount}筆，{string.Join(",", errList)}");
        }


        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdatePushed(int id)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<SolarTermsPush>(ref sErrMsg, x => x.Id == id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.IsPushed = true;
            model.UpdateDate = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

    }
}
