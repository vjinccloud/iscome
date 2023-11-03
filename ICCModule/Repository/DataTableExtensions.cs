using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ICCModule.Repository
{

    public static class DataTableExtensions
    {
        /// <summary>DataTable 轉換成 List結構物件
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        /// <summary>單筆資料綁定 根據
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="properties">類別的屬性值</param>
        /// <returns></returns>
        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            var Columns = row.Table.Columns;

            T item = new T();
            foreach (var property in properties)
            {
                //檢查是否有對應欄位 沒有則略過
                if (Columns[property.Name] == null)
                    continue;
                //取得輸入值
                var Value = row[property.Name];
                //判斷null值
                if (Value == System.DBNull.Value)
                {
                    Value = null;
                }
                property.SetValue(item, Value, null);
            }
            return item;
        }
    }
}
