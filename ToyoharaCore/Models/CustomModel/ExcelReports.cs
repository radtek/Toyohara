using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace ToyoharaCore.Models.CustomModel
{
    public class ExcelReports<T>
    {
        public List<UI_SELECT_GRID_SETTINGSResult> gridSettings { get; set; }
        IEnumerable<T> data { get; set; }
        string stored_procedure { get; set; }
        string physicalPath { get; set; }
        int rowCount { get; set; }
        int columnCount { get; set; }
        Action<ExcelWorksheet, List<UI_SELECT_GRID_SETTINGSResult>> headerAction { get; set; }
        public int workSheetNumber { get; set; }
        public ExcelPackage ep { get; set; }
        public ExcelWorksheet workSheet { get; set; }
        public int? user_id { get; set; }
        //public delegate IEnumerable<T> GetData();
        //public GetData getDataAction { get; set; }
        //public string @params { get; set; }
        //int? user_id { get; set; }
        private PortalDMTOSModel portalDMTOS { get; set; }
        public ExcelReports(IEnumerable<T> data, int rowCount, int columnCount, int? user_id, string physicalPath,
            string procedureName, int workSheetNumber, Action<ExcelWorksheet, List<UI_SELECT_GRID_SETTINGSResult>> headerAction)
        //, GetData getDataAction, string @params, int? user_id)
        {
            //gridSettings.OrderBy(x => x.number).ToList();
            this.stored_procedure = procedureName;
            this.portalDMTOS = new PortalDMTOSModel();
            this.gridSettings = portalDMTOS.UI_SELECT_GRID_SETTINGS(user_id, stored_procedure, null, 1);
            this.data = data;
            this.physicalPath = physicalPath;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.ep = new ExcelPackage(new FileInfo(physicalPath));
            this.workSheetNumber = workSheetNumber;
            this.workSheet = ep.Workbook.Worksheets[workSheetNumber];
            if (headerAction == null)
                this.headerAction = HeaderCreation;
            else
                this.headerAction = headerAction;
            //this.data = getDataAction();
            //this.@params = @params;
            //this.user_id = user_id;
        }
        private void HeaderCreation(ExcelWorksheet workSheet, List<UI_SELECT_GRID_SETTINGSResult> settings)
        {
            foreach (UI_SELECT_GRID_SETTINGSResult setting in settings)
            {
                if ((bool)setting.is_visible && (bool)setting.global_visible)
                {
                    workSheet.Column(columnCount).Width = ((double)setting.width / 7);
                    workSheet.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(setting.russian_field_description);
                    workSheet.Cells[rowCount + 1, columnCount].Value = columnCount;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.WrapText = true;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Font.Name = "Calibri";
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Font.Size = 8;
                    workSheet.Cells[rowCount, columnCount, rowCount + 1, columnCount].Style.Font.Bold = true;
                    columnCount++;
                }
            }
            workSheet.Row(rowCount + 1).Height = 20;
            columnCount = 1;
            rowCount = rowCount + 2;
        }
        public void ExcelReport(bool? is_date_time = false, int? searchRowCount = null)
        {
            //int? event_id = null;
            //PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            //if (data == null)
            //{                
            //    event_id=portalDMTOS.SYS_START_EVENT(user_id, stored_procedure, @params).FirstOrDefault().event_id;
            //    getDataAction();
            //}

            if (searchRowCount == null) { HeaderCreation(workSheet, gridSettings); }
            else
            {
                bool start = false;
                for (int row = workSheet.Dimension.Start.Row; row <= workSheet.Dimension.End.Row & start == false; row++)
                {
                    if (!start)
                    {
                        for (int col = 1; col <= searchRowCount; col++)
                        {
                            if (Convert.ToString(workSheet.Cells[row, col].Value) != Convert.ToString(col))
                            {
                                start = false;
                                break;
                            }
                            else
                            {
                                start = true;
                                rowCount = row + 1;
                            }
                        }
                    }
                }
                if (start == false)
                {
                    rowCount = 1;
                }
            }

            int startRowCount = rowCount;
            PropertyInfo[] fieldNames = typeof(T).GetProperties();
            bool outline_flag = fieldNames.Any(x => x.Name == "outline_level");
            int finishColumnCount = 0;
            foreach (var a in data)
            {
                foreach (UI_SELECT_GRID_SETTINGSResult settings in gridSettings)
                {


                    var color = fieldNames.Where(x => x.Name == "color").FirstOrDefault();
                    if (color != null)
                    {
                        var color_val = color.GetValue(a);
                        if (color_val != null && Convert.ToString(color_val) != "")
                        {
                            workSheet.Cells[rowCount, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            workSheet.Cells[rowCount, columnCount].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(Convert.ToInt32(color_val.ToString().Substring(0, 2), 16), Convert.ToInt32(color_val.ToString().Substring(2, 2), 16), Convert.ToInt32(color_val.ToString().Substring(4, 2), 16)));
                        }
                    }

                    try
                    {
                        finishColumnCount = columnCount > finishColumnCount ? columnCount : finishColumnCount;
                        var field = fieldNames.Where(x => x.Name == settings.field_description).FirstOrDefault().GetValue(a);
                        SetValueInCellExport(workSheet, field, settings, is_date_time);
                    }
                    catch { }
                }

                //if (gridSettings.Where(x=>).field_description == "OutlineLevel")
                if (outline_flag)
                    workSheet.Row(rowCount).OutlineLevel = Convert.ToInt32(fieldNames.Where(x => x.Name == "outline_level").FirstOrDefault().GetValue(a));
                rowCount++;
                columnCount = 1;
            }


            for(int i= startRowCount; i<=rowCount; i++)
            {
                for (int j = 1; j <= finishColumnCount; j++)
                {
                    workSheet.Cells[i, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[i, j].Style.WrapText = true;
                    workSheet.Cells[i, j].Style.Font.Name = "Calibri";
                    workSheet.Cells[i, j].Style.Font.Size = 8;
                }
            }


            ep.Save();
            ep = null;
            workSheet = null;
            //portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
        }
        public void SetValueInCellExport(ExcelWorksheet ew1, object value, UI_SELECT_GRID_SETTINGSResult setting, bool? is_date_time)
        {
            bool is_write = false;
            if (setting == null) is_write = true;
            else if ((bool)setting.global_visible && (bool)setting.is_visible) is_write = true;

            if (is_write)
            {
                if (value != null && value is string | value is DateTime)
                {
                    if (value is DateTime)
                        if (is_date_time == true)
                            value = DateTime.Parse(Convert.ToString(value));
                        else
                            value = DateTime.Parse(Convert.ToString(value)).ToShortDateString();

                    string[] arrayHTMLValue = Convert.ToString(value).Split(new string[] { "<br>" }, StringSplitOptions.None);
                    if (arrayHTMLValue != null && arrayHTMLValue.Length != 0)
                    {
                        ExcelRichTextCollection rtfCollection = ew1.Cells[rowCount, columnCount].RichText;
                        ExcelRichText ert = rtfCollection.Add(CommonMethods.HtmlToText(arrayHTMLValue[0]));
                        if (arrayHTMLValue[0].IndexOf("orange") > 0) ert.Color = System.Drawing.Color.Orange;
                        if (arrayHTMLValue.Length > 1)
                        {
                            for (int f = 1; f < arrayHTMLValue.Length; f++)
                            {
                                rtfCollection.Add(CommonMethods.HtmlToText("\n"));
                                ExcelRichText ert1 = rtfCollection.Add(CommonMethods.HtmlToText(arrayHTMLValue[f]));
                                if (arrayHTMLValue[f].IndexOf("orange") > 0) ert1.Color = System.Drawing.Color.Orange;
                            }
                        }
                    }
                    else
                    ew1.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(Convert.ToString(value));


                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                if (value != null && value is double | value is int | value is float | value is decimal)
                {
                    ew1.Cells[rowCount, columnCount].Value = double.Parse(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
                    ew1.Cells[rowCount, columnCount].Style.Numberformat.Format = CommaFind(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
                }
                //ew1.Cells[rowCount, columnCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.WrapText = true;
                //ew1.Cells[rowCount, columnCount].Style.Font.Name = "Calibri";
                //ew1.Cells[rowCount, columnCount].Style.Font.Size = 8;

                if (value is string)
                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                if (value is double | value is int | value is float)
                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ew1.Cells[rowCount, columnCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                columnCount++;
            }

           


        }
        //static
        public void SetValueInCellExport(ExcelWorksheet ew1, ref int rowCount, ref int columnCount, object value, UI_SELECT_GRID_SETTINGSResult setting)
        {

            bool is_write = false;
            if (setting == null) is_write = true;
            else if ((bool)setting.global_visible && (bool)setting.is_visible) is_write = true;

            if (is_write)
            {
                if (value != null && value is string | value is DateTime)
                {
                    if (value is DateTime)
                        value = DateTime.Parse(Convert.ToString(value)).ToShortDateString();
                    ew1.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(Convert.ToString(value));
                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                if (value != null && value is double | value is int | value is float | value is decimal)
                {
                    ew1.Cells[rowCount, columnCount].Value = double.Parse(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
                    ew1.Cells[rowCount, columnCount].Style.Numberformat.Format = CommaFind(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
                }
                //ew1.Cells[rowCount, columnCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //ew1.Cells[rowCount, columnCount].Style.WrapText = true;
                //ew1.Cells[rowCount, columnCount].Style.Font.Name = "Calibri";
                //ew1.Cells[rowCount, columnCount].Style.Font.Size = 8;

                if (value is string)
                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                if (value is double | value is int | value is float)
                    ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ew1.Cells[rowCount, columnCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                columnCount++;
            }
        }
        public void SetValueInCellExport(ExcelWorksheet ew1, int rowCount, int columnCount, object value, string color)
        {


            if (color != null && Convert.ToString(color) != "")
            {
                workSheet.Cells[rowCount, columnCount].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[rowCount, columnCount].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(Convert.ToInt32(color.ToString().Substring(0, 2), 16), Convert.ToInt32(color.ToString().Substring(2, 2), 16), Convert.ToInt32(color.ToString().Substring(4, 2), 16)));
            }

            if (value != null && value is string | value is DateTime)
            {
                if (value is DateTime)
                    value = DateTime.Parse(Convert.ToString(value)).ToShortDateString();
                ew1.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(Convert.ToString(value));
                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            if (value != null && value is double | value is int | value is float | value is decimal)
            {
                ew1.Cells[rowCount, columnCount].Value = double.Parse(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
                ew1.Cells[rowCount, columnCount].Style.Numberformat.Format = CommaFind(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));
            }
            //ew1.Cells[rowCount, columnCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //ew1.Cells[rowCount, columnCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //ew1.Cells[rowCount, columnCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //ew1.Cells[rowCount, columnCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //ew1.Cells[rowCount, columnCount].Style.WrapText = true;
            //ew1.Cells[rowCount, columnCount].Style.Font.Name = "Calibri";
            //ew1.Cells[rowCount, columnCount].Style.Font.Size = 8;

            if (value is string)
                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            if (value is double | value is int | value is float)
                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells[rowCount, columnCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
        public void ProcData()
        {
        }
        public static void HeaderCreation(ExcelWorksheet workSheet, ref int rowCount, ref int columnCount, List<UI_SELECT_GRID_SETTINGSResult> settings)
        {
            foreach (UI_SELECT_GRID_SETTINGSResult setting in settings)
            {
                if ((bool)setting.is_visible && (bool)setting.global_visible)
                {
                    workSheet.Column(columnCount).Width = ((double)setting.width / 7);
                    workSheet.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(setting.russian_field_description);
                    workSheet.Cells[rowCount + 1, columnCount].Value = columnCount;
                }

                columnCount++;
            }
            rowCount = rowCount + 1;
        }
        public static string CommaFind(string str)
        {
            string str1 = "#,##0.";
            if (str.IndexOf(",") > -1)
            {
                foreach (char n in str.Substring(str.IndexOf(",") + 1))
                { str1 = str1 + "0"; }
            }
            else
                str1 = "#,##0";
            return str1;
        }

    }
}
