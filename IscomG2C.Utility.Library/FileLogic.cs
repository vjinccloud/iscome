using System;
using System.IO;
using System.Security.Cryptography;

namespace IscomG2C.Utility
{
    public class FileLogic
    {
        /// <summary>
        /// 檢查檔案格式
        /// </summary>
        /// <param name="File_Name">檔案完整名稱 ex. Test.xls</param>
        /// <param name="FileExtName">檔案副檔名 ex. .xls</param>
        /// <returns></returns>
        public bool CheckFileFormat(string File_Name, string FileExtName)
        {
            //檢查檔案格式
            if (Path.GetExtension(File_Name).ToUpper() != FileExtName)
            {
                return false;
            }
            return true;
        }

        /// <summary>刪除單一檔案
        /// 
        /// </summary>
        /// <param name="fileFullPath"></param>
        public void DeleteFile(string fileFullPath)
        {
            FileInfo file = new FileInfo(fileFullPath);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        /// <summary>刪除所有資料夾內的檔案
        /// 
        /// </summary>
        /// <param name="fullFolderPath">資料夾完整路徑</param>
        public void DeleteAllTypeInFolder(string fullFolderPath)
        {
            if (Directory.Exists(fullFolderPath))
            {
                System.IO.DirectoryInfo virtualFolderInfo = new DirectoryInfo(fullFolderPath);

                //  刪除此資料內的所有檔案
                foreach (FileInfo file in virtualFolderInfo.GetFiles())
                {
                    file.Delete();
                }

                //  刪除此資料夾內的所有資料夾
                foreach (DirectoryInfo dir in virtualFolderInfo.GetDirectories())
                {
                    dir.Delete(true);
                }

                //  刪除本身
                System.IO.Directory.Delete(fullFolderPath);
            }
        }

        /// <summary>複製資料夾及子目錄的所有檔案
        /// 
        /// </summary>
        /// <param name="sourceSaaSWebSiteDir">原始資料夾</param>
        /// <param name="destSaaSBackupDir">目的資料夾</param>
        /// <param name="copySubDirs">是否複製子目錄</param>
        public void CopyDirectory(string srcDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(srcDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + srcDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>複製資料夾下所有子資料夾及檔案
        /// 
        /// </summary>
        /// <param name="SourcePath">來源路徑</param>
        /// <param name="TargetPath">目的路徑</param>
        public void CopyDirectory(string SourcePath, string TargetPath)
        {
            if (!Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(TargetPath);
            }

            DirectoryInfo SourceDirInfo = new DirectoryInfo(SourcePath);
            DirectoryInfo[] ChildDirArray = SourceDirInfo.GetDirectories();
            foreach (DirectoryInfo ChildDir in ChildDirArray)
            {
                string path = Path.Combine(TargetPath, ChildDir.Name);
                CopyDirectory(ChildDir.FullName, path);
                //CopyDirectory(ChildDir.FullName, TargetPath + "\\" + ChildDir.Name);
            }

            FileInfo[] ChildFileArray = SourceDirInfo.GetFiles();
            foreach (FileInfo ChildFile in ChildFileArray)
            {
                string fileName = ChildFile.Name;
                string destFile = System.IO.Path.Combine(TargetPath, fileName);
                System.IO.File.Copy(ChildFile.FullName, destFile, true);
            }
        }

        /// <summary>判斷資料夾是否存在，不存在則建立
        /// 
        /// </summary>
        /// <param name="fullFolderPath">資料夾完整路徑</param>
        public void CheckFolderExisted(string fullFolderPath)
        {
            bool isExists = Directory.Exists(fullFolderPath);

            if (!isExists)
            {
                Directory.CreateDirectory(fullFolderPath);
            }
        }

        /// <summary>複製檔案
        /// 
        /// </summary>
        /// <param name="srcFullName">原始檔案完整路徑</param>
        /// <param name="destFullName">原始檔案完整路徑</param>
        public void CopyFile(string srcFullName, string destFullName)
        {
            (new FileInfo(srcFullName)).CopyTo(destFullName, true);
        }

        /// <summary>判斷資料夾是否存在
        /// 
        /// </summary>
        /// <param name="fullFolderPath">資料夾完整路徑</param>
        /// <returns></returns>
        public bool IsExistsFolder(string fullFolderPath)
        {
            bool isExists = Directory.Exists(fullFolderPath);
            return isExists;
        }

        /// <summary>
        /// 判斷檔案是否存在
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool IsFileExisted(string fullFileName)
        {
            return System.IO.File.Exists(fullFileName);
        }

        /// <summary>
        /// 取回指定路徑下檔案的 MD5 編碼 Hash code
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns></returns>
        //public static string CalculateMD5(string filePath)
        //{
        //    using (var md5 = MD5.Create())
        //    {
        //        using (var stream = File.OpenRead(filePath))
        //        {
        //            var hash = md5.ComputeHash(stream);
        //            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        //        }
        //    }
        //}

        /// <summary>
        /// 取回指定路徑下檔案的 MD5 編碼 Hash code
        /// </summary>
        /// <param name="stream">檔案流</param>
        /// <returns></returns>
        public static string CalculateMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
