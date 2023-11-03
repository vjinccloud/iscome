using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ICCModule.Helper
{
    public class ExcelHelper
    {
        /// <summary>
        /// 將Excel轉為DataTable-xlsx
        /// </summary>
        /// <param name="excelFileStream">原始路徑</param>
        /// <param name="sheetIndex">資料列</param>
        /// <param name="headerRowIndex">目標文件夾</param>
        public static DataTable RenderDataTableFromExcelXlsx(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            IWorkbook workbook = new XSSFWorkbook(excelFileStream);
            ISheet sheet = (XSSFSheet)workbook.GetSheetAt(sheetIndex);

            DataTable table = new DataTable();

            IRow headerRow = (XSSFRow)sheet.GetRow(headerRowIndex);
            if (headerRow != null)
            {
                int cellCount = headerRow.LastCellNum;
                int ScellCount = 0;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    if (!string.IsNullOrEmpty(headerRow.GetCell(i).StringCellValue))
                    {
                        table.Columns.Add(column);
                        ScellCount++;
                    }
                }

                for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = (XSSFRow)sheet.GetRow(i);
                    if (row == null || row.FirstCellNum <0) continue;
                    DataRow dataRow = table.NewRow();
                    int emtyCount = 1;

                    for (int j = row.FirstCellNum; j < ScellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (string.IsNullOrEmpty(row.GetCell(j).ToString()))
                            {
                                emtyCount++;
                            }
                            //String  Numeric
                            //如要針對不同型別做個別處理，可善用.CellType判斷型別
                            //再用.StringCellValue, .DateCellValue, .NumericCellValue...取值
                            //此處只簡單轉成字串
                            switch (row.GetCell(j).CellType.ToString())
                            {
                                case "String":
                                    dataRow[j] = row.GetCell(j).StringCellValue;
                                    break;
                                case "Numeric":
                                    dataRow[j] = row.GetCell(j).NumericCellValue;
                                    break;
                                default:
                                    dataRow[j] = row.GetCell(j).ToString();
                                    break;
                            }

                        }
                        else
                        {
                            emtyCount++;
                        }
                    }
                    if (emtyCount < ScellCount)
                    {
                        table.Rows.Add(dataRow);
                    }

                }

            }
            excelFileStream.Close();
            return table;
        }
        /// <summary>
        /// 將Excel轉為DataTable-xls
        /// </summary>
        /// <param name="excelFileStream">原始路徑</param>
        /// <param name="sheetIndex">資料列</param>
        /// <param name="headerRowIndex">目標文件夾</param>
        public static DataTable RenderDataTableFromExcelXls(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(excelFileStream);
            ISheet sheet = (HSSFSheet)workbook.GetSheetAt(sheetIndex);

            DataTable table = new DataTable();

            IRow headerRow = (HSSFRow)sheet.GetRow(headerRowIndex);
            if (headerRow != null)
            {
                int cellCount = headerRow.LastCellNum;
                int ScellCount = 0;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    if (!string.IsNullOrEmpty(headerRow.GetCell(i).StringCellValue))
                    {
                        table.Columns.Add(column);
                        ScellCount++;
                    }
                }

                for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = (HSSFRow)sheet.GetRow(i);
                    if (row == null || row.FirstCellNum < 0) continue;
                    DataRow dataRow = table.NewRow();
                    int emtyCount = 1;

                    for (int j = row.FirstCellNum; j < ScellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (string.IsNullOrEmpty(row.GetCell(j).ToString()))
                            {
                                emtyCount++;
                            }
                            //String  Numeric
                            var aa = row.GetCell(j).CellType.ToString();
                            //如要針對不同型別做個別處理，可善用.CellType判斷型別
                            //再用.StringCellValue, .DateCellValue, .NumericCellValue...取值
                            //此處只簡單轉成字串
                            switch (row.GetCell(j).CellType.ToString())
                            {
                                case "String":
                                    dataRow[j] = row.GetCell(j).StringCellValue;
                                    break;
                                case "Numeric":
                                    dataRow[j] = row.GetCell(j).NumericCellValue;
                                    break;
                                default:
                                    dataRow[j] = row.GetCell(j).ToString();
                                    break;
                            }

                        }
                        else
                        {
                            emtyCount++;
                        }
                    }
                    if (emtyCount < ScellCount)
                    {
                        table.Rows.Add(dataRow);
                    }

                }
            }
            excelFileStream.Close();
            return table;
        }
        /// <summary>
        /// List To DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// List To DataTable With TableName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> data, string tableName)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            table.TableName = tableName;
            return table;
        }
        /// <summary>
        /// 單工作表
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        public static MemoryStream RenderDataTableToExcel(DataTable sourceTable,List<string> numColumnList = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                #region 生成檔案

                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet Name"
                });

                #endregion 生成檔案

                #region 寫入資料

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                // 建立欄位

                // handling header. 
                Row fieldRow = new Row();
                foreach (DataColumn column in sourceTable.Columns)
                {
                    fieldRow.Append(
                        new Cell() { CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName), DataType = CellValues.String }
                    );
                }                    
                sheetData.AppendChild(fieldRow);
                //// 輸入資料
                foreach (DataRow row in sourceTable.Rows)
                {
                    Row rows = new Row();
                    foreach (DataColumn column in sourceTable.Columns)
                    {
                        var _dataType = CellValues.String;
                        if (numColumnList != null)
                        {
                            if (numColumnList.Contains(column.ColumnName)) _dataType = CellValues.Number;
                        }
                        rows.Append(
                            new Cell() { CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(row[column].ToString()), DataType = _dataType }
                        );
                    }
                    sheetData.AppendChild(rows);
                }

                #endregion 寫入資料
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        public static MemoryStream RenderDataTableToExcel(DataSet sourceTables, List<string> numColumnList = null)
        {
            UInt32Value sheet = 1;
            MemoryStream memoryStream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                foreach (DataTable sourceTable in sourceTables.Tables)
                {
                    #region 生成檔案
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    Worksheet workSheet = new Worksheet();
                    SheetData sheetData = new SheetData();

                    #endregion 生成檔案

                    #region 寫入資料

                    //SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // 建立欄位

                    // handling header. 
                    Row fieldRow = new Row();
                    foreach (DataColumn column in sourceTable.Columns)
                        fieldRow.Append(
                            new Cell() { CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName), DataType = CellValues.String }
                        );
                    sheetData.AppendChild(fieldRow);
                    //// 輸入資料
                    foreach (DataRow row in sourceTable.Rows)
                    {
                        Row rows = new Row();
                        foreach (DataColumn column in sourceTable.Columns)
                        {
                            var _dataType = CellValues.String;
                            if (numColumnList != null)
                            {
                                if (numColumnList.Contains(column.ColumnName)) _dataType = CellValues.Number;
                            }
                            rows.Append(
                                new Cell() { CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(row[column].ToString()), DataType = _dataType }
                            );
                        }
                        sheetData.AppendChild(rows);
                    }
                    workSheet.AppendChild(sheetData);
                    worksheetPart.Worksheet = workSheet;

                    Sheet newSheets = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = sheet,
                        Name = sourceTable.TableName
                    };
                    sheets.Append(newSheets);
                    #endregion 寫入資料
                    sheet++;
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        /// <summary>
        /// 通過DataTable獲得CSV格式資料
        /// </summary>
        /// <param name="dataTable">資料表</param>
        /// <returns>CSV字串資料</returns>
        public static StringBuilder GetCSVFormatData(DataTable dataTable)
        {
            StringBuilder StringBuilder = new StringBuilder();
            // 寫出表頭
            var columnName = "";
            foreach (DataColumn DataColumn in dataTable.Columns)
            {
                if (!string.IsNullOrEmpty(columnName)) columnName += "," + DataColumn.ColumnName;
                else columnName += DataColumn.ColumnName;
            }
            StringBuilder.Append(columnName);
            StringBuilder.Append("\n");
            // 寫出資料
            int count = 0;
            foreach (DataRow dataRowView in dataTable.Rows)
            {
                count++;
                foreach (DataColumn DataColumn in dataTable.Columns)
                {
                    string field = dataRowView[DataColumn.ColumnName].ToString();

                    if (field.IndexOf('"') >= 0)
                    {
                        field = field.Replace("\"", "\"\"");
                    }
                    field = field.Replace("  ", " ");
                    if (field.IndexOf(',') >= 0 || field.IndexOf('"') >= 0 || field.IndexOf('<') >= 0 || field.IndexOf('>') >= 0 || field.IndexOf("'") >= 0)
                    {
                        field = "\"" + field + "\"";
                    }
                    StringBuilder.Append(field + ",");
                    field = string.Empty;
                }
                if (count != dataTable.Rows.Count)
                {
                    StringBuilder.Append("\n");
                }
            }
            return StringBuilder;
        }
    }
}
