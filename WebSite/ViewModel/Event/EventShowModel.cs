using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    public class EventShowModel
    {
        public EventQueryModel QueryData { get; set; } = new EventQueryModel();
        public List<activityPlan> PlanData { get; set; }
        public List<defCode> EventType { get; set; } = Service_defCode.GetList("ActivityType");
        public int TotalPage { get; set; }
        public List<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
    }

    public class CalendarEvent
    {
        public string title { get; set; }
        public string url { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string backgroundColor { get; set; } = "#ffffcc";
        public string textColor { get; set; } = "#000000";
    }

    public class EventQueryModel
    {
        public int PageIndex { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EventType { get; set; } = "";
        public string KeyWord { get; set; } = "";
    }
    public class RegistShowModel
    {
        public activityPlan PlanData { get; set; }
        public RegistDataModel RegistData { get; set; } = new RegistDataModel();
    }

    public class RegistDataModel
    {
        public int ActId { get; set; }
        public List<int> PlanOpenTimeID { get; set; } = new List<int>();

        /// <summary> 姓名
        /// 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 身分證字號
        /// 
        /// </summary> 
        public string IdentifiedID { get; set; }

        /// <summary> 電話
        /// 
        /// </summary> 
        public string Phone { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 農藥管理人員證件號
        /// </summary>
        public string PesticideManagementStaffID { get; set; }

        /// <summary>
        /// 農藥管理人員證件過期日期
        /// </summary>
        public DateTime? PesticideManagementStaffExpiryDate { get; set; }

        /// <summary>
        /// 服務單位
        /// </summary>
        public string ServiceUnit { get; set; }

        /// <summary>
        /// 餐點種類 (0: 素 1: 葷)
        /// </summary>
        public bool? MealsType { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string ValidateCode { get; set; }
    }
    public class SignInModel
    {
        public int OpenId { get; set; }
        /// <summary>
        /// 活動與場次名稱
        /// </summary> 
        public string ActivityName { get; set; }
        /// <summary>
        /// 非報到時段
        /// </summary> 
        public bool WrongSignDate { get; set; }
        public string Url { get; set; } = "/Event/AllEvent";
        /// <summary>
        /// 報到成功
        /// </summary> 
        public bool SignSuccess { get; set; } = false;
        /// <summary>
        /// 錯誤訊息
        /// </summary> 
        public string ErrMsg { get; set; }
    }
}