using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using ICCModule.ViewModel;
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
    public class Service_monitorAreaPoint
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<monitorAreaPoint> GetList(Expression<Func<monitorAreaPoint, bool>> filter = null)
        {
            return RepositoryUtility.GetList<monitorAreaPoint>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static monitorAreaPoint GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<monitorAreaPoint>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(monitorAreaPoint data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(monitorAreaPoint data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorAreaPoint>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            //更新內容
            model.Village = data.Village;
            model.SerialNum = data.SerialNum;
            model.NorthLatitude = data.NorthLatitude;
            model.EastLongitude = data.EastLongitude;
            model.CropType = data.CropType;
            model.Crops = data.Crops;
            model.Comment = "";
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<monitorAreaPoint>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<monitorAreaPoint, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<monitorAreaPoint>(filter);
        }


        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult InsertOrUpdate(monitorAreaPoint data, monitorAreaPointDate dataDate)
        {
            bool rslt = true;
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorAreaPoint>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                data.CreatedAt = DateTime.Now;
                data.UpdatedAt = DateTime.Now;
                RepositoryUtility.Insert(data);

                dataDate.AreaPointID = data.ID;
                dataDate.CreatedAt = DateTime.Now;
                dataDate.UpdatedAt = DateTime.Now;
                RepositoryUtility.Insert(dataDate);
            }
            else
            {
                model.Village = data.Village;
                model.SerialNum = data.SerialNum;
                model.NorthLatitude = data.NorthLatitude;
                model.EastLongitude = data.EastLongitude;
                model.CropType = data.CropType;
                model.Crops = data.Crops;
                model.UpdatedAt = DateTime.Now;
                //更新回資料庫
                rslt = Base.Update(ref sErrMsg);
            }

            BaseRepository Base2 = new BaseRepository();
            var modelDate = Base2.QueryData<monitorAreaPointDate>(ref sErrMsg, x => x.ID == dataDate.ID).SingleOrDefault();
            if (modelDate == null)
            {
                dataDate.AreaPointID = data.ID;
                dataDate.CreatedAt = DateTime.Now;
                dataDate.UpdatedAt = DateTime.Now;
                RepositoryUtility.Insert(dataDate);
            }
            else
            {
                modelDate.Date = dataDate.Date;
                modelDate.Pests = dataDate.Pests;
                modelDate.Surveys = dataDate.Surveys;
                modelDate.Victims = dataDate.Victims;
                modelDate.CropGrowthPeriod = dataDate.CropGrowthPeriod;
                modelDate.SurveyArea = dataDate.SurveyArea;
                modelDate.DiscoverFAW = dataDate.DiscoverFAW;
                modelDate.DamageContent = dataDate.DamageContent;
                modelDate.Adress = dataDate.Adress;
                modelDate.OtherArts = dataDate.OtherArts;
                modelDate.OtherArtsNum = dataDate.OtherArtsNum;
                modelDate.Tempeature = dataDate.Tempeature;
                modelDate.Level_0 = dataDate.Level_0;
                modelDate.Level_1 = dataDate.Level_1;
                modelDate.Level_2 = dataDate.Level_2;
                modelDate.Level_3 = dataDate.Level_3;
                modelDate.Level_4 = dataDate.Level_4;
                modelDate.Comment = dataDate.Comment;
                modelDate.MonitoringRecord = dataDate.MonitoringRecord;
                modelDate.DailyRainfall = dataDate.DailyRainfall;
                modelDate.UpdatedAt = DateTime.Now;
                Base2.Update(ref sErrMsg);
            }

            return new BaseResult(rslt, sErrMsg);
        }

    }
}
