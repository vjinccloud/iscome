using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Helper;
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
    public class Service_pesticideSeller
    {
        /// <summary>讀取清單</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<pesticideSeller> GetSellerList(Expression<Func<pesticideSeller, bool>> filter = null)
        {
            return RepositoryUtility.GetList<pesticideSeller>(filter);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static IQueryable<PesticideModel> GetList(Expression<Func<pesticideSeller, bool>> filter = null, int? checkResult = null, DateTime? checkStartDate = null, DateTime? checkEndDate = null, bool? checkData = null)
        {
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = from ps in Base.QueryData<pesticideSeller>(filter)
                        join pa in Base.QueryData<pesticideAudit>(x => (!checkStartDate.HasValue || x.Date >= checkStartDate) && (!checkEndDate.HasValue || x.Date.Date <= checkEndDate))
                        on ps.LicenseNum equals pa.SellerID into temp
                        from pa in temp.DefaultIfEmpty()
                        where (!checkResult.HasValue ? true : (checkResult == 1 ? pa.Result : (checkResult == 2 ? !pa.Result : (checkResult == 3 ? pa == null : true)))) &&
                                (!checkData.HasValue ? true : ((checkData ?? true) ? pa != null : pa == null))
                        group new { ps, pa } by ps.LicenseNum into gp
                        select new PesticideModel
                        {
                            LicenseNum = gp.Key,
                            VendorName = gp.FirstOrDefault().ps.VendorName,
                            VendorAddress = gp.FirstOrDefault().ps.VendorAddress,
                            Status = gp.FirstOrDefault().ps.Status,
                            ContactPhone = gp.FirstOrDefault().ps.ContactPhone,
                            LastCheckDate = gp.OrderByDescending(x => x.pa.Date).FirstOrDefault().pa.Date,
                            FriendlyStartDate = gp.FirstOrDefault().ps.FriendlyStartDate,
                            FriendlyEndDate = gp.FirstOrDefault().ps.FriendlyEndDate,
                            CreatedAt = gp.FirstOrDefault().ps.CreatedAt,
                            UpdatedAt = gp.FirstOrDefault().ps.UpdatedAt,
                            Result = gp.OrderByDescending(x => x.pa.Date).FirstOrDefault().pa.Result,
                        };

            return model;
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<PesticideExportModel> GetExportList(Expression<Func<pesticideSeller, bool>> filter = null, int? checkResult = null, DateTime? checkStartDate = null, DateTime? checkEndDate = null, bool? checkData = null)
        {
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = from ps in Base.QueryData<pesticideSeller>(filter)
                        join pa in Base.QueryData<pesticideAudit>(null)
                        on ps.LicenseNum equals pa.SellerID into temp
                        from pa in temp.DefaultIfEmpty()
                        where (!checkResult.HasValue || (checkResult == 1 ? pa.Result : true) || (checkResult == 2 ? !pa.Result : true) || (checkResult == 3 ? pa == null : true)) &&
                                (!checkStartDate.HasValue || pa.Date >= checkStartDate) && (!checkEndDate.HasValue || pa.Date.Date <= checkEndDate) && (!checkData.HasValue || ((checkData ?? true) ? pa != null : pa == null))
                        select new PesticideExportModel
                        {
                            LicenseNum = ps.LicenseNum,
                            VendorName = ps.VendorName,
                            VendorAddress = ps.VendorAddress,
                            Status = ps.Status,

                            Result = pa.Result,
                            Date = pa.Date,
                            Instructions = pa.Instructions,
                        };

            return model.ToList();
        }

        /// <summary>
        /// 讀取單筆
        /// </summary>
        /// <returns></returns>
        public static pesticideSeller GetDetail(string id)
        {
            return RepositoryUtility.GetDetail<pesticideSeller>(x => x.LicenseNum == id);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(pesticideSeller data)
        {
            if (string.IsNullOrEmpty(data.Location) && !string.IsNullOrEmpty(data.VendorAddress))
            {
                data.Location = GetLocation(data.VendorAddress);
                if (!string.IsNullOrEmpty(data.Location) && data.Location.Split(',').Count() == 2)
                {
                    var _coordinate = CoordinateTransHelper.TWD97_To_lonlat((data.Location.Split(',')[0]).ToDouble32(), (data.Location.Split(',')[1]).ToDouble32(), 2);
                    data.Latitude = _coordinate.Split(',')[1].ToDecimal32();
                    data.Longitude = _coordinate.Split(',')[0].ToDecimal32();
                }
            }
            data.CreatedAt = DateTime.Now;
            data.UpdatedAt = DateTime.Now;
            return RepositoryUtility.Insert(data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public static BaseResult Update(pesticideSeller data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<pesticideSeller>(ref sErrMsg, x => x.LicenseNum == data.LicenseNum).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            if ((string.IsNullOrEmpty(model.Location) || (model.VendorAddress != data.VendorAddress && model.Location == data.Location)) && !string.IsNullOrEmpty(data.VendorAddress))
            {
                model.Location = GetLocation(data.VendorAddress);

                if (!string.IsNullOrEmpty(model.Location) && model.Location.Split(',').Count() ==2)
                {
                    var _coordinate = CoordinateTransHelper.TWD97_To_lonlat((model.Location.Split(',')[0]).ToDouble32(), (model.Location.Split(',')[1]).ToDouble32(), 2);
                    model.Latitude = _coordinate.Split(',')[1].ToDecimal32();
                    model.Longitude = _coordinate.Split(',')[0].ToDecimal32();
                }
            }
            model.VendorName = data.VendorName;
            model.VendorAddress = data.VendorAddress;
            model.Status = data.Status;
            model.ContactPhone = data.ContactPhone;
            model.LastCheckDate = data.LastCheckDate;
            model.FriendlyStartDate = data.FriendlyStartDate;
            model.FriendlyEndDate = data.FriendlyEndDate;
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 更新經緯度
        /// </summary>
        /// <returns></returns>
        public static BaseResult UpdateLocation(pesticideSeller pSeller)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<pesticideSeller>(ref sErrMsg, x => x.LicenseNum == pSeller.LicenseNum).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Location = pSeller.Location;
            model.Longitude = pSeller.Longitude;
            model.Latitude = pSeller.Latitude;
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
        public static BaseResult Delete(string ID)
        {
            return RepositoryUtility.Delete<pesticideSeller>(x => x.LicenseNum == ID);
        }

        /// <summary>
        /// 匯入
        /// </summary>
        /// <returns></returns>
        public static BaseResult Import(List<pesticideSeller> data)
        {
            string sErrMsg = "";
            int iCount = 0, uCount = 0, eCount = 0;
            foreach (var item in data)
            {
                BaseRepository Base = new BaseRepository();
                //查詢資料
                var model = Base.QueryData<pesticideSeller>(ref sErrMsg, x => x.LicenseNum == item.LicenseNum).SingleOrDefault();
                if (model == null)
                {
                    if (Insert(item).result) iCount++;
                    else eCount++;
                }
                else
                {
                    model.VendorName = item.VendorName;
                    model.VendorAddress = item.VendorAddress;
                    model.Status = item.Status;
                    model.ContactPhone = item.ContactPhone;
                    model.UpdatedAt = DateTime.Now;

                    //更新回資料庫
                    if (Base.Update(ref sErrMsg)) uCount++;
                    else eCount++;
                }
            }
            return new BaseResult(true, $"匯入成功，新增{iCount}筆，更新{uCount}筆，錯誤{eCount}筆");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetLocation(string addr)
        {
            string Which_MAP = AppSettingHelper.GetAppSetting("Which_MAP");
            switch (Which_MAP)
            {
                case "Google":
                    var geocodeResult = new GoogleMapGeocodeHelper().GeocodeAddressNa(addr);
                    if (geocodeResult.status == "OK" && geocodeResult.results.Count > 0)
                    {
                        var location = geocodeResult.results.FirstOrDefault().geometry.location;
                        return $"{location.lat},{location.lng}";
                    }

                    break;
                case "TGOS":
                    var tgosResult = new TGOSHelper().QueryAddressNa(addr);
                    return tgosResult;
            }
            return "";
        }
    }
}
