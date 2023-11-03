using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using InformationSystem.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class MemberCheckViewModel
    {
        public MemberCheckViewModel()
        {
            defCodes = Service_sysRole.GetList("", "");
            defCodesIdentify = Service_defCode.GetList("UserIdentify"); ;
            Districts = CommonDataHelper.GetDistricts("高雄市");
        }

        /// <summary>
        /// 定義列表
        /// </summary>
        public List<sysRole> defCodes { get; set; }
        
        /// <summary>
        /// 前台角色別
        /// </summary>
        public List<defCode> defCodesIdentify { get; set; }

        /// <summary>
        /// 行政區列表
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 帳號狀態 前台對應 Status: Y/N 後台對應: N 正常 D 停用 L 鎖定
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 帳號 前台對應 Account 後台對應 LoginID
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 電子信箱 前台對應 Email 後台對應 EmailAddress
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 姓名 前台對應 Name 後台對應 UserName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色代碼 前台對應 RoleCode 後台對應 RoleID
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 城市 前台對應 City 後台固定 高雄市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 行政區 前台對應 District 後取出 Zip 後台對應 District = Zip
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// 性別 前台對應 Sexy 後台固定 Sexy
        /// </summary>
        public string Sexy { get; set; }

        /// <summary>
        /// 民國年 前台對應 Year 後台固定 YearOfBirth , 需要加上 1911 
        /// </summary>
        public string YearOfBirth { get; set; }

        /// <summary>
        /// 行動電話 前台對應 Mobile 後台固定 Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 身分證 前台對應 Sexy 後台 無
        /// </summary>
        public string IdentifyID { get; set; }

        /// <summary>
        /// 來源
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 前台還是後台
        /// </summary>
        public bool IsFront { get; set; }
    }
}