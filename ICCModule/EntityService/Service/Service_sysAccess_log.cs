using ICCModule.Entity.Tables;
using ICCModule.Repository;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysAccess_log
    {
        /// <summary>
        /// 讀取清單_舊版
        /// </summary>
        /// <returns></returns>
        public static List<sysAccess_log> GetList(DateTime beg, DateTime end, PagerInfo page)
        {
            return RepositoryUtility.GetList_Range<sysAccess_log, DateTime>(page /*換頁參數*/
                , x => x.CDate > beg && x.CDate < end /*查詢條件*/
                , x => x.CDate /*排序依據欄位*/
                , false /*反序排列*/
                );
        }

        /// <summary>
        /// 讀取清單 BY USER
        /// </summary>
        /// <returns></returns>
        public static List<sysAccess_log> GetLogList(string LoginID, DateTime? beg, DateTime? end, PagerInfo page)
        {
            return RepositoryUtility.GetList_Range<sysAccess_log, DateTime>(page /*換頁參數*/
                , x => (string.IsNullOrEmpty(LoginID) || x.LoginID == LoginID) && x.CDate.Date >= beg && x.CDate.Date <= end /*查詢條件*/
                , x => x.CDate /*排序依據欄位*/
                , false /*反序排列*/
                );
        }

        /// <summary>
        /// 讀取VIEW清單 BY USER
        /// </summary>
        /// <returns></returns>
        public static List<VW_sysAccess_Log> GetVWList(string LoginID, DateTime? beg, DateTime? end, PagerInfo page)
        {
            beg = beg ?? DateTime.Parse("1900-01-01");
            end = end ?? DateTime.MaxValue;
            List<VW_sysAccess_Log> VWLog = new List<VW_sysAccess_Log>();
            List<sysAccess_log> Log = GetLogList(LoginID, beg, end, page);
            List<sysMenu> Menus = Service_sysMenu.GetList(null);
            List<sysUserInfo> Users = Service_sysUserInfo.GetList();
            //組合
            var _temp = (from L in Log
                         //串表單路徑
                         join M in Menus on L.Path.ToLower() equals M.Path.ToLower() into leftM
                         from M in leftM.DefaultIfEmpty()
                         //串使用者
                         join U in Users on L.LoginID.ToLower() equals U.LoginID.ToLower() into leftU
                         from U in leftU.DefaultIfEmpty()
                         select new { L, M, UserName = U == null ? "" : U.UserName }
                         ).ToList();
            VWLog = _temp.ConvertAll(x => {
                return new VW_sysAccess_Log() { ID = x.L.ID, LoginID = x.L.LoginID, UserName = x.UserName
                    , Path = x.L.Path, MenuName = x.M?.Name ?? x.L.Path
                    , Act = x.L.Act, ActName = (x.L.Act.Contains("read")) ? "讀取" : (x.L.Act.Contains("create") ? "新增" : (x.L.Act.Contains("update") ? "編輯" : (x.L.Act.Contains("delete") ? "刪除" : (x.L.Act.Contains("export") ? "匯出" : (x.L.Act.Contains("import") ? "匯入" : (x.L.Act == "deny" ? "無權限操作" : "其他操作"))))))
                    , LoginIP = x.L.LoginIP, Remark = x.L.Remark, CDate = x.L.CDate
                };
            }).OrderByDescending(x => x.ID).ToList();
            return VWLog;
        }
        
        /// <summary>
        /// 讀取VIEW清單 BY USER
        /// </summary>
        /// <returns></returns>
        public static List<sysAccess_log> GetVWList(DateTime? beg, DateTime? end)
        {
            Expression<Func<sysAccess_log, bool>> filter = x => true;
            if (beg.HasValue) filter = filter.And(x => x.CDate.Date >= beg);
            if (end.HasValue) filter = filter.And(x => x.CDate.Date <= end);
            List<sysAccess_log> Log = RepositoryUtility.GetList<sysAccess_log>(filter).OrderByDescending(x => x.CDate).ToList();
            
            return Log;
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysAccess_log data)
        {
            return RepositoryUtility.Insert<sysAccess_log>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(long ID)
        {
            return RepositoryUtility.Delete<sysAccess_log>(x => x.ID == ID);
        }

        public class VW_sysAccess_Log
        {
            public long ID { get; set; }
            public string LoginID { get; set; }
            public string UserName { get; set; }
            public string Path { get; set; }
            public string MenuName { get; set; }
            public string Act { get; set; }
            public string ActName { get; set; }
            public string LoginIP { get; set; }
            public string Remark { get; set; }
            public DateTime CDate { get; set; }
        }
    }
}
