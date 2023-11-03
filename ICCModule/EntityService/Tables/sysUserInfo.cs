using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "sysUserInfo")]
    public class sysUserInfo
    {
        public sysUserInfo()
        {
            this.AddDate = DateTime.Now;
            this.EditDate = DateTime.Now;
            this.VerifyMode = "";
        }
        /// <summary> 登入ID
        ///
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public String LoginID { get; set; }
        /// <summary> 登入密碼
        ///
        /// </summary>
        [Column]
        public String LoginPass { get; set; }
        /// <summary> 使用者姓名
        ///
        /// </summary>
        [Column]
        public String UserName { get; set; }
        /// <summary>
        /// 單位名稱
        /// </summary>
        [Column(CanBeNull = true)]
        public string UnitName { get; set; }
        /// <summary>
        /// 行動電話
        /// </summary>
        [Column(CanBeNull = true)]
        public string Mobile { get; set; }
        /// <summary>
        /// 行政區
        /// </summary>
        [Column(CanBeNull = true)]
        public string District { get; set; }
        /// <summary>
        /// 選擇區域
        /// </summary>
        public List<string> SelectDistrict { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        [Column(CanBeNull = true)]
        public string Sexy { get; set; }
        /// <summary>
        /// 出生年
        /// </summary>
        [Column(CanBeNull = true)]
        [DataType(DataType.Date)]
        public DateTime? YearOfBirth { get; set; }
        /// <summary> (無用欄位)
        ///
        /// </summary>
        [Column]
        public String RoleID { get; set; }
        /// <summary> N:正常/D:停用/L:鎖定
        ///
        /// </summary>
        [Column]
        public String State { get; set; }
        /// <summary> 重設密碼的TokenID
        ///
        /// </summary>
        [Column]
        public String ResetTokenID { get; set; }
        /// <summary> 建立日期
        ///
        /// </summary>
        [Column]
        public DateTime AddDate { get; set; }
        /// <summary> 編輯日期
        ///
        /// </summary>
        [Column]
        public DateTime EditDate { get; set; }
        /// <summary> 登入錯誤累計次數
        ///
        /// </summary>
        [Column]
        public Int32 ErrorCount { get; set; }
        /// <summary> 最後一次變更密碼的日期
        ///
        /// </summary>
        [Column]
        public DateTime? LastChangePWDate { get; set; }
        /// <summary> 最後一次登入錯誤累積次數
        ///
        /// </summary>
        [Column]
        public Int32 LastErrorSum { get; set; }
        /// <summary> 最後一次登入之SessionID
        ///
        /// </summary>
        [Column]
        public String LastSessionID { get; set; }
        /// <summary> 信箱位址
        ///
        /// </summary>
        [Column]
        public String EmailAddress { get; set; }

        /// <summary> 登入驗證模式,DB:資料庫帳密驗證,AD:AD Server驗證
        ///
        /// </summary>
        [Column]
        public String VerifyMode { get; set; }

        /// <summary> 身分別，對應到 defCode 表
        ///
        /// </summary>
        [Column]
        public String Identify { get; set; }

        /// <summary> 特權帳號
        ///
        /// </summary>
        [Column]
        public string Special { get; set; }

        /// <summary>
        /// 對應角色
        /// </summary>
        public sysRole Role
        {
            get
            {
                return Service_sysRole.GetDetail(RoleID);
            }
        }

        /// <summary>
        /// 角色名稱
        /// </summary>
        /// <value></value>
        public string RoleName
        {
            get
            {
                if (Role == null)
                {
                    return String.Empty;
                }
                return Role.RoleName;
            }
        }

        /// <summary>
        /// 取回民國年的出生年
        /// </summary>
        public string YearOfBirthStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(YearOfBirth, "yyy"); }
            set
            {
                if (value == null)
                {
                    YearOfBirth = null;
                }
                else
                {
                    YearOfBirth = Utility_DateTime.ParseExact(value, "yyyy-MM-dd") ?? DateTime.Now;
                }
            }
        }
    }
}
