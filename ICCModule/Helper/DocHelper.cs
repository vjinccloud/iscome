using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TemplateEngine.Docx;

namespace ICCModule.Helper
{
    public class DocHelper
    {
        /// <summary>
        /// 產生word
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="wordList"></param>
        /// <param name="listContent"></param>
        /// <returns></returns>
        public static BaseResult SetWord(string targetUrl, Dictionary<string, string> wordList, DataSet data = null)
        {
            var result = new BaseResult();

            //https://dotblogs.com.tw/AlenWu_coding_blog/2020/01/21/template_docx
            var _response = "";
            try
            {
                var templateUrl = HttpContext.Current.Server.MapPath("~/WordTemplate.docx");
                var fillContent = new Content();

                List<IContentItem> fields = null;
                TableContent tableContent = null;

                //FieldContent → 代換一般內容(RTF)
                fields = new List<IContentItem>();
                foreach (var item in wordList)
                {
                    if (item.Value == "hide()") fields.Add(new FieldContent(item.Key, "").Hide());
                    else fields.Add(new FieldContent(item.Key, ReplaceLowOrderASCIICharacters(item.Value ?? "").Replace("\n", "\r\n")));
                }
                if (data != null)
                    foreach (DataTable _table in data.Tables)
                    {
                        if (_table.Rows.Count > 0)
                        {
                            //TableContent → 代換表格並依照資料數自動產生列
                            tableContent = new TableContent(_table.TableName);
                            foreach (DataRow _row in _table.Rows)
                            {
                                if (File.Exists(_row["PicUrl"].ToString()))
                                {
                                    tableContent.AddRow(new FieldContent($"@PicNum", ReplaceLowOrderASCIICharacters(_row["PicNum"].ToString()).Replace("\n", "\r\n")),
                                                        new ImageContent($"@PicUrl", File.ReadAllBytes(_row["PicUrl"].ToString())));
                                }
                            }
                            if (tableContent.Rows != null && tableContent.Rows.Any()) fillContent.Tables.Add(tableContent); //表格丟Tables
                            else fillContent.Tables.Add(tableContent.Hide());
                        }
                        //if (_table.Rows.Count > 0)
                        //{
                        //    var columnName = new List<string>();
                        //    foreach (DataColumn _column in _table.Columns)
                        //    {
                        //        columnName.Add(_column.ColumnName);
                        //    }
                        //    var rContents = new RepeatContent(_table.TableName);
                        //    foreach (DataRow _row in _table.Rows)
                        //    {
                        //        List<IContentItem> dd = new List<IContentItem>();
                        //        foreach (var item in columnName)
                        //        {
                        //            if (item.Contains("PicUrl"))
                        //            {
                        //                if (File.Exists(_row[item].ToString()))
                        //                {
                        //                    //tableContent.AddRow(new ImageContent($"@{item}", File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/" + (_row[item].ToString())))));
                        //                    var d = new ImageContent($"@{item}", File.ReadAllBytes(_row[item].ToString()));
                        //                    dd.Add(d);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                //tableContent.AddRow(new FieldContent($"@{item}", _row[item].ToString()));
                        //                IContentItem d = new FieldContent($"@{item}", ReplaceLowOrderASCIICharacters(_row[item].ToString()).Replace("\n", "\r\n"));
                        //                dd.Add(d);
                        //            }
                        //        }
                        //        if (dd.Count > 0)
                        //        {
                        //            var dtype = dd.ToArray().GetType();
                        //            rContents.AddItem(dd.ToArray());
                        //        }
                        //    }
                        //    if (rContents.Items != null && rContents.Items.Count > 0) fillContent.Repeats.Add(rContents);
                        //}
                        else
                        {
                            fillContent.Tables.Add(new TableContent(_table.TableName));
                        }
                    }

                //最後把要代換的所有資料丟到Content內
                fields.ForEach(f => fillContent.Fields.Add(f as FieldContent)); //一般內容丟Fields

                string _directory = HttpContext.Current.Server.MapPath("~/word");
                FileHelper.CreateFolder(_directory);

                //複製 Template 檔案
                //File.Copy(templateUrl,_filePath + @"\word\" + targetUrl, true);
                targetUrl = targetUrl.Replace("/", "").Replace(":", "");
                File.Copy(templateUrl, HttpContext.Current.Server.MapPath("~/word/" + targetUrl), true);

                //將Content丟給TemplateProcessor處理，SetRemoveContentControls=true表示要清除標籤內文字，如不要清除則設為false
                //using (var outputDocument = new TemplateProcessor(_filePath + @"\word\" + targetUrl).SetRemoveContentControls(true))
                using (var outputDocument = new TemplateProcessor(HttpContext.Current.Server.MapPath("~/word/" + targetUrl)).SetRemoveContentControls(true))
                {
                    try
                    {
                        outputDocument.FillContent(fillContent);
                        outputDocument.SaveChanges(); //儲存變更檔案       
                        _response = @"word\" + targetUrl;
                    }
                    catch (Exception e)
                    {
                        result.result = false;
                        result.Msg = e.Message + e.InnerException?.Message;
                        return result;
                    }
                    finally
                    {
                        outputDocument.Dispose();
                    }
                }
                //File.Copy(HttpContext.Current.Server.MapPath("~/word/" + targetUrl), HttpContext.Current.Server.MapPath("~/word/" + targetUrl.Replace(".docx", ".doc")), true);
                //_response = _response.Replace(".docx", ".doc");

                result.result = true;
                result.Msg = _response;
            }
            catch (Exception e)
            {
                result.result = false;
                result.Msg = e.Message + e.InnerException?.Message;
                return result;
            }

            return result;
        }
        /// <summary>
        /// 避免[十六進位值是無效的字元]的異常
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        private static string ReplaceLowOrderASCIICharacters(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat(" ", ss);//&#x{0:X};
                else info.Append(cc);
            }
            return info.ToString();
        }
    }
}
