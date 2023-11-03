using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace IscomG2C.Utility.Library
{
    /// <summary>Excel處理類別 主要針對格式設定的部分作處理 因為語法很麻煩
    /// 
    /// </summary>
    public class Utility_WorkbookEngine
    {

        /// <summary>設定自動調整欄寬
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="column"></param>
        /// <param name="column_end"></param>
        /// <returns></returns>
        public static ISheet SetSheet_AutoSizeColumn(ISheet sheet, int column, int column_end)
        {
            for (int i = column; i <= column_end; i++)
            {
                //調整部分列的欄位寬度
                sheet.AutoSizeColumn(i, true);

                sheet.SetColumnWidth(i, sheet.GetColumnWidth(i) + 5);
            }
            return sheet;
        }

        /// <summary>設定Excel格式-加框線
        /// 
        /// </summary>
        /// <returns></returns>
        public static HSSFCellStyle SetStyle_Border(HSSFCellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;//.CellBorderType.THIN;
            oStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            //回傳設定
            return oStyle;
        }

        /// <summary>設定Excel格式-加框線
        /// 
        /// </summary>
        /// <returns></returns>
        public static ICellStyle SetStyle_Border(ICellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;//.CellBorderType.THIN;
            oStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            oStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            oStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            //回傳設定
            return oStyle;
        }

        /// <summary>設定Excel格式-灰色底色
        /// 
        /// </summary>
        /// <returns></returns>
        public static ICellStyle SetStyle_Background_Grey(ICellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            oStyle.FillPattern = FillPattern.SolidForeground;
            //回傳設定
            return oStyle;
        }

        /// <summary>設定Excel格式-加框線
        /// 
        /// </summary>
        /// <returns></returns>
        public static HSSFCellStyle SetStyle_Background_YELLOW(HSSFCellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;//.YELLOW.index;
            oStyle.FillPattern = FillPattern.SolidForeground;
            //回傳設定
            return oStyle;
        }

        /// <summary>設定Excel格式-加框線(紅底)
        /// 
        /// </summary>
        /// <returns></returns>
        public static HSSFCellStyle SetStyle_Background_RED(HSSFCellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;//.RED.index;
            oStyle.FillPattern = FillPattern.SolidForeground;
            //回傳設定
            return oStyle;
        }

        /// <summary>設定Excel格式-加框線(紅底)
        /// 
        /// </summary>
        /// <returns></returns>
        public static HSSFCellStyle SetStyle_Background_Pink(HSSFCellStyle oStyle)
        {
            //設定儲存格框線
            oStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Rose.Index;//.RED.index;
            oStyle.FillPattern = FillPattern.SolidForeground;
            //回傳設定
            return oStyle;
        }
    }
}
