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
    /// 作物
    /// </summary>
    public class Service_memberInventory
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<memberInventory> GetList(Expression<Func<memberInventory, bool>> filter = null)
        {
            return RepositoryUtility.GetList<memberInventory>(filter);
        }
        
        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static memberInventory GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<memberInventory>(x => x.Id == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(memberInventory data)
        {
            data.CreateDate = DateTime.Now;
            data.ModifyDate = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(memberInventory data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<memberInventory>(ref sErrMsg, x => x.Id == data.Id).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.LicenseWord = data.LicenseWord;
            model.LicenseNumber = data.LicenseNumber;
            model.ChineseName = data.ChineseName;
            model.Contents = data.Contents;
            model.DosageForm = data.DosageForm;
            model.BrandName = data.BrandName;
            model.TradeName = data.TradeName;
            model.ValidityPeriod = data.ValidityPeriod;
            model.ModifyDate = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int Id)
        {
            return RepositoryUtility.Delete<memberInventory>(x => x.Id == Id);
        }
    }
}
