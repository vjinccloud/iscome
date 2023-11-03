using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace IscomG2C.Utility
{
    public class FileHelper
    {
        #region 檢查檔案格式

        /// <summary>
        /// 檢查檔案格式
        /// </summary>
        /// <param name="File_Name">檔案完整名稱 ex. Test.xls</param>
        /// <param name="Type">檔案類型</param>
        /// <returns></returns>
        public static bool CheckFileFormat(string File_Name, string Type)
        {
            bool boolResult = false;
            if (!string.IsNullOrWhiteSpace(Type))
            {
                List<string> FileExtName = new List<string>();
                switch (Type)
                {
                    case "image":
                        FileExtName.Add(".jpg");
                        FileExtName.Add(".jpeg");
                        FileExtName.Add(".gif");
                        FileExtName.Add(".png");
                        break;
                }

                string fileExtension = Path.GetExtension(File_Name).ToLower(); // 文件副檔名
                //檢查檔案格式
                foreach (string item in FileExtName)
                {
                    if (fileExtension == item.ToLower())
                    {
                        boolResult = true;
                        break;
                    }
                }
            }
            return boolResult;
        }

        #endregion

        #region 檢查檔案是否存在

        /// <summary>
        /// 檢查檔案是否存在
        /// </summary>
        /// <param name="File_Path">檔案實體路徑 ex. C:\Temp\Test.xls</param>
        public static bool CheckFileExist(string File_Path)
        {
            if (!File.Exists(File_Path))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 建立資料夾

        /// <summary>
        /// 建立資料夾
        /// </summary>
        /// <param name="Folder_Path">資料夾路徑 ex. C:\Temp\Test\</param>
        public static void CreateFolder(string Folder_Path)
        {
            DirectoryInfo dir = new DirectoryInfo(Folder_Path);
            //檢查資料夾是否存在
            if (!dir.Exists)
            {
                dir.Create();
                //Directory.CreateDirectory(Folder_Path);
            }
        }

        #endregion

        #region 刪除資料夾及資料夾下檔案

        /// <summary>
        /// 刪除資料夾及資料夾下檔案
        /// </summary>
        /// <param name="DirPath">欲刪除資料夾的虛擬路徑</param>
        public static void DeleteDir(string DirPath)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(DirPath))) { return; }
            try
            {
                string[] FileList = Directory.GetFiles(HttpContext.Current.Server.MapPath(DirPath));
                foreach (string FilePath in FileList)
                {
                    File.Delete(FilePath);
                }
                Directory.Delete(HttpContext.Current.Server.MapPath(DirPath));
            }
            catch (Exception ex) { return; }
        }

        #endregion

        #region 刪除檔案
        /// <summary>
        /// 刪除單一檔案
        /// </summary>
        /// <param name="FilePath">欲刪除資料的虛擬路徑與檔案名稱</param>
        public static void DeleteFile(string FilePath)
        {
            try
            {
                File.Delete(HttpContext.Current.Server.MapPath(FilePath));
            }
            catch (Exception ex)
            {
                return;
            }
        }
        #endregion

        #region 複製資料夾
        /// <summary>
        /// 複製資料夾
        /// </summary>
        /// <param name="SourceDir">原始資料夾位置</param>
        /// <param name="DestDir">目的資料夾位置</param>
        /// <returns></returns>
        public static void MoveDir(string SourceDir, string DestDir)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            string SourcePath = Server.MapPath(SourceDir);
            string DestPath = Server.MapPath(DestDir);

            string fileName = "";
            string destFile = "";
            if (!System.IO.Directory.Exists(DestPath))
            {
                Directory.CreateDirectory(DestPath);
            }

            string[] files = System.IO.Directory.GetFiles(SourcePath);

            // Copy the files and overwrite destination files if they already exist.
            foreach (string s in files)
            {
                // Use static Path methods to extract only the file name from the path.
                fileName = System.IO.Path.GetFileName(s);
                destFile = System.IO.Path.Combine(DestPath, fileName);
                File.Copy(s, destFile, true);
            }

        }
        #endregion

        #region 移動檔案(單檔)

        /// <summary>
        /// 移動檔案(單檔)
        /// </summary>
        /// <param name="FilePath">原始檔案路徑</param>
        /// <param name="SourceDir">欲取代的資料夾位置 ex. /Temp/systemadmin/</param>
        /// <param name="DestDir">目的資料夾位置 ex. /Student/9311001/</param>
        /// <returns></returns>
        public static string MoveFile(string FilePath, string SourceDir, string DestDir)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            string NewFilePath = FilePath.Replace(SourceDir, DestDir);
            string SourcePath = Server.MapPath(FilePath);
            string CopyPath = Server.MapPath(NewFilePath);

            if (SourcePath != CopyPath)
            {
                FileInfo SourceFile = new FileInfo(SourcePath);

                CreateFolder(CopyPath.Replace(SourceFile.Name, ""));

                if (File.Exists(SourcePath))
                {
                    File.Copy(SourcePath, CopyPath, true);
                    File.Delete(SourcePath);
                }
            }

            return NewFilePath;
        }

        /// <summary>
        /// 移動檔案(單檔)
        /// </summary>
        /// <param name="FilePath">原始檔案路徑</param>
        /// <param name="SourceDir">欲取代的資料夾位置 ex. /Temp/systemadmin/</param>
        /// <param name="DestDir">目的資料夾位置 ex. /Student/9311001/</param>
        /// <param name="OverWrite">是否覆蓋同名檔案</param>
        /// <returns></returns>
        public static string MoveFile(string FilePath, string SourceDir, string DestDir, bool OverWrite)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            string SourcePath = Server.MapPath(FilePath);
            string CopyPath = FilePath.Replace(SourceDir, DestDir);

            if (OverWrite == true)
            {
                if (SourcePath != CopyPath)
                {
                    FileInfo SourceFile = new FileInfo(SourcePath);

                    CreateFolder(CopyPath.Replace(SourceFile.Name, ""));

                    File.Copy(SourcePath, CopyPath, true);
                    File.Delete(SourcePath);
                }
            }
            else
            {
                string FileName = GetFileName(CopyPath);
                if (File.Exists(CopyPath))
                {
                    int FileCount = 2;
                    string TempPath = CopyPath.Replace(FileName, FileCount.ToString() + "_" + FileName);

                    while (File.Exists(TempPath))
                    {
                        FileCount++;
                        TempPath = CopyPath.Replace(FileName, FileCount.ToString() + "_" + FileName);
                    }

                    FileName = FileCount.ToString() + "_" + FileName;
                    CopyPath = TempPath;
                }

                CreateFolder(CopyPath.Replace(FileName, ""));

                File.Copy(SourcePath, CopyPath, true);
                File.Delete(SourcePath);
            }

            return CopyPath;
        }


        #endregion

        public static string GetFileName(string Path)
        {
            string[] FileNameSplit = Path.Split('\\');
            return FileNameSplit[FileNameSplit.Length - 1];
        }

        /// <summary>
        /// 取得實體路徑
        /// </summary>
        /// <param name="Virtualpath">虛擬路徑</param>
        /// <returns></returns>
        public static string GetPhysicalPath(string Virtualpath)
        {
            HttpServerUtility Server = HttpContext.Current.Server;
            return Server.MapPath(Virtualpath);
        }
    }
}
