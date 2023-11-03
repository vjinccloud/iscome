using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCModule
{
    /// <summary>
    /// 分頁類別
    /// </summary>
    [Serializable]
    public class PagerInfo
    {
        public PagerInfo()
        {
            m_iPageIndex = 1;
            m_iPageCount = 30;//預設30筆
            m_iDataCount = 1;

            this.SetPageCountList();
        }

        public PagerInfo(int pageCount)
        {
            m_iPageIndex = 1;
            m_iPageCount = pageCount;
            m_iDataCount = 1;

            this.SetPageCountList();
        }

        /// <summary>設定分頁資料數量集合
        /// 
        /// </summary>
        /// <param name="listPageCount"></param>
        public PagerInfo(List<int> listPageCount)
        {
            m_iPageIndex = 1;
            //m_iDataCount = 1;

            m_iPageCountList = listPageCount;
            m_iPageCountList.Sort();
            m_iPageCount = listPageCount[0];

        }

        /// <summary>
        /// 設定分頁資料數量集合
        /// </summary>
        private void SetPageCountList()
        {
            m_iPageCountList = new List<int>();

            m_iPageCountList.Add(15);
            m_iPageCountList.Add(30);
            m_iPageCountList.Add(50);
            m_iPageCountList.Add(100);
            m_iPageCountList.Add(200);

            m_iPageCountList.Sort();
        }

        /// <summary>
        /// 目前頁碼 
        /// </summary>
        public int m_iPageIndex { get; set; }
        /// <summary>
        /// 每頁資料數量
        /// </summary>
        public int m_iPageCount { get; set; }
        /// <summary>
        /// 資料總數量
        /// </summary>
        public int m_iDataCount { get; set; }
        /// <summary>
        /// 總頁數
        /// </summary>
        public int m_iPageTotal { get; set; }
        /// <summary>
        /// 上一頁
        /// </summary>
        public int m_iPrePage { get; set; }
        /// <summary>
        /// 下一頁
        /// </summary>
        public int m_iNextPage { get; set; }
        /// <summary>
        /// 分頁資料數量集合
        /// </summary>
        public List<int> m_iPageCountList { get; set; }
    }
}
