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
    /// 監測專案
    /// </summary>
    public class Service_monitorProject
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<monitorProject> GetList(Expression<Func<monitorProject, bool>> filter = null)
        {
            return RepositoryUtility.GetList<monitorProject>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static monitorProject GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<monitorProject>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(monitorProject data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(monitorProject data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<monitorProject>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            //更新內容
            model.Year = data.Year;
            model.Name = data.Name;
            model.Frequency = data.Frequency;
            model.StartDate = data.StartDate;
            model.EndDate = data.EndDate;
            model.Show = data.Show;
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
            return RepositoryUtility.Delete<monitorProject>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<monitorProject, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<monitorProject>(filter);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<MonitorProjectModel> GetViewList(Expression<Func<monitorProject, bool>> filter = null)
        {
            BaseRepository Base = new BaseRepository();
            var _res = from mp in Base.QueryData<monitorProject>(filter)
                       join mpa in Base.QueryData<monitorProjectArea>(null)
                       on mp.ID equals mpa.ProjectID
                       join map in Base.QueryData<monitorAreaPoint>(null)
                       on mpa.ID equals map.ProjectAreaID
                       join mapd in Base.QueryData<monitorAreaPointDate>(null)
                       on map.ID equals mapd.AreaPointID
                       join cp in Base.QueryData<crops>(null)
                       on map.Crops equals cp.ID into temp
                       from cp in temp.DefaultIfEmpty()
                       join ct in Base.QueryData<cropType>(null)
                       on map.CropType equals ct.ID into temp2
                       from ct in temp2.DefaultIfEmpty()
                       select new MonitorProjectModel
                       {
                           ProjectName = mp.Name,
                           Distist = mpa.Distist,

                           Village = map.Village,
                           SerialNum = map.SerialNum,
                           NorthLatitude = map.NorthLatitude,
                           EastLongitude = map.EastLongitude,
                           CropType = ct.Name,
                           Crops = cp.Name,

                           Date = mapd.Date,
                           Pests = mapd.Pests,
                           Surveys = mapd.Surveys,
                           Victims = mapd.Victims,
                           CropGrowthPeriod = mapd.CropGrowthPeriod,
                           SurveyArea = mapd.SurveyArea,
                           DiscoverFAW = mapd.DiscoverFAW,
                           DamageContent = mapd.DamageContent,
                           Adress = mapd.Adress,
                           OtherArts = mapd.OtherArts,
                           OtherArtsNum = mapd.OtherArtsNum,
                           Tempeature = mapd.Tempeature,
                           Level_0 = mapd.Level_0,
                           Level_1 = mapd.Level_1,
                           Level_2 = mapd.Level_2,
                           Level_3 = mapd.Level_3,
                           Level_4 = mapd.Level_4,
                           Comment = mapd.Comment,
                       };
            return _res.ToList();
        }
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<MonitorProjectSelectModel> GetSelectList(Expression<Func<monitorProject, bool>> filter = null, Expression<Func<monitorAreaPointDate, bool>> filterDate = null)
        {
            BaseRepository Base = new BaseRepository();
            var _res = from mp in Base.QueryData<monitorProject>(filter)
                       join mpa in Base.QueryData<monitorProjectArea>(null)
                       on mp.ID equals mpa.ProjectID
                       join map in Base.QueryData<monitorAreaPoint>(null)
                       on mpa.ID equals map.ProjectAreaID
                       join mapd in Base.QueryData<monitorAreaPointDate>(filterDate)
                       on map.ID equals mapd.AreaPointID
                       join cp in Base.QueryData<crops>(null)
                       on map.Crops equals cp.ID into temp
                       from cp in temp.DefaultIfEmpty()
                       join ct in Base.QueryData<cropType>(null)
                       on map.CropType equals ct.ID into temp2
                       from ct in temp2.DefaultIfEmpty()
                       select new MonitorProjectSelectModel
                       {
                           Distist = mpa.Distist,
                           Village = map.Village,
                           SerialNum = map.SerialNum,
                           Crops = cp.Name,
                           Pests = mapd.Pests,
                           StartDate = mp.StartDate,
                           EndDate = mp.EndDate,
                       };
            return _res.ToList();
        }
    }
}
