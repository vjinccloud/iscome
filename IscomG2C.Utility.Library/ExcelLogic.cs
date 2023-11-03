using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

namespace CommonModule.Logic
{
    public class NPOIExcelLogic
    {
        /// <summary>取得儲存格的欄位值
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="rowIndex">列數位置(從0開始)</param>
        /// <param name="columnIndex">行數位置(從0開始)</param>
        /// <returns></returns>
        public String GetCellValue(ref ISheet ws, int rowIndex, int columnIndex)
        {
            IRow row = ws.GetRow(rowIndex);
            if (row == null || row.Cells.Count == 0)
            {
                return null;
            }
            else
            {
                //  -1代表從0開始計算
                if ((row.Cells.Count) >= columnIndex)
                {
                    String cellValue = row.GetCell(columnIndex).ToString();
                    return cellValue;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>建立Cell
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="cellStyle">儲存格樣式</param>
        /// <param name="rowIndex">列數(從0開始)</param>
        /// <param name="columnStartIndex">行數開始位置(從0開始)</param>
        /// <param name="columnEndIndex">行數結束位置(從0開始)</param>
        public void CreateTableCell(ref ISheet ws, ref  HSSFCellStyle cellStyle, int rowIndex, int columnStartIndex, int columnEndIndex)
        {
            for (int i = columnStartIndex; i <= columnEndIndex; i++)
            {
                ws.GetRow(rowIndex).CreateCell(i).SetCellValue("");
                ws.GetRow(rowIndex).GetCell(i).CellStyle = cellStyle;
            }
        }

        /// <summary>建立Cell
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="rowIndex">列數(從0開始)</param>
        /// <param name="columnStartIndex">行數開始位置(從0開始)</param>
        /// <param name="columnEndIndex">行數結束位置(從0開始)</param>
        public void CreateTableCell(ref ISheet ws, int rowIndex, int columnStartIndex, int columnEndIndex)
        {
            for (int i = columnStartIndex; i <= columnEndIndex; i++)
            {
                ws.GetRow(rowIndex).CreateCell(i).SetCellValue("");
                //ws.GetRow(rowIndex).GetCell(i).CellStyle = cellStyle;
            }
        }

        /// <summary>建立Cell
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="cellStyle">儲存格樣式</param>
        /// <param name="rowStartIndex">列數開始位置(從0開始)</param>
        /// <param name="rowEndIndex">列數結束位置(從0開始)</param>
        /// <param name="columnStartIndex">行數開始位置(從0開始)</param>
        /// <param name="columnEndIndex">行數結束位置(從0開始)</param>
        public void CreateTableCell(ref ISheet ws, ref  HSSFCellStyle cellStyle, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex)
        {
            for (int x = rowStartIndex; x <= rowEndIndex; x++)
            {
                IRow row = ws.GetRow(x);
                if (row == null || row.Cells.Count == 0)
                {
                    ws.CreateRow(x);
                }

                for (int y = columnStartIndex; y <= columnEndIndex; y++)
                {
                    ws.GetRow(x).CreateCell(y).SetCellValue("");
                    ws.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
        }

        /// <summary>新增跨列合併儲存格
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="cellStyle">儲存格樣式</param>
        /// <param name="rowIndex">列數(從0開始)</param>
        /// <param name="isColSpan">是否執行合併儲存格</param>
        /// <param name="colSpanCount">合併儲存格數量</param>
        /// <param name="cellValue">儲存格值</param>
        /// <param name="columnIndex">行數開始位置(從0開始)</param>
        public void AddColumnSpanContent(ref ISheet ws, ref HSSFCellStyle cellStyle, int rowIndex, bool isColSpan, int colSpanCount, string cellValue, int columnIndex = 0)
        {
            IRow row = ws.GetRow(rowIndex);
            if (row == null || row.Cells.Count == 0)
            {
                ws.CreateRow(rowIndex);
            }
            ws.GetRow(rowIndex).CreateCell(columnIndex).SetCellValue(cellValue);
            ws.GetRow(rowIndex).GetCell(columnIndex).CellStyle = cellStyle;

            if (isColSpan)
            {
                int lastIdnex = columnIndex + colSpanCount;
                this.CreateTableCell(ref ws, ref cellStyle, rowIndex, (columnIndex + 1), lastIdnex);
                CellRangeAddress cellRegion = new CellRangeAddress(rowIndex, rowIndex, columnIndex, lastIdnex);
                if (cellRegion != null)
                {
                    ws.AddMergedRegion(cellRegion);
                }
            }
        }

        /// <summary>新增跨列合併儲存格
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="rowIndex">列數(從0開始)</param>
        /// <param name="isColSpan">是否執行合併儲存格</param>
        /// <param name="colSpanCount">合併儲存格數量</param>
        /// <param name="cellValue">儲存格值</param>
        /// <param name="columnIndex">行數開始位置(從0開始)</param>
        public void AddColumnSpanContent(ref ISheet ws, int rowIndex, bool isColSpan, int colSpanCount, string cellValue, int columnIndex = 0)
        {
            IRow row = ws.GetRow(rowIndex);
            if (row == null || row.Cells.Count == 0)
            {
                ws.CreateRow(rowIndex);
            }
            ws.GetRow(rowIndex).CreateCell(columnIndex).SetCellValue(cellValue);
            //ws.GetRow(rowIndex).GetCell(columnIndex).CellStyle = cellStyle;

            if (isColSpan)
            {
                int lastIdnex = columnIndex + colSpanCount;
                this.CreateTableCell(ref ws, rowIndex, (columnIndex + 1), lastIdnex);
                CellRangeAddress cellRegion = new CellRangeAddress(rowIndex, rowIndex, columnIndex, lastIdnex);
                if (cellRegion != null)
                {
                    ws.AddMergedRegion(cellRegion);
                }
            }
        }


        /// <summary>新增跨行合併儲存格
        /// 
        /// </summary>
        /// <param name="ws">EXCEL</param>
        /// <param name="cellStyle">儲存格樣式</param>
        /// <param name="rowStartIndex">列數(從0開始)</param>
        /// <param name="isRowSpan">是否執行合併儲存格</param>
        /// <param name="rowSpanCount">合併儲存格數量</param>
        /// <param name="cellValue">儲存格值</param>
        /// <param name="colIndex">列數(從0開始)</param>
        public void AddRowSpanContent(ref ISheet ws, ref HSSFCellStyle cellStyle, int rowStartIndex, bool isRowSpan, int rowSpanCount, string cellValue, int colIndex = 0)
        {
            IRow row = ws.GetRow(rowStartIndex);
            if (row == null || row.Cells.Count == 0)
            {
                ws.CreateRow(rowStartIndex);
            }
            ws.GetRow(rowStartIndex).CreateCell(colIndex).SetCellValue(cellValue);
            ws.GetRow(rowStartIndex).GetCell(colIndex).CellStyle = cellStyle;

            if (isRowSpan)
            {
                int rowEndIndex = rowStartIndex + rowSpanCount - 1;
                this.CreateTableCell(ref ws, ref cellStyle, (rowStartIndex + 1), rowEndIndex, colIndex, colIndex);
                CellRangeAddress cellRegion = new CellRangeAddress(rowStartIndex, rowEndIndex, (colIndex), (colIndex));
                if (cellRegion != null)
                {
                    ws.AddMergedRegion(cellRegion);
                }
            }
        }

        /// <summary>客製化轉換npoi色瑪的index
        /// 
        /// </summary>
        /// <param name="workbook">excel</param>
        /// <param name="SystemColour">RGB</param>
        /// <returns></returns>
        public short GetXLColour(HSSFWorkbook workbook, System.Drawing.Color SystemColour)
        {
            short s = 0;
            //HSSFPalette XlPalette = workbook.GetCustomPalette();
            //HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //if (XlColour == null)
            //{
            //    if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
            //    {
            //        if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 64)
            //        {
            //            NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE = 64;
            //            NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE += 1;
            //            XlColour = XlPalette.AddColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //        }
            //        else
            //        {
            //            XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
            //        }

            //        s = XlColour.GetIndex();
            //    }

            //}
            //else
            //{
            //    s = XlColour.GetIndex();
            //}
            return s;
        }

        /// <summary>EXCEL下拉設定
        /// 
        /// </summary>
        /// <param name="sErrMsg">錯誤訊息</param>
        /// <param name="listResult">結果</param>
        /// <param name="helper"></param>
        /// <param name="sheet">Excel頁籤</param>
        /// <param name="cellsCount">欄位號碼</param>
        public void SetExcelDropdownList(ref string sErrMsg, List<string> listResult, IDataValidationHelper helper, ISheet sheet, int cellsCount)
        {
            IDataValidationConstraint DataValidationConstraint = helper.CreateExplicitListConstraint(listResult.ToArray());
            CellRangeAddressList CellRegions = new CellRangeAddressList(1, 200000, cellsCount, cellsCount);
            IDataValidation data_list = helper.CreateValidation(DataValidationConstraint, CellRegions);
            sheet.AddValidationData(data_list);
        }
    }
}
