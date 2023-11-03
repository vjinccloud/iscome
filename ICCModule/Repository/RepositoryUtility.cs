using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ICCModule;
using System.Linq.Expressions;
using System.Data.SqlClient;
//using IscomG2C.Utility;
using ICCModule.Repository.Class;
using System.Configuration;
using IscomG2C.Utility;

namespace ICCModule.Repository
{
    /// <summary>簡化版本的BaseRepository工具 不需要建立BaseRepository實體即可直接呼叫使用
    /// 
    /// </summary>
    public static class RepositoryUtility
    {
        /// <summary>取得清單
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class
        {
            BaseRepository BR = new BaseRepository();
            List<T> list
                = BR.QueryData<T>(ref sErrMsg, filter)
                .ToList();

            //關閉資料表物件
            BR.Dispose();

            return list;
        }

        /// <summary>取得清單
        /// 
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="filter">過濾條件</param>
        /// <returns></returns>
        public static List<T> GetList<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string Msg = "";
            return GetList<T>(ref Msg, filter);
        }

        /// <summary>取得清單
        /// 
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <typeparam name="T">類別</typeparam>
        /// <typeparam name="TKey">排序依據類別 通常建議給DateTime</typeparam>
        /// <param name="page">換頁參數</param>
        /// <param name="filter">過濾條件</param>
        /// <param name="Order">排序依據欄位</param>
        /// <param name="isASC">是否正向排列</param>
        /// <returns></returns>
        public static List<T> GetList_Range<T, TKey>(ref string sErrMsg
            , PagerInfo page
            , Expression<Func<T, bool>> filter = null
            , Expression<Func<T, TKey>> Order = null
            , bool isASC = true) where T : class
        {
            BaseRepository BR = new BaseRepository();
            var QueryData
                = BR.QueryData<T>(ref sErrMsg, filter)
                ;
            if (Order != null)
            {
                if (isASC)
                {
                    QueryData = QueryData.OrderBy(Order);
                }
                else
                {
                    QueryData = QueryData.OrderByDescending(Order);
                }
            }
            //最後要回傳的結果紀錄
            List<T> dataList = new List<T>();
            //如果有指定換頁
            if (page != null)
            {   //根據換頁參數作部分擷取
                PagerInfoRepository PR = new PagerInfoRepository();
                dataList = PR.GetRange<T>(ref sErrMsg, QueryData, page);
            }
            else
            {   //沒有指定換頁 則直接轉換成List即可
                dataList = QueryData.ToList();
            }
            //關閉資料表物件
            BR.Dispose();

            return dataList;
        }

        /// <summary>取得清單
        /// 
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <typeparam name="TKey">排序依據類別 通常建議給DateTime</typeparam>
        /// <param name="page">換頁參數</param>
        /// <param name="filter">過濾條件</param>
        /// <param name="Order">排序依據欄位</param>
        /// <param name="isASC">是否正向排列</param>
        /// <returns></returns>
        public static List<T> GetList_Range<T, TKey>(PagerInfo page
            , Expression<Func<T, bool>> filter = null
            , Expression<Func<T, TKey>> Order = null
            , bool isASC = true) where T : class
        {
            string sErrMsg = "";
            return GetList_Range<T, TKey>(ref sErrMsg, page, filter, Order, isASC);
        }


        /// <summary>取得清單
        /// 
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <typeparam name="T">類別</typeparam>
        /// <typeparam name="TKey">排序依據類別 通常建議給DateTime</typeparam>
        /// <param name="page">換頁參數</param>
        /// <param name="filter">過濾條件</param>
        /// <param name="Order">排序依據欄位</param>
        /// <param name="ThenOrder">排序依據欄位2</param>
        /// <param name="isASC">是否正向排列</param>
        /// <param name="ThenisASC">欄位2是否正向排列</param>
        /// <returns></returns>
        public static List<T> GetList_Range<T, TKey>(ref string sErrMsg
            , PagerInfo page
            , Expression<Func<T, bool>> filter = null
            , Expression<Func<T, TKey>> Order = null
            , Expression<Func<T, TKey>> ThenOrder = null
            , bool isASC = true
            , bool ThenisASC = true) where T : class
        {
            BaseRepository BR = new BaseRepository();
            var QueryData
                = BR.QueryData<T>(ref sErrMsg, filter)
                ;

            if (Order != null && ThenOrder != null)
            {
                if (isASC && ThenisASC)
                {
                    QueryData = QueryData.OrderBy(Order).ThenBy(ThenOrder);
                }
                else if (isASC && !ThenisASC)
                {
                    QueryData = QueryData.OrderBy(Order).ThenByDescending(ThenOrder);
                }
                else if (!isASC && ThenisASC)
                {
                    QueryData = QueryData.OrderByDescending(Order).ThenBy(ThenOrder);
                }
                else if (!isASC && !ThenisASC)
                {
                    QueryData = QueryData.OrderByDescending(Order).ThenByDescending(ThenOrder);
                }

            }
            else if (Order != null)
            {
                if (isASC)
                {
                    QueryData = QueryData.OrderBy(Order);
                }
                else
                {
                    QueryData = QueryData.OrderByDescending(Order);
                }
            }

            //最後要回傳的結果紀錄
            List<T> dataList = new List<T>();
            //如果有指定換頁
            if (page != null)
            {   //根據換頁參數作部分擷取
                PagerInfoRepository PR = new PagerInfoRepository();
                dataList = PR.GetRange<T>(ref sErrMsg, QueryData, page);
            }
            else
            {   //沒有指定換頁 則直接轉換成List即可
                dataList = QueryData.ToList();
            }
            //關閉資料表物件
            BR.Dispose();

            return dataList;
        }


        /// <summary>取得第一筆符合的資料
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static T GetDetail<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class
        {
            var list = GetList(ref sErrMsg, filter);
            return list.FirstOrDefault();
        }

        /// <summary>取得第一筆符合的資料
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static T GetDetail<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string Msg = "";
            return GetDetail<T>(ref Msg, filter);
        }

        /// <summary>取得數量
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static int GetCount<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string sErrMsg = "";

            return GetCount<T>(ref sErrMsg, filter);
        }

        /// <summary>取得數量
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static int GetCount<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class
        {
            BaseRepository BR = new BaseRepository();
            int Count
                = BR.QueryData<T>(ref sErrMsg, filter)
                .Count();

            //關閉資料表物件
            BR.Dispose();

            return Count;
        }

        /// <summary>檢查是否具備符合條件的資料
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool CheckExist<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string sErrMsg = "";

            int Count = GetCount<T>(ref sErrMsg, filter);
            //沒有資料為false
            if (Count == 0)
                return false;
            //有資料
            return true;
        }


        /// <summary>新增
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool Insert<T>(T data, ref string Msg) where T : class
        {
            BaseRepository BR = new BaseRepository();
            bool Result = BR.Insert<T>(data, ref Msg);
            if (Result == false)
            {
                ErrorLog.SaveErrorLog(Msg);
            }
            BR.Dispose();
            return Result;
        }

        /// <summary>新增
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static BaseResult Insert<T>(T data) where T : class
        {
            BaseResult rs = new BaseResult();
            string Msg = "";
            rs.result = Insert<T>(data, ref Msg);
            rs.Msg = Msg ?? "";
            return rs;
        }


        /// <summary>批次新增(效能較佳)
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">資料列</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool InsertList<T>(List<T> data, ref string Msg) where T : class
        {
            //string Msg = "";
            BaseRepository BR = new BaseRepository();
            bool Result = BR.InsertList<T>(data, ref Msg);
            if (Result == false)
            {
                ErrorLog.SaveErrorLog(Msg);
            }
            BR.Dispose();
            return Result;
        }

        /// <summary>批次新增(效能較佳) 回傳BaseResult物件
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult InsertList<T>(List<T> data) where T : class
        {
            string Msg = "";
            bool rs = InsertList<T>(data, ref Msg);
            return new BaseResult(rs, Msg);
        }

        /// <summary>刪除-根據查詢條件 刪除第一筆符合的資料
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sErrMsg"></param>
        /// <returns></returns>
        public static bool Delete<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class
        {
            //特殊 如果根本沒有給條件 則不刪除 避免誤刪
            if (filter == null)
                return false;

            BaseRepository BR = new BaseRepository();
            //根據查詢條件 抓取第一筆
            var data = BR.QueryData(ref sErrMsg, filter).FirstOrDefault();
            if (data == null)
            {
                sErrMsg = "查無刪除資料";
                return false;
            }
            //刪除
            bool Result = BR.Delete<T>(data, ref sErrMsg);
            BR.Dispose();
            return Result;
        }

        /// <summary>刪除
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sErrMsg"></param>
        /// <returns></returns>
        public static BaseResult Delete<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string sErrMsg = "";

            bool rs = Delete<T>(ref sErrMsg, filter);
            return new BaseResult(rs, sErrMsg);
        }

        /// <summary>刪除全部(請謹慎使用)
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteAll<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class
        {

            //特殊 如果根本沒有給條件 則不刪除 避免誤刪
            if (filter == null)
                return false;

            BaseRepository BR = new BaseRepository();
            //根據查詢條件 抓取第一筆
            var dataList = BR.QueryData(ref sErrMsg, filter).ToList();
            if (dataList == null)
            {
                sErrMsg = "查無刪除資料";
                return false;
            }
            //刪除
            bool Result = true;
            foreach (var data in dataList)
            {
                Result = Result && BR.Delete<T>(data, ref sErrMsg);
            }
            BR.Dispose();
            return Result;
        }
        /// <summary>刪除全部
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool DeleteAll<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            string sErrMsg = "";
            return DeleteAll<T>(ref sErrMsg, filter);
        }

        /// <summary>執行SQL語法
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="SQL"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static bool EXEC_SQL(ref string Msg, string SQL, List<InputDBParam> Param = null, string connStr = "")
        {
            //如果沒有指定 則用預設的
            if (connStr == "")
                connStr = ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;
            SqlConnection db = new SqlConnection(connStr);

            SqlCommand cmd = new SqlCommand(SQL, db);
            cmd.CommandType = CommandType.Text;


            if (Param == null)
                Param = new List<InputDBParam>();

            foreach (var item in Param)
            {
                cmd.Parameters.Add(item.Name, item.type);
                cmd.Parameters[item.Name].Value = item.Value;
            }

            try
            {
                db.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                return false;
            }
            finally
            {
                db.Close();
            }
            return true;
        }


        /// <summary>執行SQL語法 並取得DataSet
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="SQL"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataSet EXEC_SQL_toDataSet(ref string Msg, string SQL, List<InputDBParam> Param = null, string connStr = "")
        {
            //如果沒有指定 則用預設的
            if (connStr == "")
                connStr = ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;

            SqlConnection db = new SqlConnection(connStr);

            SqlCommand cmd = new SqlCommand(SQL, db);
            cmd.CommandType = CommandType.Text;

            if (Param == null)
                Param = new List<InputDBParam>();

            foreach (var item in Param)
            {
                cmd.Parameters.Add(item.Name, item.type);
                cmd.Parameters[item.Name].Value = item.Value;
            }
            DataSet ds = new DataSet();

            try
            {
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(ds);

            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                ErrorLog.SaveErrorLog(ex);
            }
            finally
            {
                db.Close();
            }
            return ds;

        }

        /// <summary>執行SQL語法 並取得DataSet
        /// 
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataSet EXEC_SQL_toDataSet(string SQL, List<InputDBParam> Param, string connStr = "")
        {
            string Msg = "";
            return EXEC_SQL_toDataSet(ref Msg, SQL, Param, connStr);
        }

        /// <summary> 執行SQL語法 並取得List型態資料 必須事先提供實體Entity類別
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Msg"></param>
        /// <param name="SQL"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static List<T> EXEC_SQL_toList<T>(ref string Msg, string SQL, List<InputDBParam> Param = null, string connStr = "")
             where T : new()
        {
            DataSet ds = EXEC_SQL_toDataSet(ref Msg, SQL, Param, connStr);

            List<T> list = new List<T>();
            if (ds == null)
                return list;
            if (ds.Tables.Count == 0)
                return list;
            DataTable table = ds.Tables[0];
            list = table.ToList<T>();
            return list;
        }
        /// <summary> 執行SQL語法 並取得List型態資料 必須事先提供實體Entity類別
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Msg"></param>
        /// <param name="SQL"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>        
        public static List<T> EXEC_SQL_toList<T>(string SQL, List<InputDBParam> Param = null, string connStr = "")
             where T : new()
        {
            string Msg = "";
            return EXEC_SQL_toList<T>(ref Msg, SQL, Param, connStr);
        }

        /// <summary>執行SQL語法 取得單筆資料的單一欄位值 回傳成字串格式
        /// 通常使用於簡單的查詢
        /// 注意!如果你要查詢的欄位超過兩個以上,建議還是使用EXEC_SQL_toList()來處理資料查詢
        /// 不然多次查詢對於效能比較不利
        /// </summary>
        /// <param name="SQL">SQL語法</param>
        /// <param name="Param">SQL參數</param>
        /// <param name="connStr">連線字串</param>
        public static string EXEC_SQL_toSingleValue(string SQL, List<InputDBParam> Param = null, string connStr = "")
        {
            try
            {
                //取得DataSet
                DataSet ds = EXEC_SQL_toDataSet(SQL, Param, connStr);
                if (ds.Tables.Count == 0)
                    return "";//沒有查詢出來的資料表
                DataTable table = ds.Tables[0];
                if (table.Rows.Count == 0)
                    return "";//沒有資料
                string output = table.Rows[0][0].ToString();
                return output;
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                return "";
            }
        }

        /// <summary>執行預存程序
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="SPName"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static bool EXEC_SP(ref string Msg, string SPName, List<InputDBParam> Param, string connStr = "")
        {
            //如果沒有指定 則用預設的
            if (connStr == "")
                connStr = ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;

            SqlConnection db = new SqlConnection(connStr);

            SqlCommand cmd = new SqlCommand(SPName, db);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var item in Param)
            {
                cmd.Parameters.Add(item.Name, item.type);
                cmd.Parameters[item.Name].Value = item.Value;
            }


            try
            {
                db.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                return false;
            }
            finally
            {
                db.Close();
            }
            return true;
        }

        /// <summary>執行預存程序
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="SPName"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataSet EXEC_SP_forDataSet(ref string Msg, string SPName, List<InputDBParam> Param, string connStr = "")
        {
            //如果沒有指定 則用預設的
            if (connStr == "")
                connStr = ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;

            SqlConnection db = new SqlConnection(connStr);

            SqlCommand cmd = new SqlCommand(SPName, db);
            cmd.CommandType = CommandType.StoredProcedure;
            if (Param == null)
                Param = new List<InputDBParam>();
            foreach (var item in Param)
            {
                cmd.Parameters.Add(item.Name, item.type);
                cmd.Parameters[item.Name].Value = item.Value;
            }
            DataSet ds = new DataSet();

            try
            {
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(ds);

            }
            catch (Exception ex)
            {
                Msg = ex.ToString();
                ErrorLog.SaveErrorLog(ex);
            }
            finally
            {
                db.Close();
            }
            return ds;
        }

        /// <summary>執行預存程序
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="SPName"></param>
        /// <param name="Param"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataSet EXEC_SP_forDataSet(string SPName, List<InputDBParam> Param, string connStr = "")
        {
            string Msg = "";
            return EXEC_SP_forDataSet(ref Msg, SPName, Param, connStr);
        }

        /// <summary>輸入的變數
        /// 
        /// </summary>
        public class InputDBParam
        {
            public InputDBParam() { }
            public InputDBParam(string Name, SqlDbType type, object Value)
            {
                this.Name = Name;
                this.type = type;
                this.Value = Value;
            }
            public string Name { get; set; }
            public object Value { get; set; }
            public SqlDbType type { get; set; }
        }
    }
}
