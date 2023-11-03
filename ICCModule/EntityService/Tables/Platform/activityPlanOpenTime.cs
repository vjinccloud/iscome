using IscomG2C.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "activity_Plan_Open_Times")]
    public class activityPlanOpenTime
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 活動規劃ID
        /// 
        /// </summary> 
        [Column]
        public long ActivityPlanID { get; set; }

        /// <summary> 場次名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary>
        /// 場次日期
        /// </summary>
        [Column]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string DateStr
        {
            get
            {
                return Date.ToString("yyyy-MM-dd");
            }
            set
            {
                Date = Convert.ToDateTime(String.Concat(value, " 00:00:00"));
            }
        }

        /// <summary>
        /// 取回民國年的開始日期
        /// </summary>
        public string ChineseDateStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(Date, "yyy/M/dd"); }
        }

        /// <summary> 時間起迄
        /// 
        /// </summary> 
        [Column]
        public string TimeBetween { get; set; }

        

        

        //public string TimeStr
        //{
        //    get
        //    {
        //        return Datetime.ToString("HH:mm:ss");
        //    }

        //    set
        //    {
        //        string date = Datetime.ToString("yyyy/MM/dd");

        //        Datetime = Convert.ToDateTime(String.Concat(date, " ", value));
        //    }
        //}

        //public string HourStr
        //{
        //    get
        //    {
        //        return Datetime.ToString("HH");
        //    }

        //    set
        //    {
        //        string date = Datetime.ToString("yyyy/MM/dd");
        //        string minuteSecond = Datetime.ToString("mm:ss");

        //        Datetime = Convert.ToDateTime(String.Concat(date, " ", value, ":", minuteSecond));
        //    }
        //}

        //public string MinuteStr
        //{
        //    get
        //    {
        //        return Datetime.ToString("mm");
        //    }

        //    set
        //    {
        //        string date = Datetime.ToString("yyyy/MM/dd");
        //        string hour = Datetime.ToString("HH");

        //        Datetime = Convert.ToDateTime(String.Concat(date, " ", hour, ":", value, ":00"));
        //    }
        //}

        /// <summary> 人數
        /// 
        /// </summary> 
        [Column]
        public int Nums { get; set; }

        /// <summary> 已報名數
        /// 
        /// </summary> 
        [Column]
        public int RegisteredNums { get; set; }

        /// <summary> 場次狀態
        /// 
        /// </summary> 
        [Column]
        public int Status { get; set; }

        /// <summary> 出席人數
        /// 
        /// </summary> 
        [Column]
        public int Attendance { get; set; }

        /// <summary> 簽到成果照片檔
        /// 
        /// </summary> 
        [Column]
        public string SignInResult { get; set; }

        /// <summary> 成果照片檔
        /// 
        /// </summary> 
        [Column]
        public string Pictures { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }
    }
}
