using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.RequestModel;
using System.Collections.Generic;
using Website.Helper;

namespace Website.ViewModel.Member
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            List<defCode> _Types = Service_defCode.GetList("UserIdentify");
            Types = _Types;
            CityDistricts = CommonDataHelper.GetCityDistricts();
        }

        /// <summary>
        /// 身分別
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 縣市，行政區，以縣市為索引
        /// </summary>
        public List<CityDistricts> CityDistricts { get; set; }

        /// <summary>
        /// 會員資訊
        /// </summary>
        public memberInfo Info { get; set; }
        
        /// <summary>
        /// 會員資訊
        /// </summary>
        public FastRegistReq FastInfo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 驗證方式
        /// </summary>
        public string VerifyMethod { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 二次確認密碼
        /// </summary>
        public string CheckPwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsErr { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; set; }
        public bool? IsFastSign { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FastRegistReq
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string LoginID { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 所在縣市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 所在區域
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 驗證密碼
        /// </summary>
        public string CheckPwd { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 
        /// 
        /// </summary>
        public string GoogleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FacebookId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LineLoginId { get; set; }
    }

    public class LineSignUpViewModel
    {
        public LineSignUpViewModel()
        {
            List<defCode> _Types = Service_defCode.GetList("UserIdentify");
            Types = _Types;
            CityDistricts = CommonDataHelper.GetCityDistricts();
            Info = new LineRegist();
        }

        /// <summary>
        /// 身分別
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 縣市，行政區，以縣市為索引
        /// </summary>
        public List<CityDistricts> CityDistricts { get; set; }

        /// <summary>
        /// 會員資訊
        /// </summary>
        public LineRegist Info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsErr { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsClose { get; set; }
    }
}