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
    public class Service_resumeProduct
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static IQueryable<resumeProductModel> GetList(Expression<Func<resumeProduct, bool>> filter = null, resumeProductReq rpFilter = null)
        {
            if (rpFilter == null) rpFilter = new resumeProductReq();
            BaseRepository Base = new BaseRepository();

            var res = from rp in Base.QueryData<resumeProduct>(filter)
                      join rpc in Base.QueryData<resumeProductCheck>(x => true)
                      on rp.ID equals rpc.ProductID into temp
                      from rpc in temp.DefaultIfEmpty()
                      where (rpFilter.TagResult == 0 ? true : (rpFilter.TagResult == 1 ? rpc.TagResult == true : (rpFilter.TagResult == 2 ? rpc.TagResult == false : rpc == null))) &&
                            (rpFilter.QuilityResult == 0 ? true : (rpFilter.QuilityResult == 1 ? rpc.QuilityResult == true : (rpFilter.QuilityResult == 2 ? rpc.QuilityResult == false : rpc == null))) &&
                            (!rpFilter.CheckData.HasValue ? true : ((rpFilter.CheckData ?? true) ? rpc != null : rpc == null)) &&(!rpFilter.CheckStartDate.HasValue || rpc.Date >= rpFilter.CheckStartDate) && (!rpFilter.CheckEndDate.HasValue || rpc.Date < (rpFilter.CheckEndDate??DateTime.Now).AddDays(1).Date)
                      group new { rp, rpc } by rp.ID into gp
                      select new resumeProductModel
                      {
                          ID = gp.FirstOrDefault().rp.ID,
                          OrganizationCode = gp.FirstOrDefault().rp.OrganizationCode,
                          VendorName = gp.FirstOrDefault().rp.VendorName,
                          ProducerName = gp.FirstOrDefault().rp.ProducerName,
                          Address = gp.FirstOrDefault().rp.Address,
                          Location = gp.FirstOrDefault().rp.Location,
                          VerificationAgency = gp.FirstOrDefault().rp.VerificationAgency,
                          VerificationItems = gp.FirstOrDefault().rp.VerificationItems,
                          LastCheckDate = gp.OrderByDescending(x => x.rpc.Date).FirstOrDefault().rpc.Date,
                          CreatedAt = gp.FirstOrDefault().rp.CreatedAt,
                          TagResult = gp.OrderByDescending(x => x.rpc.Date).FirstOrDefault().rpc.TagResult,
                          QuilityResult = gp.OrderByDescending(x => x.rpc.Date).FirstOrDefault().rpc.QuilityResult,
                      };

            return res;
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<resumeProductExportModel> GetExportList(Expression<Func<resumeProduct, bool>> filter = null, resumeProductReq rpFilter = null)
        {
            //查詢資料
            if (rpFilter == null) rpFilter = new resumeProductReq();
            BaseRepository Base = new BaseRepository();

            var res = from rp in Base.QueryData<resumeProduct>(filter)
                      join rpc in Base.QueryData<resumeProductCheck>(x => true)
                      on rp.ID equals rpc.ProductID into temp
                      from rpc in temp.DefaultIfEmpty()
                      where (rpFilter.TagResult == 0 ? true : (rpFilter.TagResult == 1 ? rpc.TagResult == true : (rpFilter.TagResult == 2 ? rpc.TagResult == false : rpc == null))) &&
                            (rpFilter.QuilityResult == 0 ? true : (rpFilter.QuilityResult == 1 ? rpc.QuilityResult == true : (rpFilter.QuilityResult == 2 ? rpc.QuilityResult == false : rpc == null))) &&
                            (!rpFilter.CheckData.HasValue ? true : ((rpFilter.CheckData ?? true) ? rpc != null : rpc == null)) && (!rpFilter.CheckStartDate.HasValue || rpc.Date >= rpFilter.CheckStartDate) && (!rpFilter.CheckEndDate.HasValue || rpc.Date < (rpFilter.CheckEndDate ?? DateTime.Now).AddDays(1).Date)
                      select new resumeProductExportModel
                      {
                          OrganizationCode = rp.OrganizationCode,
                          VendorName = rp.VendorName,
                          ProducerName = rp.ProducerName,
                          VerificationItems = rp.VerificationItems,
                          VerificationArea = rp.VerificationArea,
                          Lot = rp.Lot,
                          LandNum = rp.LandNum,
                          ExpirationDate = rp.ExpirationDate,
                          Date = rpc.Date,
                          TagResult = rpc.TagResult,
                          QuilityResult = rpc.QuilityResult,
                          ArbitrationInstructions = rpc.ArbitrationInstructions,
                      };

            return res.ToList();
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static resumeProduct GetDetail(int id)
        {
            return RepositoryUtility.GetDetail<resumeProduct>(x => x.ID == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(resumeProduct data)
        {
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;

            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(resumeProduct data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<resumeProduct>(ref sErrMsg, x => x.ID == data.ID).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.OrganizationCode = data.OrganizationCode;
            model.VendorName = data.VendorName;
            model.ProducerName = data.ProducerName;
            model.Principal = data.Principal;
            model.ContactPerson = data.ContactPerson;
            model.Address = data.Address;
            model.Phone = data.Phone;
            model.VerificationAgency = data.VerificationAgency;
            model.VerificationTypes = data.VerificationTypes;
            model.VerificationItems = data.VerificationItems;
            model.VerificationArea = data.VerificationArea;
            model.Lot = data.Lot;
            model.LandNum = data.LandNum;
            model.ExpirationDate = data.ExpirationDate;
            model.LastCheckDate = data.LastCheckDate;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult UpdateLocation(string ID, string Location)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<resumeProduct>(ref sErrMsg, x => x.ID == Convert.ToInt32(ID)).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Location = Location;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult UpdateLocationAddr(string oriAddr, string Location)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<resumeProduct>(ref sErrMsg, x => x.Address == oriAddr);
            foreach (var item in model)
            {
                item.Location = Location;
                item.UpdatedAt = DateTime.Now;
            }
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
            return RepositoryUtility.Delete<resumeProduct>(x => x.ID == ID);
        }

        /// <summary>
        /// 匯入
        /// </summary>
        /// <returns></returns>
        public static BaseResult Import(List<resumeProduct> data)
        {
            string sErrMsg = "";
            int iCount = 0, uCount = 0, eCount = 0;
            foreach (var item in data)
            {
                BaseRepository Base = new BaseRepository();
                //查詢資料
                var model = Base.QueryData<resumeProduct>(ref sErrMsg, x => x.Lot == item.Lot && x.LandNum == item.LandNum).SingleOrDefault();
                if (model == null)
                {
                    var iMsg = Insert(item);
                    if (iMsg.result) iCount++;
                    else eCount++;
                }
                else
                {
                    model.VendorName = model.VendorName;
                    model.ProducerName = model.ProducerName;
                    model.Address = model.Address;
                    model.Phone = model.Phone;
                    model.VerificationAgency = model.VerificationAgency;
                    model.VerificationTypes = model.VerificationTypes;
                    model.VerificationItems = model.VerificationItems;
                    model.VerificationArea = model.VerificationArea;
                    model.Lot = model.Lot;
                    model.LandNum = model.LandNum;
                    model.ExpirationDate = model.ExpirationDate;
                    model.UpdatedAt = DateTime.Now;

                    //更新回資料庫
                    if (Base.Update(ref sErrMsg)) uCount++;
                    else eCount++;
                }
            }
            return new BaseResult(true, $"匯入成功，新增{iCount}筆，更新{uCount}筆，錯誤{eCount}筆");
        }
    }
}
