using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICCModule.Repository.Interface;
using IscomG2C.Utility;

namespace ICCModule.Repository.Class
{
    /// <summary>
    /// 分頁資訊
    /// </summary>
    [Serializable]
    public class PagerInfoRepository : IPagerInfo
    {
        /// <summary>
        /// 分頁函式
        /// </summary>
        /// <typeparam name="T">指定類別</typeparam>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="_modules">結果資料</param>
        /// <param name="pager">分頁資訊</param>
        /// <returns>回傳分頁結果</returns>
        public List<T> GetRange<T>(ref string sErrMsg, IQueryable<T> _modules, PagerInfo pager) where T : class
        {
            if (_modules == null)
            {
                return new List<T>();
            }

            try
            {
                //設定分頁狀態
                pager.m_iDataCount = _modules.Count();
                pager.m_iPageTotal = pager.m_iDataCount / pager.m_iPageCount;

                if ((pager.m_iDataCount % pager.m_iPageCount) > 0)
                {
                    pager.m_iPageTotal += 1;
                }

                if (pager.m_iPageIndex == 0)
                {
                    pager.m_iPageIndex = 1;
                }

                pager.m_iPrePage = pager.m_iPageIndex - 1;
                pager.m_iNextPage = pager.m_iPageIndex + 1;

                if (pager.m_iPageIndex >= pager.m_iPageTotal)
                {
                    pager.m_iPageIndex = pager.m_iPageTotal;
                    pager.m_iNextPage = pager.m_iPageIndex;
                }

                if (pager.m_iPrePage < 1)
                {
                    pager.m_iPrePage = 1;
                }

                Int32 firstIndex = (pager.m_iPageIndex - 1) * pager.m_iPageCount;

                if (pager.m_iPageTotal == 0)
                {
                    pager.m_iPageTotal = 1;
                    firstIndex = 0;
                }

                Int32 lastIndex = firstIndex + pager.m_iPageCount;
                if (lastIndex >= pager.m_iDataCount)
                {
                    lastIndex = pager.m_iDataCount;
                }

                return _modules.Skip(firstIndex).Take(pager.m_iPageCount).ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                sErrMsg = ex.ToString();
                return new List<T>();
            }
        }
    }
}
