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
    public class Service_pesticideResidueTesting
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<pesticideResidueTesting> GetList(Expression<Func<pesticideResidueTesting, bool>> filter = null)
        {
            return RepositoryUtility.GetList<pesticideResidueTesting>(filter);
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static pesticideResidueTesting GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<pesticideResidueTesting>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(pesticideResidueTesting data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(pesticideResidueTesting data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<pesticideResidueTesting>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.ID = data.ID;
            model.Year = data.Year;
            model.PlanType = data.PlanType;
            model.PlanComment = data.PlanComment;
            model.SampleNum = data.SampleNum;
            model.SampleDate = data.SampleDate;
            model.City = data.City;
            model.District = data.District;
            model.SampleName = data.SampleName;
            model.CropType = data.CropType;
            model.CropName = data.CropName;
            model.ProviderName = data.ProviderName;
            model.ProviderCode = data.ProviderCode;
            model.ProviderCity = data.ProviderCity;
            model.ProviderDistrict = data.ProviderDistrict;
            model.ProviderAddress = data.ProviderAddress;
            model.ProviderPhone = data.ProviderPhone;
            model.SampleSource = data.SampleSource;
            model.SampleLocation = data.SampleLocation;
            model.ProvideUnit = data.ProvideUnit;
            model.SampleResult = data.SampleResult;
            model.SampleContext = data.SampleContext;
            model.Result = data.Result;
            model.HandingSituation = data.HandingSituation;
            model.ClosingSituation = data.ClosingSituation;
            model.Penalty = data.Penalty;
            model.Staging = data.Staging;
            model.PaymentDeadline = data.PaymentDeadline;
            model.PaymentStatus = data.PaymentStatus;
            model.StagingData = data.StagingData;
            model.TransferDate = data.TransferDate;
            model.ClosingDate = data.ClosingDate;
            model.ClosingNumber = data.ClosingNumber;
            model.ClosingInstructions = data.ClosingInstructions;
            model.IsTransfer = data.IsTransfer;
            model.TransferCounselingDate = data.TransferCounselingDate;
            model.UpdatedAt = DateTime.Now;
            model.AnalyzeDate = data.AnalyzeDate;
            model.SampleID = data.SampleID;
            model.FastSampleID = data.FastSampleID;
            model.JointUnit = data.JointUnit;
            model.ProviderUnit = data.ProviderUnit;

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
            return RepositoryUtility.Delete<pesticideResidueTesting>(x => x.ID == ID);
        }
        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteMany(Expression<Func<pesticideResidueTesting, bool>> filter)
        {
            return RepositoryUtility.DeleteAll<pesticideResidueTesting>(filter);
        }

        /// <summary>
        /// 匯入
        /// </summary>
        /// <returns></returns>
        public static BaseResult Import(List<pesticideResidueTesting> importData)
        {
            var errList = new List<string>();
            string sErrMsg = "";
            int iCount = 0, uCount = 0, eCount = 0;
            foreach (var item in importData)
            {
                BaseRepository Base = new BaseRepository();
                //查詢資料
                var model = Base.QueryData<pesticideResidueTesting>(ref sErrMsg, x => x.SampleNum == item.SampleNum).SingleOrDefault();
                if (model == null)
                {
                    var newData = Insert(item);
                    if (newData.result) iCount++;
                    else
                    {
                        errList.Add(newData.Msg);
                        eCount++;
                    }
                }
                else
                {
                    model.Year = model.Year;
                    model.PlanType = model.PlanType;
                    model.SampleNum = model.SampleNum;
                    model.SampleDate = model.SampleDate;
                    model.District = model.District;
                    model.SampleName = model.SampleName;
                    model.CropType = model.CropType;
                    model.City = model.City;
                    model.ProviderCity = model.ProviderCity;
                    model.ProvideUnit = model.ProvideUnit;

                    model.CropName = model.CropName;
                    model.ProviderName = model.ProviderName;
                    model.SampleSource = model.SampleSource;
                    model.SampleResult = model.SampleResult;
                    model.HandingSituation = model.HandingSituation;

                    model.Penalty = model.Penalty;
                    model.PaymentDeadline = model.PaymentDeadline;
                    model.TransferDate = model.TransferDate;
                    model.ClosingDate = model.ClosingDate;
                    model.ClosingNumber = model.ClosingNumber;

                    model.ClosingInstructions = model.ClosingInstructions;
                    model.TransferCounselingDate = model.TransferCounselingDate;
                    model.Result = model.Result;
                    model.HandingSituation = model.HandingSituation;
                    model.Staging = model.Staging;

                    model.PaymentStatus = model.PaymentStatus;
                    model.UpdatedAt = DateTime.Now;

                    //更新回資料庫
                    var updData = Base.Update(ref sErrMsg);
                    if (updData) uCount++;
                    else
                    {
                        errList.Add(sErrMsg);
                        eCount++;
                    }
                }
            }
            return new BaseResult(true, $"匯入成功，新增{iCount}筆，更新{uCount}筆，錯誤{eCount}筆，{string.Join(",",errList)}");
        }
    }
}
