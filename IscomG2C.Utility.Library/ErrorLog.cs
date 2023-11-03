using System;
using System.Collections.Generic;
#region 引用套件
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Text;
#endregion

namespace IscomG2C.Utility
{
    /// <summary>用於記錄錯誤訊息，將錯誤訊息寫入/ErrorLog/資料夾中
    /// 
    /// </summary>
    public class ErrorLog
    {
        /// <summary>將呼叫堆疊資訊與錯誤訊息寫入/ErrorLog/資料夾中 預設分類為:ErrorLog
        /// 
        /// </summary>
        /// <param name="log">錯誤訊息</param>
        public static void SaveErrorLog(string log, string CategoryDirName = "Log")
        {

            StackTrace track = new StackTrace();//取得當前的堆疊資訊

            log = log + "\r\n堆疊資訊\r\n" + track.ToString();//堆疊資訊加上錯誤訊息

            SaveTextFile(log, CategoryDirName);
        }

        /// <summary>將Exception訊息寫入/ErrorLog/資料夾中 預設分類為:Exception
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void SaveErrorLog(Exception ex, string CategoryDirName = "Exception", string Message = "")
        {
            //取得錯誤的堆疊資訊 在上
            string StackTrace = ex.StackTrace;
            //取得當前的堆疊資訊 在下
            StackTrace track = new StackTrace();

            //寫入檔案: 備註 錯誤訊息 堆疊資訊
            SaveTextFile(Message + "\r\n堆疊資訊\r\n" + ex.ToString()
                + StackTrace + track.ToString()
                , CategoryDirName);
        }

        /// <summary>儲存文字檔
        /// 
        /// </summary>
        /// <param name="log">錯誤文字檔</param>
        /// <param name="CategoryDirName">錯誤類型</param>
        static void SaveTextFile(string log, string CategoryDirName = "Other")
        {
            try
            {
                string root = "";
                //判斷呼叫的專案檔是否為Web專案?
                if (System.Web.HttpContext.Current != null)
                {
                    //網頁版本的路徑取法
                    root = HttpRuntime.AppDomainAppPath;
                }
                else
                {
                    //視窗程式或主控台應用程式叫法
                    root = System.IO.Directory.GetCurrentDirectory();
                }
                string DirPath = root + "\\ErrorLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + CategoryDirName + "\\";
                //檢查資料夾路徑
                //HttpRuntime.AppDomainAppPath
                //System.Web.HttpContext.Current.Server
                bool dir_exist = System.IO.Directory.Exists(DirPath);
                //若沒有則新增資料夾
                if (dir_exist == false)
                    System.IO.Directory.CreateDirectory(DirPath);
                //路徑
                string path = DirPath + "Log_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ".txt";
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.WriteLine(log);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
