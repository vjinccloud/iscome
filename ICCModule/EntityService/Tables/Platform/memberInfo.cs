using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "member_Infos")]
    public class memberInfo
    {
        /// <summary> 流水號
        ///
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 會員帳號
        ///
        /// </summary>
        [Column]
        public string LoginID { get; set; }

        /// <summary> 會員名稱
        ///
        /// </summary>
        [Column]
        public string Name { get; set; }

        /// <summary> 密碼(加密字串)
        ///
        /// </summary>
        [Column]
        public string LoginPass { get; set; }

        /// <summary>
        /// 檢核碼
        /// </summary>
        [Column]
        public string CheckCode { get; set; }

        /// <summary>
        /// 密碼過期日
        /// </summary>
        [Column]
        public DateTime? PasswordExpiredAt { get; set; }

        /// <summary>
        /// 舊密碼，Json Array 字串
        /// </summary>
        [Column]
        public string OldPasswords { get; set; }

        /// <summary>
        /// 舊密碼 Json 陣列
        /// </summary>
        public JArray OldPasswordArr
        {
            get
            {
                if (String.IsNullOrEmpty(OldPasswords))
                {
                    return new JArray();
                }

                return JArray.Parse(OldPasswords);
            }

            set
            {
                OldPasswords = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary> 信箱
        ///
        /// </summary>
        [Column]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 信箱驗證用ID，唯一值
        /// </summary>
        [Column(IsDbGenerated = true)]
        public Guid EmailID { get; set; }

        /// <summary> 信箱驗證時間
        ///
        /// </summary>
        [Column]
        public DateTime? EmailVerifiedAt { get; set; }

        /// <summary> 簡訊驗證時間
        ///
        /// </summary>
        [Column]
        public DateTime? SMSVerifiedAt { get; set; }

        /// <summary>
        /// 驗證方式
        /// </summary>
        [Column]
        public string VerifyMethod { get; set; }

        /// <summary> 記住我有效令牌
        ///
        /// </summary>
        [Column]
        public string RememberToken { get; set; }

        /// <summary> 行動電話
        ///
        /// </summary>
        [Column]
        public string Mobile { get; set; }

        /// <summary> 出生年
        ///
        /// </summary>
        [Column]
        [DataType(DataType.Date)]
        public DateTime? YearOfBirth { get; set; }

        public string Year
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(YearOfBirth, "yyy"); }
            set { YearOfBirth = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }
        
        public int NewYear { get; set; }

        /// <summary> 性別
        ///
        /// </summary>
        [Column]
        public string Sexy { get; set; }

        /// <summary> 縣市
        ///
        /// </summary>
        [Column]
        public string City { get; set; }

        /// <summary> 行政區
        ///
        /// </summary>
        [Column]
        public string District { get; set; }

        /// <summary> 訂閱關鍵字
        ///
        /// </summary>
        [Column]
        public string SubscribeEpidemic { get; set; }

        /// <summary>
        /// 身分別
        /// </summary>
        [Column]
        public String Identify { get; set; }

        /// <summary>
        /// 身分角色別，對應到 defCode 表
        /// </summary>
        [Column]
        public String RoleCode { get; set; }

        /// <summary> 狀態
        ///
        /// </summary>
        [Column]
        public string Status { get; set; }

        /// <summary> 黑名單過期時間
        ///
        /// </summary>
        [Column]
        public DateTime? BlacklistExpiredAt { get; set; }

        /// <summary>
        /// 植醫逾期次數
        /// </summary>
        [Column]
        public byte ExpiredTimes { get; set; }

        /// <summary> 創建時間
        ///
        /// </summary>
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        ///
        /// </summary>
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        [Column]
        public string LineMessageId { get; set; }
        
        /// <summary> 
        /// Line綁定代碼
        /// </summary>
        [Column]
        public string LineBindCode { get; set; }
        
        /// <summary> 
        /// 
        /// </summary>
        [Column]
        public string GoogleId { get; set; }
        
        /// <summary> 
        /// 
        /// </summary>
        [Column]
        public string FacebookId { get; set; }

        /// <summary> 
        /// Line登入序號
        /// </summary>
        [Column]
        public string LineLoginId { get; set; }
        
        /// <summary> 
        /// Line綁定時限
        /// </summary>
        [Column]
        public DateTime? LineBindLimit { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        [Column]
        public string LineUserId { get; set; }

        /// <summary> 
        /// 
        /// </summary>
        [Column]
        public string LineNonce { get; set; }

        /// <summary> 
        /// 註冊來源
        /// </summary>
        [Column]
        public string RegistFrom { get; set; }

        /// <summary>
        /// 檢查輸入密碼是否相符
        /// </summary>
        /// <param name="needCheckPws">待驗證密碼</param>
        /// <returns></returns>
        public bool CheckPassword(string needCheckPws)
        {
            return Utility_Cryptography.Compare_HashCode(needCheckPws, LoginPass);
        }

        /// <summary>
        /// 雜湊函數 SHA-512，最大輸入長度 2^128 -1
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <returns>輸出一組64位不可逆編碼</returns>
        public static string getSHA512(string sDataIn)
        {
            string sData = sDataIn;                             //取得來源字串
            SHA512 sha512 = new SHA512CryptoServiceProvider();  //建立一個SHA512
            byte[] source = Encoding.Default.GetBytes(sData);   //將字串轉為Byte[]
            byte[] crypto = sha512.ComputeHash(source);         //進行SHA512加密
            return Convert.ToBase64String(crypto);              //把加密後的字串從Byte[]轉為字串
        }

        private static Random random = new Random();

        /// <summary>
        /// 網路亂數
        /// </summary>
        /// <param name="length">亂數長度</param>
        /// <returns></returns>
        public static string getNewRandom(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
