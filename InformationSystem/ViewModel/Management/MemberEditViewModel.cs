using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using InformationSystem.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class MemberEditViewModel
    {
        public MemberEditViewModel()
        {
            Districts = CommonDataHelper.GetDistricts("高雄市");
            defCodes = Service_defCode.GetList("UserIdentify", "N");
        }
        /// <summary>
        /// 定義列表
        /// </summary>
        public List<defCode> defCodes { get; set; }

        /// <summary>
        /// 帳號新增
        /// </summary>
        public memberInfo memberInfo { get; set; }

        /// <summary>
        /// 行政區列表
        /// </summary>
        public List<District> Districts { get; set; }

        /// <summary>
        /// 帳號檢查
        /// </summary>
        public bool isRe { get; set; }
    }
}