using ICCModule.Entity.Tables;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysLogin_log
    {
        /// <summary>
        /// 讀取清單_舊版
        /// </summary>
        /// <returns></returns>
        public static List<sysLogin_log> GetList(DateTime beg, DateTime end, PagerInfo page)
        {
            return RepositoryUtility.GetList_Range<sysLogin_log, DateTime>(page /*換頁參數*/
                , x => x.CDate > beg && x.CDate < end /*查詢條件*/
                , x => x.CDate /*排序依據欄位*/
                , false /*反序排列*/
                );
        }

        /// <summary>
        /// 讀取清單 BY USER
        /// </summary>
        /// <returns></returns>
        public static List<sysLogin_log> GetLogList(string LoginID, DateTime? beg, DateTime? end, PagerInfo page)
        {
            return RepositoryUtility.GetList_Range<sysLogin_log, DateTime>(page /*換頁參數*/
                , x => (string.IsNullOrEmpty(LoginID) || x.LoginID == LoginID) && x.CDate >= beg && x.CDate <= end /*查詢條件*/
                , x => x.CDate /*排序依據欄位*/
                , false /*反序排列*/
                );
        }

        /// <summary>
        /// 讀取VIEW清單 BY USER
        /// </summary>
        /// <returns></returns>
        public static List<VW_sysLogin_log> GetVWList(string LoginID, DateTime? beg, DateTime? end, PagerInfo page)
        {
            beg = beg ?? DateTime.Parse("1900-01-01");
            end = end ?? DateTime.MaxValue;
            List<VW_sysLogin_log> VWLog = new List<VW_sysLogin_log>();
            List<sysLogin_log> Log = GetLogList(LoginID, beg, end, page);
            List<sysUserInfo> Users = Service_sysUserInfo.GetList();
            //組合
            var _temp = (from L in Log
                         //串使用者
                         join U in Users on L.LoginID.ToLower() equals U.LoginID.ToLower() into leftU
                         from U in leftU.DefaultIfEmpty()
                         select new { L, UserName = U == null ? "" : U.UserName }
                         ).ToList();
            VWLog = _temp.ConvertAll(x => {
                return new VW_sysLogin_log() { ID = x.L.ID, LoginID = x.L.LoginID, UserName = x.UserName
                    , LoginIP = x.L.LoginIP, Record = x.L.Record, CDate = x.L.CDate
                };
            }).OrderByDescending(x => x.ID).ToList();
            return VWLog;
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysLogin_log data)
        {
            return RepositoryUtility.Insert<sysLogin_log>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(long ID)
        {
            return RepositoryUtility.Delete<sysLogin_log>(x => x.ID == ID);
        }

        public class VW_sysLogin_log
        {
            public long ID { get; set; }
            public string LoginID { get; set; }
            public string UserName { get; set; }
            public string LoginIP { get; set; }
            public string Record { get; set; }
            public DateTime CDate { get; set; }
        }
    }
}
