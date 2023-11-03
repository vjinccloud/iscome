using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;

namespace IscomG2C.Utility
{

    /// <summary>公用模組-SelectListItem
    /// 
    /// </summary>
    public class Utility_SelectListItem
    {
        /// <summary>根據指定的Value列 轉換成 Text列字串
        /// 
        /// </summary>
        /// <param name="RefItemList">參考的項目列</param>
        /// <param name="ValueList">Value列(字串格式)</param>
        /// <param name="InputMark">Value列的分隔符號</param>
        /// <param name="OuputMark">輸出的分隔符號</param>
        /// <returns></returns>
        public static string ValueList_To_TextList(List<SelectListItem> RefItemList, string ValueList, string InputMark, string OuputMark)
        {
            var list = Select_ItemList(RefItemList, ValueList, InputMark);
            var TextList = list.Select(x => x.Text).ToList();
            return Utility_String.GetString_By_StringList(TextList, OuputMark);
        }

        /// <summary>根據輸入的Value列 篩選符合的資料
        /// 
        /// </summary>
        /// <param name="inputItemList">輸入的項目列</param>
        /// <param name="ValueList">要取得的Value列(字串格式)</param>
        /// <param name="Mark">Value列的分隔符號</param>
        /// <returns></returns>
        public static List<SelectListItem> Select_ItemList(List<SelectListItem> inputItemList, string ValueList, string Mark)
        {
            //處理字串分割 若無值或分割失敗則會則回傳空陣列
            var input = Utility_String.GetList_Split(ValueList, Mark);
            //根據輸入的Value列 篩選符合的資料
            return Select_ItemList(inputItemList, input);
        }

        /// <summary>根據輸入的Value列 篩選符合的資料
        /// 
        /// </summary>
        /// <param name="inputItemList">輸入的項目列</param>
        /// <param name="ValueList">要篩選的Value列</param>
        /// <returns></returns>
        public static List<SelectListItem> Select_ItemList(List<SelectListItem> inputItemList, List<string> ValueList)
        {
            if (inputItemList == null)
                return inputItemList;

            List<SelectListItem> RetList = new List<SelectListItem>();

            foreach (var value in ValueList)
            {
                var item = inputItemList.Find(x => x.Value == value);
                if (item != null)
                    RetList.Add(item);
            }
            return RetList;
        }

        /// <summary>根據輸入的DataSet 取得下拉式選單資料
        /// 
        /// </summary>
        /// <param name="CarType"></param>
        /// <returns></returns>
        public static List<SelectListItem> Get_ItemList(DataSet ds, string ColName_Text, string ColName_Value)
        {
            List<SelectListItem> tmpList = new List<SelectListItem>();

            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SelectListItem Item = new SelectListItem();
                    Item.Text = row[ColName_Text].ToString();
                    Item.Value = row[ColName_Value].ToString();

                    tmpList.Add(Item);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex);
                //throw;
            }


            return tmpList;
        }

        /// <summary>根據輸入的字串列 取得下拉式選單資料
        /// 
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static List<SelectListItem> Get_ItemList(List<string> stringList)
        {
            List<SelectListItem> RetList = new List<SelectListItem>();

            foreach (var item in stringList)
            {
                SelectListItem Selectitem = new SelectListItem();
                Selectitem.Text = item;
                Selectitem.Value = item;
                RetList.Add(Selectitem);
            }
            return RetList;
        }

        /// <summary>根據 ItemList 轉換為 html中的option字串
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Get_HtmlOptionString(List<SelectListItem> ItemList)
        {
            string RtnValue = "";
            foreach (var item in ItemList)
            {
                RtnValue += "<option value=\"" + item.Value + "\">" + item.Text + " </option> ";
            }
            return RtnValue;
        }

        /// <summary>複製一份ItemList
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <returns></returns>
        public static List<SelectListItem> Copy(List<SelectListItem> ItemList)
        {
            List<SelectListItem> retList = new List<SelectListItem>();
            foreach (var item in ItemList)
            {
                retList.Add(Copy(item));
            }
            return retList;
        }

        /// <summary>複製一筆Item
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SelectListItem Copy(SelectListItem item)
        {
            SelectListItem item_new = new SelectListItem();

            item_new.Text = item.Text;
            item_new.Selected = item.Selected;
            item_new.Value = item.Value;
            return item_new;
        }

        /// <summary>判斷ItemList內是否有值? 如果沒有則加入此Item
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<SelectListItem> Add_IfNotExist(List<SelectListItem> ItemList, SelectListItem item)
        {
            if (ItemList == null)
                return ItemList;
            if (item == null)
                return ItemList;

            if (!ItemList.Exists(x =>
                x.Value == item.Value))
            {   //如果不存在則加入
                ItemList.Add(item);
            }

            return ItemList;
        }

        /// <summary>判斷ItemList內是否有Value值? 如果沒有則加入此Value
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static List<SelectListItem> Add_IfNotExist(List<SelectListItem> ItemList, string Value)
        {
            if (Utility_String.IsNullValue(Value))
                return ItemList;//如果Value值為空值或--- 則不處理

            SelectListItem item = new SelectListItem();
            item.Text = Value;
            item.Value = Value;

            return Add_IfNotExist(ItemList, item);
        }

        /// <summary>設定選擇的值
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static List<SelectListItem> Set_SelectValue(List<SelectListItem> ItemList, string Value)
        {
            //判斷輸入的List非空
            if (ItemList == null)
                return null;

            //統一設定為"不選"
            ItemList.ForEach(x => x.Selected = false);

            //找到指定的Value對應的Item
            var item =
            ItemList.Find(x => x.Value == Value);

            //如果找不到
            if (item == null)
                return ItemList;//找不到值

            //將找到的item設為"選取"
            item.Selected = true;


            return ItemList;
        }

        /// <summary>取得文字部分
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string GetText(List<SelectListItem> ItemList, string Value, string DefaultText = "")
        {
            if (ItemList == null)
                return DefaultText;

            var item = ItemList.Find(x => x.Value == Value);
            if (item == null)
                return DefaultText;
            return item.Text;
        }

        /// <summary>取得文字部分
        /// 
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string GetText(List<SelectListItem> ItemList, Int64 Value, string DefaultText = "")
        {
            return GetText(ItemList, Value.ToString(), DefaultText);
        }



    }
}
