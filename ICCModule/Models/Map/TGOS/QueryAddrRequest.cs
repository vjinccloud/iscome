using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.Models.Map.TGOS
{
    /// <summary>
    /// 呼叫門牌位置比對服務
    /// </summary>
    public class QueryAddrRequest
    {
        public QueryAddrRequest()
        {
            // GPS 用
            oSRS = "EPSG:4326";
            oFuzzyType = 0;
            oResultDataType = "JSON";
            oFuzzyBuffer = 0;
            oIsOnlyFullMatch = true;
            oIsLockCounty = true;
            oIsLockTown = true;
            oIsLockVillage = false;
            oIsLockRoadSection = false;
            oIsLockLane = false;
            oIsLockAlley = false;
            oIsLockArea = false;
            oIsSameNumber_SubNumber = true;
            oCanIgnoreVillage = true;
            oCanIgnoreNeighborhood = true;
            oReturnMaxCount = 0;
        }

        /// <summary>
        /// 應用程式識別碼(APPId)，這個值是由使用API的使用者在申請成功後所取得，必須與應用程式介接驗證碼(APIKey)成對使用，這個值是由使用API的使用者在呼叫API時所需傳入
        /// </summary>
        public string oAPPId { get; set; }

        /// <summary>
        /// 應用程式介接驗證碼(APIKey)，這個值是由使用API的使用者在申請成功後所取得，必須與應用程式識別碼(APPId)成對使用，這個值是由使用API的使用者在呼叫API時所需傳入
        /// </summary>
        public string oAPIKey { get; set; }

        /// <summary>
        /// 所要查詢的門牌地址
        /// </summary>
        public string oAddress { get; set; }

        /// <summary>
        /// 回傳的坐標系統，允許傳入的坐標系統代碼為：EPSG:4326、EPSG:3825、EPSG:3826、EPSG:3827、EPSG:3828
        /// </summary>
        public string oSRS { get; set; }

        /// <summary>
        /// 目前有下列三種模糊比對的機制:
        // (一) 最近門牌號機制：先找最近且大於輸入的門牌號，找不到則找最近且小於輸入的門牌號；代碼為 0
        // (二) 單雙號機制：先找最近且大於輸入的單號或雙號門牌號，找不到則找最近且小於輸入的單號或雙號門牌號；代碼為 1
        // (三) 單雙號機制 + 最近門牌號機制：當單雙號機制找不到門牌時，就改採用最近門牌號機制；代碼為 2
        /// </summary>
        public int oFuzzyType { get; set; }

        /// <summary>
        /// 回傳的資料格式，允許傳入的代碼為：JSON、XML
        /// </summary>
        public string oResultDataType { get; set; }

        /// <summary>
        /// 模糊比對回傳門牌號的許可誤差範圍，輸入格式為正整數，如輸入 0 則代表不限制誤差範圍
        /// </summary>
        public int oFuzzyBuffer { get; set; }

        /// <summary>
        /// 是否只進行完全比對，允許傳入的值為：true、false，如輸入 true ，模糊比對機制將不被使用
        /// </summary>
        public bool oIsOnlyFullMatch { get; set; }

        /// <summary>
        /// 是否鎖定縣市，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [縣市] 要與所輸入的門牌地址中的 [縣市] 完全相同
        /// </summary>
        public bool oIsLockCounty { get; set; }

        /// <summary>
        /// 是否鎖定鄉鎮市區，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [鄉鎮市區] 要與所輸入的門牌地址中的 [鄉鎮市區] 完全相同
        /// </summary>
        public bool oIsLockTown { get; set; }

        /// <summary>
        /// 是否鎖定村里，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [村里] 要與所輸入的門牌地址中的 [村里] 完全相同
        /// </summary>
        public bool oIsLockVillage { get; set; }

        /// <summary>
        /// 是否鎖定路段，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [路段] 要與所輸入的門牌地址中的 [路段] 完全相同
        /// </summary>
        public bool oIsLockRoadSection { get; set; }

        /// <summary>
        /// 	是否鎖定巷，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [巷] 要與所輸入的門牌地址中的 [巷] 完全相同
        /// </summary>
        public bool oIsLockLane { get; set; }

        /// <summary>
        /// 是否鎖定弄，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [弄] 要與所輸入的門牌地址中的 [弄] 完全相同
        /// </summary>
        public bool oIsLockAlley { get; set; }

        /// <summary>
        /// 是否鎖定地區，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [地區] 要與所輸入的門牌地址中的 [地區] 完全相同
        /// </summary>
        public bool oIsLockArea { get; set; }

        /// <summary>
        /// 號之、之號是否視為相同，允許傳入的值為：true、false
        /// </summary>
        public bool oIsSameNumber_SubNumber { get; set; }

        /// <summary>
        /// 找不時是否可忽略村里，允許傳入的值為：true、false
        /// </summary>
        public bool oCanIgnoreVillage { get; set; }

        /// <summary>
        /// 找不時是否可忽略鄰，允許傳入的值為：true、false
        /// </summary>
        public bool oCanIgnoreNeighborhood { get; set; }

        /// <summary>
        /// 如為多筆時，可限制回傳最大筆數（1~9），輸入格式為正整數，如輸入 0 則以本服務上限10筆進行回傳。
        /// </summary>
        public int oReturnMaxCount { get; set; }
    }
}
