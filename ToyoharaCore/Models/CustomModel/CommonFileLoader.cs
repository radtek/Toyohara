using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToyoharaCore.Models;
using ToyoharaCore.Attributes;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using ToyoharaCore.Models.CustomModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
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
    public class CommonFileLoader<T>
    { public CommonFileLoader(int loading_type_id, string file_name, string summary, int user_id,
                              string stored_procedure_select, 
                             // int columnCountSelect, int rowCountSelect, int ColumnCount, int RowCount,
                              string stored_procedure_insert, List<ProcedureParam> selectProcedureParams, List<ProcedureParam> insertProcedureParams, int workSheetNumber,
                              string excelFileName, string insertGuid, string reportNameForUser, int real_user_id,
                              Action<ExcelWorksheet, List<UI_SELECT_GRID_SETTINGSResult>> headerAction =null
        ) {
            this.loading_type_id = loading_type_id;
            //this.description = description;
            this.file_name = file_name;
            this.summary = summary;
            this.user_id = user_id;
            this.stored_procedure_select = stored_procedure_select;
            this.stored_procedure_insert = stored_procedure_insert;
            this.selectProcedureParams = selectProcedureParams;
            //this.ColumnCount = ColumnCount;
            //this.RowCount = RowCount;
            //this.columnCountSelect = columnCountSelect;
            //this.rowCountSelect = rowCountSelect;
            this.workSheetNumber = workSheetNumber;
            this.portalDMTOS = new PortalDMTOSModel();
            this.grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(user_id, stored_procedure_select, null, 1).ToList();
            this.fieldNamesInsert = typeof(T).GetProperties();
            this.ep = new ExcelPackage(new FileInfo(file_name));
            this.workSheet = ep.Workbook.Worksheets[workSheetNumber];
            this.insertProcedureParams = insertProcedureParams;
            this.excelFileName = excelFileName;
            this.real_user_id = real_user_id;
            this.insertGuid = insertGuid;
            this.reportNameForUser = reportNameForUser;
        }
        //public Func<IEnumerable<T>, string> ProcedureExceptionResult { get; set; }
        //public Func<IEnumerable<T>> ProcedureInsert { get; set; }
        //public Func<IEnumerable<T>> ProcedureResult{get;set;}
        private ExcelPackage ep { get; set; }
        public string reportNameForUser { get; set; }
        public string insertGuid { get; set; }
        public string error { get; set; }
        public int loading_type_id { get; set; }
       // public string description { get; set; }
        public string summary { get; set; }
        public int user_id { get; set; }
        public string stored_procedure_insert { get; set; }
        public string stored_procedure_select { get; set; }

        public string file_name { get; set; }
        int loading_id { get; set; }
        int totalCount { get; set; }
        public int real_user_id { get; set; }

        //public int ColumnCount { get; set; }
        //public int RowCount { get; set; }
        private PortalDMTOSModel portalDMTOS { get; set; }
        private List<UI_SELECT_GRID_SETTINGSResult> grid_settings { get; set; }
        private PropertyInfo[] fieldNamesInsert { get; set; }
        public List<ProcedureParam> selectProcedureParams { get; set; }
        public List<ProcedureParam> insertProcedureParams { get; set; }        
        public int workSheetNumber { get; set; }
        private ExcelWorksheet workSheet { get; set; }
        //public int rowCountSelect { get; set; }
        //public int columnCountSelect { get; set; }
        public string excelFileName { get; set; }
        public string fileNameFromUser { get; set; }       
        Action<ExcelWorksheet, List<UI_SELECT_GRID_SETTINGSResult>> headerAction { get; set; }

        [AppAuthorizeAttribute]
        public string LoadFile() {
            List<T> l=new List<T>();
            SYS_INSERT_LOADING2Result lOADING2Result= portalDMTOS.SYS_INSERT_LOADING2(loading_type_id, reportNameForUser, insertGuid, summary, user_id).FirstOrDefault();
            loading_id = Convert.ToInt32(lOADING2Result.loading_id);

            totalCount = Convert.ToInt32(lOADING2Result.column_cnt);
            var sel = selectProcedureParams.Where(x => x.Name == "loading_id").FirstOrDefault();
            if (sel != null)
            {   sel.Value = loading_id;
                //sel.ParamType = loading_id.GetType();
            }
            var ins = insertProcedureParams.Where(x => x.Name == "loading_id").FirstOrDefault();
            if (ins != null)
            {
                ins.Value = loading_id;
                //ins.ParamType = loading_id.GetType();
            }
            // error=ProcedureExceptionResult(ProcedureResult());
            ExceptionR(workSheet);
            //APL_SELECT_PROJECT_STATES_FOR_DDResult result = new APL_SELECT_PROJECT_STATES_FOR_DDResult { id = loading_id, description = error };

            if (error != "" && error != null)
            {
                var obj3 = new { loading_id = loading_id, error = error, allow_commit = 0, excelFileName = insertGuid, fileName = reportNameForUser, description = "", loading_state_description = "", total_cnt = "", error_cnt = "", warning_cnt = "" };
                return JsonConvert.SerializeObject(obj3);
            }
                //return "{loading_id:" + Convert.ToString(loading_id) + ", error:" + "\"" + error + "\"" + ", allow_commit:0}"; }
                List<T> obj = Activator.CreateInstance<List<T>>();



            SYS_SELECT_LOADING_INFOResult loading_info=portalDMTOS.SYS_SELECT_LOADING_INFO(loading_id, user_id, real_user_id).FirstOrDefault();




            int? event_id = null;
            try
            {
                //obj.GetType().GetProperty(setting.field_description).SetValue(obj, propertyInfo.PropertyType);
                Type type = portalDMTOS.GetType();
                MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure_select).FirstOrDefault();
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object[] param = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    //param[i] = obj.GetType().GetProperty(fieldNamesThis.Wh).GetValue(obj);

                   if (parameters[i].Name == "event_id") {
                        event_id = portalDMTOS.SYS_START_EVENT(174, stored_procedure_select, "").FirstOrDefault().event_id;
                        param[i] = event_id;
                    }
                    else {
                        param[i] = selectProcedureParams.Where(x => x.Name == parameters[i].Name).FirstOrDefault().Value;
                    }
                    //PropertyInfo propertyInfoThis = selectProcedureParams.Where(x => x.Name == param[i]).FirstOrDefault();

                }
                l = (List<T>)methodInfo.Invoke(portalDMTOS, param);
                
                // try { error = Convert.ToString(l.FirstOrDefault().GetType().GetProperty("error_description").GetValue(l.FirstOrDefault())); } catch { }
                //error = Convert.ToString(error);
            }
            catch (Exception exc) { error += "__Ошибка в приложении на уровне базы данных, обратитесь к админиcтратору"; }
            //ExcelReports<T> excel = new ExcelReports<T>(l, rowCountSelect, columnCountSelect, user_id, excelFileName, stored_procedure_select, workSheetNumber, headerAction);
            //ExcelReports<T> excel = new ExcelReports<T>(l, 1, 1, user_id, excelFileName, stored_procedure_select, workSheetNumber, headerAction);
            ExcelReports<T> excel = new ExcelReports<T>(l, 1, 1, user_id, excelFileName, stored_procedure_select, workSheetNumber, headerAction);
            excel.ExcelReport(null, totalCount);
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, excelFileName);




            //if (error != "" && error != null)
            //    portalDMTOS.SYS_COMMIT_LOADING(loading_id, user_id, real_user_id);
            var obj2 = new { loading_id = loading_id, error = error==null?"":error, allow_commit = loading_info.allow_commit, excelFileName = "ImportFiles\\" + insertGuid+".xlsx",
                             fileName = reportNameForUser, description=loading_info.description, loading_state_description=loading_info.loading_state_description,
                             total_cnt =loading_info.total_cnt, error_cnt=loading_info.error_cnt, warning_cnt=loading_info.warning_cnt };
            //return"{'loading_id':" + Convert.ToString(loading_id) + ", 'error':" + "'" + error + "'" + ", 'allow_commit':"+Convert.ToString(allow_commit)+"}";
            return JsonConvert.SerializeObject(obj2);
        }
        void ExceptionR(ExcelWorksheet ew ) {
            try
            {

                bool start = false;
                int RowCount=1;
                for (int row = ew.Dimension.Start.Row; row <= ew.Dimension.End.Row & start == false; row++)
                {
                    if (!start)
                    {
                        for (int col = 1; col <= totalCount; col++)
                        {
                            if (Convert.ToString(ew.Cells[row, col].Value) != Convert.ToString(col))
                            {
                                start = false;
                                break;
                            }
                            else
                            {
                                start = true;
                                RowCount = row;
                            }
                        }
                    }
                }
                if (start == false)
                {
                    { error = "__Неверный шаблон файла! В файле не найдена, или ошибочна строка с нумерацией столбцов"; }
                }


                int ColumnCount = 1;
                RowCount++;
                bool rowFlag=true;
                while(rowFlag){
                    rowFlag = false;
                    while (ColumnCount<=totalCount)
                    { if (Convert.ToString(ew.Cells[RowCount, ColumnCount].Value) != "")
                            rowFlag = true;
                        ColumnCount++;
                    }
                    ColumnCount = 1;
                    if (!rowFlag)
                        break;
                    T obj = Activator.CreateInstance<T>();
                    while (ColumnCount <= totalCount)
                    {
                        if (rowFlag)
                        {
                            UI_SELECT_GRID_SETTINGSResult setting = grid_settings.Where(x => x.number == ColumnCount).FirstOrDefault();
                            PropertyInfo propertyInfo = fieldNamesInsert.Where(x => x.Name == setting.field_description).FirstOrDefault();
                            if (setting != null && Convert.ToBoolean(setting.global_editable))
                            {
                                try
                                {
                                    Type type = obj.GetType().GetProperty(setting.field_description).PropertyType;
                                    obj.GetType().GetProperty(setting.field_description).SetValue(obj, Convert.ChangeType(ew.Cells[RowCount, ColumnCount].Value, type));
                                }
                                catch { error += "__Файл содержит неверно заполненные поля"; rowFlag = false; break; }
                            }
                            //T obj= (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
                            //T obj=(T)Activator.CreateInstance(typeof(T), new object[] {});                            
                        }
                        ColumnCount++;
                    }
                    try
                    {
                        Type type = portalDMTOS.GetType();
                        MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure_insert).FirstOrDefault();
                        ParameterInfo[] parameters = methodInfo.GetParameters();

                        object[] param = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var par = insertProcedureParams.Where(x => x.Name == parameters[i].Name).FirstOrDefault();
                            if (par != null)
                                param[i] = insertProcedureParams.Where(x => x.Name == parameters[i].Name).FirstOrDefault().Value;
                            else
                                param[i] = obj.GetType().GetProperty(parameters[i].Name).GetValue(obj);
                        }
                        methodInfo.Invoke(portalDMTOS, param);
                    }
                    catch (Exception ex) { error = ex.ToString(); error += "__Неверный формат файла. Поля файла заполнены ошибочно."; rowFlag = false; break; }

                    RowCount++;
                    ColumnCount = 1;
                }
            }
            catch { error = "__Неверный формат файла! В файле не найдена, или не совпадает строка с нумерацией столбцов"; }
            return;
        }
    }
}
