using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace ICCModule
{
    public static class CopyModel
    {
        /// <summary>
        /// 將同名屬性成員自動綁定 
        /// </summary>
        /// <typeparam name="T">來源型別</typeparam>
        /// <typeparam name="K">目標型別</typeparam>
        /// <param name="objFrom">來源物件</param>
        public static K Binding<T, K>(T objFrom, K objTo)
        {
            List<PropertyInfo> colList_from = typeof(T).GetProperties().ToList();
            List<PropertyInfo> colList_To = typeof(K).GetProperties().ToList();

            //K objTo = new K();
            //綁定名稱相同的欄位
            foreach (var col_from in colList_from)
            {
                //找同名屬性成員
                PropertyInfo col = colList_To.Find(x => x.Name == col_from.Name);
                //找不到就不綁定
                if (col == null)
                    continue;
                try
                {
                    //從來源物件抓取資料
                    var value = col_from.GetValue(objFrom,null);
                    //將變數綁定到要輸出的物件
                    col.SetValue(objTo, value, null);
                }
                catch (Exception ex)
                {   //這邊如果綁定失敗就先忽略,天曉得發生啥事...
                }
            }
            return objTo;
        }

        /// <summary>
        /// 自動綁定 會根據不同class泛型 進行淺層複製
        /// </summary>
        /// <typeparam name="T">來源型別</typeparam>
        /// <typeparam name="K">目標型別</typeparam>
        /// <param name="objFrom">來源物件</param>
        public static K Copy<T, K>(T objFrom)
            where K : new()
        {
            K objTo = new K();
            //回傳 同名屬性成員自動綁定
            return Binding<T, K>(objFrom, objTo);
        }

        /// <summary>
        /// 用序列化來複製物件
        /// </summary>
        /// <param name="originEntity">原始的 Entity List</param>
        /// <returns></returns>
        public static T Copy<T>(T origin)
             where T : new()
        {
            T newobject = default(T);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            try
            {
                bf.Serialize(stream, origin);
                stream.Seek(0, SeekOrigin.Begin);
                newobject = (T)bf.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return newobject;
        }

    }
}
