using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace ICCModule.Repository.Interface
{
    public interface IBaseRepository : IPagerInfo
    {
        void Save();
        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="caseFilter">過濾條件</param>
        /// <returns>回傳查詢資料</returns>
        IQueryable<T> QueryData<T>(ref string sErrMsg, Expression<Func<T, bool>> filter = null) where T : class;
        /// <summary>
        /// 查詢單筆資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="predicate">查詢條件</param>
        /// <returns>回傳查詢資料</returns>
        T SingleOrDefault<T>(ref string sErrMsg, Expression<Func<T, bool>> predicate) where T : class;
        /// <summary>
        /// 新增資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="entity">儲存物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳新增資料結果</returns>
        bool Insert<T>(T entity, ref string sErrMsg) where T : class;
        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳更新資料結果</returns>
        bool Update(ref string sErrMsg);
        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <typeparam name="T">類別</typeparam>
        /// <param name="entity">刪除物件</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <returns>回傳刪除資料結果</returns>
        bool Delete<T>(T entity, ref string sErrMsg) where T : class;
    }
}
