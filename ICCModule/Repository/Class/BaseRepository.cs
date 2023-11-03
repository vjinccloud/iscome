using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICCModule.Repository.Interface;
using System.Reflection;
using System.Configuration;
using IscomG2C.Utility;
using ICCModule.Helper;
using System.Data;

namespace ICCModule.Repository.Class
{
    public class BaseRepository : PagerInfoRepository, IBaseRepository, IDisposable
    {
        #region ====== 自訂參數與函式 ======

        /// <summary>資料表物件
        /// 
        /// </summary>
        protected ICCModuleContext context = new ICCModuleContext();

        /// <summary>連線字串
        /// 
        /// </summary>
        protected string sConn
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;
            }
        }

        public void SetRepository(string strConn)
        {
            context = new ICCModuleContext(strConn);
            context.Connection.ConnectionString = strConn;
        }

        public BaseRepository(string strConn)
        {
            context = new ICCModuleContext(strConn);
            context.Connection.ConnectionString = strConn;
        }

        public BaseRepository()
        {
            context.Connection.ConnectionString = sConn;
        }

        ///// <summary>關閉異動資料表物件
        ///// 
        ///// </summary>
        //public void CloseContextOperation()
        //{
        //    if (this.context != null)
        //        this.context.Dispose();
        //    return;
        //}

        /// <summary>
        /// copy value from _objFrom to _objTo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_objFrom"></param>
        /// <param name="_objTo"></param>
        /// <param name="_tType"></param>
        /// <returns>_objTo</returns>
        private T CopyValueO2O<T>(T _objFrom, T _objTo, Type _objType)
        {
            FieldInfo[] fields = _objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(_objTo, field.GetValue(_objFrom));
            }
            return _objTo;
        }

        #endregion

        #region IBaseRepository 成員

        /// <summary>
        /// 儲存
        /// </summary>
        public void Save()
        {
            context.SubmitChanges();

            //紀錄系統中的 SQL Log
            Save_SQL_Log();
        }

        /// <summary>紀錄系統中的 SQL Log
        /// 
        /// </summary>
        public void Save_SQL_Log()
        {
            //AllenLi:先取消掉此機制,如果每次操作資料庫存取都要額外保留紀錄最徒增系統負擔

            ////如果有啟用
            if (context.Log == null)
                return;

            string SQL = context.Log.ToString();
            if (SQL == "")
                return;

            ////取得當前的系統紀錄ID
            //long ID = SessionHelper.Get_LogID();
            //if (ID == 0)
            //    return;
            ////備存一份到資料庫裏面
            //List<RepositoryUtility.InputDBParam> Param = new List<RepositoryUtility.InputDBParam>();
            //Param.Add(new RepositoryUtility.InputDBParam() { Name = "@SQLRecord", type = SqlDbType.NVarChar, Value = context.Log.ToString() });
            //Param.Add(new RepositoryUtility.InputDBParam() { Name = "@ID", type = SqlDbType.BigInt, Value = ID });
            //string updSQL = @"
            //    update System_Log
            //    set SQLRecord=SQLRecord+@SQLRecord
            //    where ID=@ID
            //";
            //string msg = "";
            //RepositoryUtility.EXEC_SQL(ref msg, updSQL, Param, this.sConn);
        }

        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="caseFilter">過濾條件</param>
        /// <returns>回傳查詢結果</returns>
        public IQueryable<T> QueryData<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter = null) where T : class
        {
            string sErrMsg = "";
            return QueryData<T>(ref sErrMsg, filter);
        }

        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="caseFilter">過濾條件</param>
        /// <returns>回傳查詢結果</returns>
        public IQueryable<T> QueryData<T>(ref string sErrMsg, System.Linq.Expressions.Expression<Func<T, bool>> filter = null) where T : class
        {
            sErrMsg = String.Empty;
            IQueryable<T> query = context.GetTable<T>().Cast<T>();

            if (filter != null)
            {
                try
                {
                    query = query.Where(filter);
                    //紀錄系統中的 SQL Log
                    Save_SQL_Log();
                }
                catch (Exception ex)
                {
                    sErrMsg = ex.Message;
                    ErrorLog.SaveErrorLog(ex);
                }
            }

            return query;

        }

        /// <summary>
        /// 查詢單筆資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="predicate">判斷條件</param>
        /// <returns>回傳查詢結果</returns>
        public T SingleOrDefault<T>(ref string sErrMsg, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            sErrMsg = String.Empty;
            T DBentity = default(T);

            try
            {
                DBentity = context.GetTable<T>().SingleOrDefault(predicate);

                //紀錄系統中的 SQL Log
                Save_SQL_Log();
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                ErrorLog.SaveErrorLog(ex);
            }

            return DBentity;
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="entity">新增物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳新增狀態</returns>
        public bool Insert<T>(T entity, ref string sErrMsg) where T : class
        {
            sErrMsg = String.Empty;
            bool bResult = false;

            try
            {
                context.GetTable<T>().InsertOnSubmit(entity);
                Save();

                bResult = true;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                ErrorLog.SaveErrorLog(ex);
            }

            return bResult;
        }

        /// <summary>新增資料
        /// 
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="entity">新增物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳新增狀態</returns>
        public bool InsertList<T>(IEnumerable<T> entityList, ref string sErrMsg) where T : class
        {
            sErrMsg = String.Empty;
            bool bResult = false;

            try
            {
                context.GetTable<T>().InsertAllOnSubmit(entityList);
                Save();

                bResult = true;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
            }

            return bResult;
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳更新狀態</returns>
        public bool Update(ref string sErrMsg)
        {
            bool bResult = false;

            try
            {
                context.SubmitChanges();

                //紀錄系統中的 SQL Log
                Save_SQL_Log();

                bResult = true;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                ErrorLog.SaveErrorLog(ex);
            }
            return bResult;
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="entity">刪除物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳刪除狀態</returns>
        public bool Delete<T>(T entity, ref string sErrMsg) where T : class
        {
            sErrMsg = String.Empty;
            bool bResult = false;

            try
            {
                context.GetTable<T>().DeleteOnSubmit(entity);
                Save();

                bResult = true;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                ErrorLog.SaveErrorLog(ex);
            }

            return bResult;
        }

        #endregion

        #region IDisposable 成員

        public void Dispose()
        {
            if (this.context != null)
                this.context.Dispose();
            return;
        }
        protected virtual void Dispose(bool disposing)
        {
            this.context.Dispose();
            if (disposing)
                GC.SuppressFinalize(this);
        }
        #endregion
    }
}
