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
using System.Reflection;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyoharaCore.Controllers
{
    public class CommonController : Controller
    {
        private IHostingEnvironment _env;
        public CommonController(IHostingEnvironment env)
        {
            _env = env;
        }

        [AppAuthorizeAttribute]
        [HttpPost]
        public void UpdateSettingsOfGrid(string column_names, string columns_is_visible, string columns_width, string procedure_name,
                                         string columns_position, string defaultSettings)
        {
            List<UI_SELECT_GRID_SETTINGSResult> settings = JsonConvert.DeserializeObject<List<UI_SELECT_GRID_SETTINGSResult>>(defaultSettings);
            PortalDMTOSModel entities = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            if (au == null) return;
            string[] ColumnNames = column_names.Split(',');
            string[] ColumnIsVisible = columns_is_visible.Split(',');
            string[] ColumnPosition = columns_position.Split(',');
            string[] ColumnWidth = columns_width.Split(',');

            for (int i = 0; i < ColumnNames.Length; i++)
            {   
                int is_visible = (bool.Parse(ColumnIsVisible[i]) == false) ? 0 : 1;
                int width = (ColumnWidth[i] == "" || ColumnWidth[i] == null) ? Convert.ToInt32(settings[i].width) : int.Parse(ColumnWidth[i]);
                int position = (ColumnPosition[i] == "" || ColumnPosition[i] == null) ? Convert.ToInt32(settings[i].number) : int.Parse(ColumnPosition[i]);
                var grid_settings = settings.Where(x => x.field_description == ColumnNames[i]).FirstOrDefault();
                if (grid_settings.ui_type == "dropdown_list_id" | grid_settings.ui_type == "dropdown_id" | grid_settings.ui_type == "dropdown_txt_id" | grid_settings.ui_type == "dropdown_txt_list_id" |
                    grid_settings.ui_type == "dropdown_list_description" | grid_settings.ui_type == "dropdown_description" | grid_settings.ui_type == "dropdown_txt_description" | grid_settings.ui_type == "dropdown_txt_list_description"
                    )
                {
                    var dropdown_or_description = settings.Where(x => x.dropdown == grid_settings.dropdown & x.field_description != grid_settings.field_description).FirstOrDefault();
                    entities.UI_UPDATE_GRID_SETTING2(delegated_user.id, procedure_name, dropdown_or_description.field_description, is_visible, width, position);
                }

                entities.UI_UPDATE_GRID_SETTING2(delegated_user.id, procedure_name, ColumnNames[i], is_visible, width, position);
            }
            return;
        }
        [AppAuthorizeAttribute]
        [HttpPost]
        public string UpdateFAQAdd(string role_id_list, string http_text, int? id, string FAQ_header, int? order_number, int? info_id)
        {
            //UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            portalDMTOS.UI_UPDATE_LINK_PAGE_NOTE2(id, info_id, FAQ_header, http_text, role_id_list, delegated_user.id, au.id, order_number);
            return portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(info_id, delegated_user.id, au.id).FirstOrDefault().http_text;
        }
        public string DropDownCard(string grid_settings, string param, string param_name = null)
        {
            List<UI_SELECT_GRID_SETTINGSResult> settings = new List<UI_SELECT_GRID_SETTINGSResult>();
            if (grid_settings != null & grid_settings != "")
                settings = JsonConvert.DeserializeObject<List<UI_SELECT_GRID_SETTINGSResult>>(grid_settings);
            //string[] dropdownsJSON=JsonConvert.DeserializeObject<string[]>(grid_settings);
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            var dropdowns_id = settings.Where(x => (x.ui_type == "dropdown_id" | x.ui_type == "dropdown_list_id") & (x.dropdown_param == param_name)).ToList();
            var dropdowns_txt = settings.Where(x => x.ui_type == "dropdown_txt_id").ToList();
            //string[] string_mass = new string[grid_settings.Count()];
            Dictionary<string, string> dropdowns = new Dictionary<string, string>();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            for (int i = 0; i < dropdowns_id.Count(); i++)
            {
                List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN(dropdowns_id[i].dropdown, param, delegated_user.id, au.id).ToList();
                string s = "<option value='0'></option>";
                foreach (APL_SELECT_PROJECT_STATES_FOR_DDResult dd in dropdown_info)
                {
                    s = s + "<option value='" + dd.id + "'>" + dd.description + "</option>";
                }
                dropdowns.Add(dropdowns_id[i].field_description, s);
                //string_mass[i] = s;                
            }
            for (int i = 0; i < dropdowns_txt.Count(); i++)
            {
                List<UI_SELECT_DROPDOWN_TXTResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN_TXT(dropdowns_txt[i].dropdown, param, delegated_user.id, au.id).ToList();
                string s = "<option value='0'></option>";
                foreach (UI_SELECT_DROPDOWN_TXTResult dd in dropdown_info)
                {
                    s = s + "<option value='" + dd.guid + "'>" + dd.description + "</option>";
                }
                dropdowns.Add(dropdowns_txt[i].field_description, s);
            }
            return JsonConvert.SerializeObject(dropdowns.ToList());
        }
        [AppAuthorizeAttribute]
        [HttpGet]
        public object GetFAQ(DataSourceLoadOptions loadOptions, int user_id)
        {
            UI_SELECT_LINKResult link_info;

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel(); try
            {
                link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));

                //ViewBag.FAQ = portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id);

                //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
                //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));

                //  return DataSourceLoader.Load(portalDMTOS.UI_SELECT_LINK_PAGE_NOTES(null,control, view, user_id).ToList(), loadOptions);//d;
            }
            catch { return BadRequest("Сессия оборвалась. Перезагрузите страницу"); }
            List<UI_SELECT_LINK_PAGE_NOTES2Result> list = portalDMTOS.UI_SELECT_LINK_PAGE_NOTES2(link_info.id, user_id).ToList();
            return DataSourceLoader.Load(list, loadOptions);

        }
        [AppAuthorizeAttribute]
        [HttpPost]
        public void UploadFileAccept(int? loading_id)
        {
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            try
            {   
                portalDMTOS.SYS_COMMIT_LOADING(loading_id, delegated_user.id, au.id);
            }
            catch (Exception ex) { string exception = ex.ToString(); }
        }

        [AppAuthorize]
        [HttpPost]
        public void DeleteFAQ(string FAQrecords)
        {
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            while (FAQrecords[0] == ',')
                FAQrecords = FAQrecords.Substring(1);
            foreach (int id in FAQrecords.Split(',').Select(x => Int32.Parse(x)))
                portalDMTOS.UI_DELETE_LINK_PAGE(id, delegated_user.id);
        }

        [AppAuthorizeAttribute]
        [HttpGet]
        public object SelectLinkData(DataSourceLoadOptions loadOptions)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            return DataSourceLoader.Load(portalDMTOS.UI_SELECT_LINK_DATA(link_info.id).ToList(), loadOptions);
        }

        [AppAuthorizeAttribute]
        [HttpGet]
        public object SelectLinkActions(DataSourceLoadOptions loadOptions)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            return DataSourceLoader.Load(portalDMTOS.UI_SELECT_LINK_ACTIONS(link_info.id).ToList(), loadOptions);
        }

        [AppAuthorizeAttribute]
        [HttpPost]
        public void DefaultSettingsOfGrid(string procedure_name)
        {
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            portalDMTOS.UI_DEFOLT_GRID_SETTINGS(delegated_user.id, procedure_name);
        }

        [AppAuthorizeAttribute]
        public PhysicalFileResult ReturnFile(string physicalPath, string fileDownloadName="Excel")
        {
            if (System.IO.File.Exists(_env.ContentRootPath + "\\wwwroot\\AppData\\" + physicalPath ))
                return PhysicalFile(_env.ContentRootPath + "\\wwwroot\\AppData\\" + physicalPath , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",(HttpUtility.UrlDecode(fileDownloadName).Replace(":"," "))+".xlsx");
            return null;
        }

        [HttpPost]
        public ActionResult Upload(string formName){
            var newFile = Request.Form.Files[formName];
            var targetLocation = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Uploads\\", newFile.FileName);
            try { }
            catch { return BadRequest("Сервер вернул ошибку, обратитесь в техническую поддержку"); }
            return Ok();
       }
        [AppAuthorizeAttribute]
        [HttpPost]
        public ActionResult UploadFile(string insertProc, string selectProc, string insertTemplate, 
            //string selectTemplate, 
            string insertProcParams,
                                       string selectProcParams, int? loading_type_id, string descripton, string summary, int columnCountSelect, int rowCountSelect,
                                       int columnCount, int rowCount, int workSheetNumber, string fileName ) {


            //try
           // {
                dynamic insertProcedureParams = JsonConvert.DeserializeObject(insertProcParams);
                List<ProcedureParam> insertParam = new List<ProcedureParam>();
                foreach (var a in insertProcedureParams)
                {
                    Type t = null;
                    if (a.Value.Value != null)
                        t = (a.Value.Value.GetType());
                    if (Convert.ToString(t) == "System.Int64")
                        insertParam.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                    else
                        insertParam.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
                }

                dynamic selectProcedureParams = JsonConvert.DeserializeObject(insertProcParams);
                List<ProcedureParam> selectParam = new List<ProcedureParam>();
                foreach (var a in selectProcedureParams)
                {
                    Type t = null;
                    if (a.Value.Value != null)
                        t = (a.Value.Value.GetType());
                    if (Convert.ToString(t) == "System.Int64")
                        selectParam.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                    else
                        selectParam.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
                }

                //string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", insertTemplate + ".xlsx");
                Guid guid = Guid.NewGuid();
                string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ImportFiles", guid + ".xlsx");
                string NameUserFile = "Excel.xlsx";
                try
                {
                    var myFile = Request.Form.Files[fileName];
                    NameUserFile = Path.GetFileName(myFile.FileName);
                    using (var fileStream = System.IO.File.Create(physicalPath))
                    {
                        myFile.CopyTo(fileStream);
                    }
                }
                //catch { Response.StatusCode = 400; }
                catch
                {
                    HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Вы можете загрузить только Excel файлы с расширением xlsx." }));
                    return new EmptyResult();
                };

                if (Path.GetExtension(NameUserFile) != ".xlsx")
                {
                    HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Вы можете загрузить только Excel файлы с расширением xlsx." }));
                    return new EmptyResult();
                }
                NameUserFile = Path.GetFileNameWithoutExtension(NameUserFile);

            //System.IO.File.Copy(templatePath, physicalPath);

            //string templateExcelPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", selectTemplate + ".xlsx");
            string templateExcelPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", insertTemplate + ".xlsx");
            Guid guid2 = Guid.NewGuid();
                string physicalPath2 = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ImportFiles", guid2 + ".xlsx");
                System.IO.File.Copy(templateExcelPath, physicalPath2);

                SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
                APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            //CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult> fileLoader =
            //new CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult>(
            //Convert.ToInt32(loading_type_id), physicalPath, summary, delegated_user.id, selectProc, columnCountSelect, rowCountSelect,
            //columnCount, rowCount, insertProc,
            //selectParam,
            //insertParam,
            //workSheetNumber, physicalPath2, guid2.ToString(), NameUserFile, delegated_user.id);
            //string result = fileLoader.LoadFile();

            ////string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER" + ".xlsx");
            ////Guid guid = Guid.NewGuid();
            ////string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            ////System.IO.File.Copy(templatePath, physicalPath);

            ////string templateExcelPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            ////Guid guid2 = Guid.NewGuid();
            ////string physicalPath2 = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid2 + ".xlsx");
            ////System.IO.File.Copy(templateExcelPath, physicalPath2);

            ////CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult> fileLoader =
            ////new CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult>(30, "Пр", physicalPath, "", 807, "APL_SELECT_PROJECT_LOADING_ITEMS", 1, 1,
            ////1, 2, "APL_INSERT_PROJECT_LOADING_ITEM",
            ////new List<ProcedureParam> {
            ////    new ProcedureParam { Name = "loading_id", ParamType = (10).GetType(), Value = "" },
            ////    new ProcedureParam { Name = "user_id", ParamType = (1).GetType(), Value = 807 }
            ////},
            ////new List<ProcedureParam> {
            ////    new ProcedureParam { Name = "loading_id", ParamType = (10).GetType(), Value = "" },
            ////    new ProcedureParam { Name = "object_id", ParamType = null, Value = null },
            ////    new ProcedureParam { Name = "user_id", ParamType = (1).GetType(), Value = 807 },
            ////    new ProcedureParam { Name = "real_user_id", ParamType = (1).GetType(), Value = 807 }
            ////},
            ////0, physicalPath2);
            ////fileLoader.LoadFile();
            ////SYS_AUTHORIZE_USERResult result=new SYS_AUTHORIZE_USERResult { }
            //HttpContext.Response.WriteAsync(result);


            // }
            //catch(Exception ex) { //HttpContext.Response.WriteAsync(ex.ToString());
            // var obj3 = new { error = ex.ToString()};
            //    HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(obj3));

            //}

            if (selectProc == "APL_SELECT_PROJECT_LOADING_ITEMS")
            {
                CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult> fileLoader =
                            new CommonFileLoader<APL_SELECT_PROJECT_LOADING_ITEMSResult>(
                            Convert.ToInt32(loading_type_id), physicalPath, summary, delegated_user.id, selectProc, 
                            //columnCountSelect, rowCountSelect,  columnCount, rowCount, 
                            insertProc,
                            selectParam,
                            insertParam,
                            workSheetNumber, physicalPath2, guid2.ToString(), NameUserFile, delegated_user.id);

                string result = fileLoader.LoadFile();
                HttpContext.Response.WriteAsync(result);
            }

            if (selectProc == "OMC_SELECT_SVR_LOADING_ITEM")
            {
                CommonFileLoader<OMC_SELECT_SVR_LOADING_ITEMResult> fileLoader =
                            new CommonFileLoader<OMC_SELECT_SVR_LOADING_ITEMResult>(
                            Convert.ToInt32(loading_type_id), physicalPath, summary, delegated_user.id, selectProc, 
                            //columnCountSelect, rowCountSelect, columnCount, rowCount, 
                            insertProc,
                            selectParam,
                            insertParam,
                            workSheetNumber, physicalPath2, guid2.ToString(), NameUserFile, delegated_user.id);

                string result = fileLoader.LoadFile();
                HttpContext.Response.WriteAsync(result);
            }


            if (selectProc == "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMS")
            {
                CommonFileLoader<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMSResult> fileLoader =
                            new CommonFileLoader<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMSResult>(
                            Convert.ToInt32(loading_type_id), physicalPath, summary, delegated_user.id, selectProc, 
                            //columnCountSelect, rowCountSelect, columnCount, rowCount, 
                            insertProc,
                            selectParam,
                            insertParam,
                            workSheetNumber, physicalPath2, guid2.ToString(), NameUserFile, delegated_user.id);

                string result = fileLoader.LoadFile();
                HttpContext.Response.WriteAsync(result);
            }


            if (selectProc == "APL_SELECT_PROJECT_REQUIREMENT_LOADING_ITEMS2")
            {
                CommonFileLoader<APL_SELECT_PROJECT_REQUIREMENT_LOADING_ITEMS2Result> fileLoader =
                            new CommonFileLoader<APL_SELECT_PROJECT_REQUIREMENT_LOADING_ITEMS2Result>(
                            Convert.ToInt32(loading_type_id), physicalPath, summary, delegated_user.id, selectProc,
                            //columnCountSelect, rowCountSelect, columnCount, rowCount, 
                            insertProc,
                            selectParam,
                            insertParam,
                            workSheetNumber, physicalPath2, guid2.ToString(), NameUserFile, delegated_user.id);

                string result = fileLoader.LoadFile();
                HttpContext.Response.WriteAsync(result);
            }



            return new EmptyResult();
        }

        [HttpGet]
        [AppAuthorizeAttribute]
        public object GetData(DataSourceLoadOptions loadOptions, string additionalParams)
        {
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            dynamic procedureParams = JsonConvert.DeserializeObject(additionalParams);
            List<ProcedureParam> procParams = new List<ProcedureParam>();
            foreach (var a in procedureParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }
            string stored_procedure = Convert.ToString(procParams.Where(x => x.Name == "storedProcedure").FirstOrDefault().Value);
            string selectedRecord = Convert.ToString((procParams.Where(x => x.Name == "selectedRecord").FirstOrDefault().Value));
            bool? showSelected = Convert.ToBoolean(procParams.Where(x => x.Name == "showSelected").FirstOrDefault().Value);

            Type type = portalDMTOS.GetType();
            MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure).FirstOrDefault();
            //dynamic instance = foobar(typeof(string));
            //var listType = typeof(List<>);
            //var constructedListType = listType.MakeGenericType(methodInfo.ReturnType);
            //var instance = (IList)Activator.CreateInstance(constructedListType);
            dynamic instance = Activator.CreateInstance(methodInfo.ReturnType);

            //var obj = Activator.CreateInstance(methodInfo.ReturnType);
            //Type generic = methodInfo.ReturnType.MakeGenericType(null);


            procParams.Add(new ProcedureParam { Name = "event_id", Value = null });
            procParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });

            //List<T> obj = Activator.CreateInstance<List<T>>();
            string controller = Convert.ToString(procParams.Where(x => x.Name == "controller").FirstOrDefault().Value);
            var list = procParams.Where(x => (x.Name != "controller" && x.Name != "storedProcedure" && x.Name != "rebind" && x.Name != "selectedRecord" && x.Name != "showSelected")).ToList();
            bool rebind = Convert.ToBoolean(procParams.Where(x => x.Name == "rebind").FirstOrDefault().Value);
            //bool rebind_flag = false;
            try
            {
                //obj.GetType().GetProperty(setting.field_description).SetValue(obj, propertyInfo.PropertyType);

                //l=methodInfo.ReturnType
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object[] param = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    //param[i] = obj.GetType().GetProperty(fieldNamesThis.Wh).GetValue(obj);
                    param[i] = procParams.Where(x => x.Name == parameters[i].Name).FirstOrDefault().Value;
                    //PropertyInfo propertyInfoThis = selectProcedureParams.Where(x => x.Name == param[i]).FirstOrDefault();
                }
                bool set_params_flag = true;
                if (!rebind)
                {
                    set_params_flag = false;
                    foreach (var a in list)
                    {
                        if (HttpContext.Session.Keys.Contains(controller + stored_procedure + a.Name))
                        {   //rebind = true;
                            string str = Convert.ToString(Convert.ToString(JsonConvert.DeserializeObject<string>(Convert.ToString(HttpContext.Session.GetString(controller + stored_procedure + a.Name)))));
                            string str2 = Convert.ToString(Convert.ToString(a.Value));
                            str = str != null ? str.ToString() : "";
                            str2 = str2 != null ? str2.ToString() : "";
                            if (str.ToLower() != str2.ToLower())
                            {
                                rebind = true;
                            }
                            //object str = JsonConvert.DeserializeObject(Convert.ToString(HttpContext.Session.GetString(a.Name)));
                            //object str2 = a.Value;
                        }
                        else if (!HttpContext.Session.Keys.Contains(controller + stored_procedure + a.Name))
                            rebind = true;

                        HttpContext.Session.SetString(controller + stored_procedure + a.Name, JsonConvert.SerializeObject(a.Value));
                    }
                    set_params_flag = true;
                }
                if (rebind)
                {
                    

                       instance = methodInfo.Invoke(portalDMTOS, param);
                    string session_params = JsonConvert.SerializeObject(instance);
                    if (HttpContext.Session.Keys.Where(x => x == controller + stored_procedure).FirstOrDefault() != null)
                        HttpContext.Session.Remove(controller + stored_procedure);
                    HttpContext.Session.SetString(controller + stored_procedure, session_params);
                    if (set_params_flag)
                    {
                        foreach (var a in list)
                        {
                            if (HttpContext.Session.Keys.Where(x=>x==controller + stored_procedure + a.Name).FirstOrDefault()!=null)
                                HttpContext.Session.Remove(controller + stored_procedure + a.Name);
                            HttpContext.Session.SetString(controller + stored_procedure + a.Name, JsonConvert.SerializeObject(a.Value));
                        }
                    }
                }
                else
                {
                    instance = JsonConvert.DeserializeObject(HttpContext.Session.GetString(controller + stored_procedure));
                }
            }
            catch (Exception ex) { string exp = Convert.ToString(ex); }
            string[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',');
            if (Convert.ToBoolean(showSelected))
                instance = ((IEnumerable<dynamic>)instance).Join(selectedRecordMass, y => Convert.ToString(y.id), m => m, (y, m) => y).ToList();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            return DataSourceLoader.Load(instance, loadOptions);//d;
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public IActionResult PostData(string values, string additionalSelectParams, string additionalInsertParams, string additionalSupportParams)
        {
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            dynamic procedureSelectParams = JsonConvert.DeserializeObject(additionalSelectParams);
            dynamic procedureInsertParams = JsonConvert.DeserializeObject(additionalInsertParams);
            dynamic SupportParams = JsonConvert.DeserializeObject(additionalSupportParams);


            List<ProcedureParam> procSelectParams = new List<ProcedureParam>();
            foreach (var a in procedureSelectParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procSelectParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procSelectParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }
            List<ProcedureParam> procInsertParams = new List<ProcedureParam>();
            foreach (var a in procedureInsertParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procInsertParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procInsertParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }

            List<ProcedureParam> procSupportParams = new List<ProcedureParam>();
            foreach (var a in SupportParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procSupportParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procSupportParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }



            string stored_procedure = Convert.ToString(procSupportParams.Where(x => x.Name == "storedProcedureSelect").FirstOrDefault().Value);
            string stored_procedure_insert = Convert.ToString(procSupportParams.Where(x => x.Name == "storedProcedureInsert").FirstOrDefault().Value);
            //string stored_procedure_finish_insert = Convert.ToString(procSupportParams.Where(x => x.Name == "storedProcedureFinishInsert").FirstOrDefault().Value);
            string selectedRecord = Convert.ToString((procSupportParams.Where(x => x.Name == "selectedRecord").FirstOrDefault().Value));
            bool? showSelected = Convert.ToBoolean(procSupportParams.Where(x => x.Name == "showSelected").FirstOrDefault().Value);
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, stored_procedure, null, 1).ToList();

            Type type = portalDMTOS.GetType();
            MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure).FirstOrDefault();
            MethodInfo methodInfoInsert = type.GetMethods().Where(x => x.Name == stored_procedure_insert).FirstOrDefault();


            //dynamic instance = foobar(typeof(string));
            //var listType = typeof(List<>);
            //var constructedListType = listType.MakeGenericType(methodInfo.ReturnType);
            //var instance = (IList)Activator.CreateInstance(constructedListType);
            Type ty = methodInfo.ReturnType.GetGenericArguments().Single();
            dynamic instance = Activator.CreateInstance(methodInfo.ReturnType.GetGenericArguments().Single());

            //var obj = Activator.CreateInstance(methodInfo.ReturnType);
            //Type generic = methodInfo.ReturnType.MakeGenericType(null);


            procSelectParams.Add(new ProcedureParam { Name = "event_id", Value = null });
            procSelectParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procSelectParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });

            procInsertParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procInsertParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });
            Type instanceType = instance.GetType();
            //List<T> obj = Activator.CreateInstance<List<T>>();
            string controller = Convert.ToString(procSupportParams.Where(x => x.Name == "controller").FirstOrDefault().Value);
            var list = procSupportParams.Where(x => (x.Name != "controller" && x.Name != "storedProcedure" && x.Name != "rebind" && x.Name != "selectedRecord" && x.Name != "showSelected")).ToList();
            bool rebind = Convert.ToBoolean(procSupportParams.Where(x => x.Name == "rebind").FirstOrDefault().Value);
            JsonConvert.PopulateObject(values, instance);
            MethodInfo methodInfo2 = type.GetMethods().Where(x => x.Name == stored_procedure_insert).FirstOrDefault();
            Type typeInsert = methodInfo2.ReturnType;
            ParameterInfo[] parametersInsert = methodInfoInsert.GetParameters();
            object[] paramInsert = new object[parametersInsert.Length];
            for (int i = 0; i < parametersInsert.Length; i++)
            {
                bool nullFlag = true;
                if (instanceType.GetProperties().Any(x => x.Name == parametersInsert[i].Name))
                { paramInsert[i] = instanceType.GetProperty(parametersInsert[i].Name).GetValue(instance); nullFlag = false; }
                if (procInsertParams.Any(x => x.Name == parametersInsert[i].Name))
                { paramInsert[i] = procInsertParams.Where(x => x.Name == parametersInsert[i].Name).FirstOrDefault().Value; nullFlag = false; }
                if (nullFlag)
                    paramInsert[i] = null;
                // procParams.Where(x => x.Name == parameters2[i].Name).FirstOrDefault().Value;
                //PropertyInfo propertyInfoThis = selectProcedureParams.Where(x => x.Name == param[i]).FirstOrDefault();
            }
            var required_field = grid_settings.Where(x => x.is_required == true).ToList();
            string error = "";
            for (int i = 0; i < parametersInsert.Length; i++)
            {
                var param = grid_settings.Where(x => x.field_description == parametersInsert[i].Name).FirstOrDefault();
                if (param != null)
                {
                    bool? required = param.is_required;
                    if (Convert.ToBoolean(required))
                    {
                        if (paramInsert[i] == null | Convert.ToString(parametersInsert) == "")
                            if (param.ui_type != "dropdown_list_id" && param.ui_type != "dropdown_id" && param.ui_type != "dropdown_txt_id" && param.ui_type != "dropdown_txt_list_id")
                                error = error + " " + param.russian_field_description + " обязательно для заполенения.";
                            else
                                error = error + " " + grid_settings.Where(x => x.field_description == parametersInsert[i].Name.Replace("id", "description")).FirstOrDefault().russian_field_description+ " обязательно для заполенения.";
                    }
                }
            }
            if (error != "")
            { return BadRequest(error); }
            dynamic s = methodInfo2.Invoke(portalDMTOS, paramInsert);
            foreach (var a in s)
            {
                if (Convert.ToString(a.error_description) != "")  error = error + " " + a.error_description; 
            }
            
            if (Convert.ToString(error.Trim()) != "")
                return BadRequest(error);
            return Ok();
        }

        [HttpPut]
        [AppAuthorizeAttribute]
        public IActionResult PutData(string key, string values, string additionalSelectParams, string additionalInsertParams, string additionalSupportParams)
        {
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            dynamic procedureSelectParams = JsonConvert.DeserializeObject(additionalSelectParams);
            dynamic procedureInsertParams = JsonConvert.DeserializeObject(additionalInsertParams);
            dynamic SupportParams = JsonConvert.DeserializeObject(additionalSupportParams);
            List<ProcedureParam> procSelectParams = new List<ProcedureParam>();
            foreach (var a in procedureSelectParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procSelectParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procSelectParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }
            List<ProcedureParam> procInsertParams = new List<ProcedureParam>();
            foreach (var a in procedureInsertParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procInsertParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procInsertParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }

            List<ProcedureParam> procSupportParams = new List<ProcedureParam>();
            foreach (var a in SupportParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procSupportParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procSupportParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }
            string stored_procedure = Convert.ToString(procSupportParams.Where(x => x.Name == "storedProcedureSelect").FirstOrDefault().Value);
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, stored_procedure, null, 1).ToList();

            string stored_procedure_insert = Convert.ToString(procSupportParams.Where(x => x.Name == "storedProcedureInsert").FirstOrDefault().Value);
            string selectedRecord = Convert.ToString((procSupportParams.Where(x => x.Name == "selectedRecord").FirstOrDefault().Value));
            bool? showSelected = Convert.ToBoolean(procSupportParams.Where(x => x.Name == "showSelected").FirstOrDefault().Value);

            Type type = portalDMTOS.GetType();
            MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure).FirstOrDefault();
            MethodInfo methodInfoInsert = type.GetMethods().Where(x => x.Name == stored_procedure_insert).FirstOrDefault();

            //dynamic instance = foobar(typeof(string));
            //var listType = typeof(List<>);
            //var constructedListType = listType.MakeGenericType(methodInfo.ReturnType);
            //var instance = (IList)Activator.CreateInstance(constructedListType);
            dynamic instance = Activator.CreateInstance(methodInfo.ReturnType);


            //var obj = Activator.CreateInstance(methodInfo.ReturnType);
            //Type generic = methodInfo.ReturnType.MakeGenericType(null);


            procSelectParams.Add(new ProcedureParam { Name = "event_id", Value = null });
            procSelectParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procSelectParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });

            procInsertParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procInsertParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });

            //List<T> obj = Activator.CreateInstance<List<T>>();
            string controller = Convert.ToString(procSupportParams.Where(x => x.Name == "controller").FirstOrDefault().Value);
            var list = procSupportParams.Where(x => (x.Name != "controller" && x.Name != "storedProcedure" && x.Name != "rebind" && x.Name != "selectedRecord" && x.Name != "showSelected")).ToList();
            bool rebind = Convert.ToBoolean(procSupportParams.Where(x => x.Name == "rebind").FirstOrDefault().Value);
            //bool rebind_flag = false;
            try
            {
                //obj.GetType().GetProperty(setting.field_description).SetValue(obj, propertyInfo.PropertyType);

                //l=methodInfo.ReturnType
                ParameterInfo[] parametersSelect = methodInfo.GetParameters();
                object[] paramSelect = new object[parametersSelect.Length];
                for (int i = 0; i < parametersSelect.Length; i++)
                {
                    //param[i] = obj.GetType().GetProperty(fieldNamesThis.Wh).GetValue(obj);
                    if (procSelectParams.Where(x => x.Name == parametersSelect[i].Name).FirstOrDefault() != null)
                        paramSelect[i] = procSelectParams.Where(x => x.Name == parametersSelect[i].Name).FirstOrDefault().Value;
                    else
                        paramSelect[i] = null;

                    //PropertyInfo propertyInfoThis = selectProcedureParams.Where(x => x.Name == param[i]).FirstOrDefault();
                }
                //ParameterInfo[] parametersInserts = methodInfoInsert.GetParameters();
                //object[] paramInserts = new object[parametersInserts.Length];
                //for (int i = 0; i < parametersInserts.Length; i++)
                //{
                //    //param[i] = obj.GetType().GetProperty(fieldNamesThis.Wh).GetValue(obj);
                //    if (procSelectParams.Where(x => x.Name == parametersSelect[i].Name).FirstOrDefault() != null)
                //        paramInserts[i] = procInsertParams.Where(x => x.Name == parametersInserts[i].Name).FirstOrDefault().Value;
                //    else
                //        paramInserts[i] = null;
                //}
                bool set_params_flag = false;
                if (!rebind)
                {
                    foreach (var a in list)
                    {
                        if (HttpContext.Session.Keys.Contains(controller + stored_procedure + a.Name))
                        {   //rebind = true;
                            string str = Convert.ToString(Convert.ToString(JsonConvert.DeserializeObject<string>(Convert.ToString(HttpContext.Session.GetString(controller + stored_procedure + a.Name)))));
                            string str2 = Convert.ToString(Convert.ToString(a.Value));
                            str = str != null ? str.ToString() : "";
                            str2 = str2 != null ? str.ToString() : "";
                            if (str != str2)
                            {
                                rebind = true;
                            }
                            //object str = JsonConvert.DeserializeObject(Convert.ToString(HttpContext.Session.GetString(a.Name)));
                            //object str2 = a.Value;
                        }
                        else if (!HttpContext.Session.Keys.Contains(controller + stored_procedure + a.Name))
                            rebind = true;

                        HttpContext.Session.SetString(controller + stored_procedure + a.Name, JsonConvert.SerializeObject(a.Value));
                    }
                    set_params_flag = true;
                }
                if (rebind)
                {
                    instance = methodInfo.Invoke(portalDMTOS, paramSelect);
                    string session_params = JsonConvert.SerializeObject(instance);
                    HttpContext.Session.SetString(controller + stored_procedure, session_params);
                    if (set_params_flag)
                    {
                        foreach (var a in list)
                        {
                            HttpContext.Session.SetString(controller + stored_procedure + a.Name, JsonConvert.SerializeObject(a.Value));
                        }
                    }
                }
                else
                {
                    instance = JsonConvert.DeserializeObject(HttpContext.Session.GetString(controller + stored_procedure), methodInfo.ReturnType);
                }
            }
            catch { }
            string[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',');
            if (Convert.ToBoolean(showSelected))
                instance = ((IEnumerable<dynamic>)instance).Join(selectedRecordMass, y => Convert.ToString(y.id), m => m, (y, m) => y).ToList();
            var obj = ((IEnumerable<dynamic>)instance).Where(y => Convert.ToString(y.id) == key).FirstOrDefault();
            //var ob=JsonConvert.DeserializeObject(obj);
            JsonConvert.PopulateObject(values, obj);
            Type typeInsert = obj.GetType();
            //Type type2 = portalDMTOS.GetType();
            MethodInfo methodInfo2 = type.GetMethods().Where(x => x.Name == stored_procedure_insert).FirstOrDefault();
            //portalDMTOS.PRC_UPDATE_ORDER_ITEM_GKI(key, x2.gki_code, x2.gki_state_id, x2.gki_order_number, x2.note, au.delegating_user_id, delegated_user.id);
            ParameterInfo[] parametersInsert = methodInfo2.GetParameters();
            object[] paramInsert = new object[parametersInsert.Length];
            for (int i = 0; i < parametersInsert.Length; i++)
            {
                //param[i] = obj.GetType().GetProperty(fieldNamesThis.Wh).GetValue(obj);
                //if (typeInsert.GetProperties().Any(x => x.Name == parametersInsert[i].Name))
                bool nullFlag = true;
                if (typeInsert.GetProperties().Any(x => x.Name == parametersInsert[i].Name))
                { paramInsert[i] = typeInsert.GetProperty(parametersInsert[i].Name).GetValue(obj); nullFlag = false; }
                if (procInsertParams.Any(x => x.Name == parametersInsert[i].Name))
                { paramInsert[i] = procInsertParams.Where(x => x.Name == parametersInsert[i].Name).FirstOrDefault().Value; nullFlag = false; }
                if (nullFlag)
                    paramInsert[i] = null;
                // procParams.Where(x => x.Name == parameters2[i].Name).FirstOrDefault().Value;
                //PropertyInfo propertyInfoThis = selectProcedureParams.Where(x => x.Name == param[i]).FirstOrDefault();
            }

            var required_field = grid_settings.Where(x => x.is_required == true).ToList();
            string error = "";
            for (int i = 0; i < parametersInsert.Length; i++)
            {
                var param = grid_settings.Where(x => x.field_description == parametersInsert[i].Name).FirstOrDefault();
                if (param != null)
                {
                    bool? required = param.is_required;
                    if (Convert.ToBoolean(required))
                    {
                        if (paramInsert[i] == null | Convert.ToString(parametersInsert) == "")
                            error = error + " " + param.russian_field_description + " обязательно для заполенения.";
                    }
                }
            }
            if (error != "")
            { return BadRequest(error); }

            dynamic s = methodInfo2.Invoke(portalDMTOS, paramInsert);
            foreach (var a in s)
            {
                if (Convert.ToString(a.error_description) != "")
                    error = error + " " + a.error_description;
            }
            if (Convert.ToString(error.Trim()) != "")
                return BadRequest(error);
            return Ok();
        }
        public string InsertCard(string procedureParams)
        {
            object error = "";
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            dynamic procedureParam = JsonConvert.DeserializeObject(procedureParams);
            List<ProcedureParam> procParams = new List<ProcedureParam>();
            foreach (var a in procedureParam)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }

            // object instance="";
            string stored_procedure = Convert.ToString(procParams.Where(x => x.Name == "storedProcedure").FirstOrDefault().Value);
            object id = procParams.Where(x => x.Name == "id").FirstOrDefault().Value;
            if (id != null)
                if (Convert.ToString(id.GetType()) == "System.Int64")
                    id = Convert.ToInt32(id);
            Type type = portalDMTOS.GetType();
            MethodInfo methodInfo = type.GetMethods().Where(x => x.Name == stored_procedure).FirstOrDefault();
            dynamic instance = Activator.CreateInstance(methodInfo.ReturnType);
            procParams.Add(new ProcedureParam { Name = "user_id", Value = delegated_user.id });
            procParams.Add(new ProcedureParam { Name = "real_user_id", Value = au.id });
            try
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object[] param = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var param_i = procParams.Where(x => x.Name == parameters[i].Name).FirstOrDefault();
                    Type t = Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType;
                    object safeValue = null;
                    if (param_i != null)
                    {
                        if (t.ToString() == "System.String")
                            safeValue = (param_i.Value == null) ? "" : Convert.ChangeType(param_i.Value, t);
                        else
                        if (t.ToString() == "System.Double")
                            safeValue = (param_i.Value == null || Convert.ToString(param_i.Value) == "") ? null : Convert.ChangeType(Convert.ToString(param_i.Value).Replace(".", ","), t);
                        else
                            safeValue = (param_i.Value == null || Convert.ToString(param_i.Value) == "") ? null : Convert.ChangeType(param_i.Value, t);
                        //if (param_i!= null)                        
                        //    param[i] =Convert.ChangeType(param_i.Value, t);
                        //else
                    }
                    param[i] = safeValue;
                    //.Value;

                }
                //string str = JsonConvert.SerializeObject(param);
                instance = methodInfo.Invoke(portalDMTOS, param);
                try
                {
                    //Type t2 = instance.GetType();
                    //MethodInfo[] methodInfo2 = t2.GetMethods();//.Where(x => x.Name == "FirstOrDefault").FirstOrDefault();                
                    //var obj = methodInfo.Invoke(instance, null);
                    //var obj2=obj.GetType().

                    // error = instance.FirstOrDefault().error_description;//.GetType().GetField("error_description").GetValue(error);
                    foreach (var a in instance)
                    {
                        error = a.error_description;
                    }
                }
                catch { /*this.Server.MachineName, this.ToString() + "::Items", (new JsonResult { Data = e }).Data.ToString(), null, au.delegating_user_id);*/ }

            }
            catch { error = "Ошибка на уровне приведения типов, обратитесь к администратору."; }


            return Convert.ToString(error);
        }

        [HttpGet]
        public ActionResult GridCardPartialUpdate(int? grid_setting_param_id, string selectProc, string flowWindowName, string flowWindowRussianName, string gridId, bool binding, 
            bool close_Window, string updateProc, string additionalParams, string gridType) {
            dynamic procedureParams = (additionalParams == null | additionalParams == "") ? null : JsonConvert.DeserializeObject(additionalParams);
            List<ProcedureParam> procParams = new List<ProcedureParam>();
            if (procedureParams != null)
                foreach (var a in procedureParams)
                {
                    Type t = null;
                    if (a.Value.Value != null)
                        t = (a.Value.Value.GetType());
                    if (Convert.ToString(t) == "System.Int64")
                        procParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                    else
                        procParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
                }

            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, selectProc, grid_setting_param_id, 2).ToList();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].ui_type == "dropdown_list_id" | settings[i].ui_type == "dropdown_id" | settings[i].ui_type == "dropdown_txt_id" | settings[i].ui_type == "dropdown_txt_list_id")
                {
                    UI_SELECT_GRID_SETTINGSResult description = settings.Where(x => x.ui_type != settings[i].ui_type & x.dropdown == settings[i].dropdown).FirstOrDefault();
                    description.number = settings[i].number;
                    description.width = settings[i].width;
                    //description.russian_field_description = settings[i].russian_field_description;
                    settings[i].russian_field_description = description.russian_field_description;
                }
            }
            UpdateGridCardModel gridCardModel = new UpdateGridCardModel(
                                                        flowWindowName,
                                                        settings,
                                                        flowWindowRussianName, 
                                                        gridId, 
                                                        binding, 
                                                        updateProc, 
                                                        procParams,
                                                        gridType);
            return PartialView("UpdateGridCard", gridCardModel);
        }

        [HttpGet]
        public ActionResult GridCardTreePartialUpdate(int? param_id, string selectProc, string flowWindowName, string flowWindowRussianName, string gridId, bool binding,
        bool close_Window, string updateProc, string id_func, string additionalParams)
        {
            dynamic procedureParams = (additionalParams==null | additionalParams=="") ? null: JsonConvert.DeserializeObject(additionalParams);
            List<ProcedureParam> procParams = new List<ProcedureParam>();
            if(procedureParams!=null)
            foreach (var a in procedureParams)
            {
                Type t = null;
                if (a.Value.Value != null)
                    t = (a.Value.Value.GetType());
                if (Convert.ToString(t) == "System.Int64")
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = Convert.ToInt32(a.Value.Value) });
                else
                    procParams.Add(new ProcedureParam { Name = a.Name, Value = a.Value.Value });
            }
            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            UpdateGridCardModel gridCardModel = new UpdateGridCardModel(flowWindowName,
            portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, selectProc, param_id, 2).ToList(),
            flowWindowRussianName, gridId, binding,  updateProc, procParams, "tree");
            return PartialView("UpdateTreeGridCard", gridCardModel);
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelLog(string object_type, int object_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));


            List<SYS_SELECT_OBJECT_LOGResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "SYS_SELECT_OBJECT_LOG", "").FirstOrDefault().event_id;
            x = portalDMTOS.SYS_SELECT_OBJECT_LOG(event_id, delegated_user.id, au.id, object_type, object_id).ToList();

            string j = JsonConvert.SerializeObject(x);
            List<SYS_SELECT_OBJECT_LOGResult> list = JsonConvert.DeserializeObject<List<SYS_SELECT_OBJECT_LOGResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<SYS_SELECT_OBJECT_LOGResult> excel =
            new ExcelReports<SYS_SELECT_OBJECT_LOGResult>(list, 1, 1, delegated_user.id, physicalPath, "SYS_SELECT_OBJECT_LOG", 0, null);

            excel.ExcelReport(true);

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
  
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }












        [HttpGet]
        [AppAuthorizeAttribute]
        public IActionResult ObjectLog(string object_type, string object_id, string openParsialDivFunction)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "SYS_SELECT_OBJECT_LOG", null, 1).ToList();
            for (int i = 0; i < grid_settings.Count; i++)
            {
                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ObjLog"] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ObjLog" + "_width"] = grid_settings[i].width;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ObjLog" + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ObjLog" + "_pos"] = grid_settings[i].number;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ObjLog" + "_edit"] = grid_settings[i].global_editable;
            }

            Settings settings = new Settings();

            settings.actionName = "UpdateSettingsOfGrid";
            settings.controllerName = "Common";
            settings.checkBoxClass = "UserSettingsCheckboxObjLog";
            settings.flowWindowName = "LogUserSettings";
            settings.storedProcedure = "SYS_SELECT_OBJECT_LOG";
            settings.widthClass = "UserSettingsWidthObjLog";
            settings.positionClass = "PositionClassObjLog";
            settings.gridSettings = grid_settings;
            settings.openParsialDivFunction = openParsialDivFunction;
            settings.parsialDivName = "ObjLog";
            ViewBag.SettingsObjLog = settings;
            ViewBag.object_type = object_type;
            ViewBag.object_id = object_id;


            return PartialView("~/Views/Shared/LogObj.cshtml");
        }



        
        [HttpGet]
        [AppAuthorizeAttribute]
        public ActionResult ShowLogPerson()
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "SYS_SELECT_LOG", null, 1).ToList();



            for (int i = 0; i < grid_settings.Count; i++)
            {
                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_log_person"] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_log_person" + "_width"] = grid_settings[i].width;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_log_person" + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_log_person" + "_pos"] = grid_settings[i].number;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_log_person" + "_edit"] = grid_settings[i].global_editable;
            }

            Settings settings = new Settings();
            settings.actionName = "UpdateSettingsOfGrid";
            settings.controllerName = "Common";
            settings.checkBoxClass = "UserSettingsCheckboxLogPerson";
            settings.flowWindowName = "UserSettingsLogPerson";
            settings.storedProcedure = "SYS_SELECT_LOG";
            settings.widthClass = "UserSettingsWidthLogPerson";
            settings.positionClass = "PositionClassLogPerson";
            settings.gridSettings = grid_settings;
            settings.openParsialDivFunction = "ShowLogPerson";
            settings.parsialDivName = "logPerson";
            ViewBag.SettingsLogPerson = settings;

            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN("USER_LOG", null, delegated_user.id, au.id).ToList();
            ViewBag.UsersValues = JsonConvert.SerializeObject(dropdown_info);
            ViewBag.RealUserId = delegated_user.id;

            return PartialView("~/Views/Shared/LogPerson.cshtml");
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelLogPerson(DataSourceLoadOptions loadOptions, DateTime start_time, DateTime finish_time, int? real_user_id)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<SYS_SELECT_LOGResult> x = null;
            event_id = portalDMTOS.SYS_START_EVENT(real_user_id, "SYS_SELECT_LOG", null).FirstOrDefault().event_id;
            x = portalDMTOS.SYS_SELECT_LOG(delegated_user.id, start_time, finish_time).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<SYS_SELECT_LOGResult> list = JsonConvert.DeserializeObject<List<SYS_SELECT_LOGResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            ExcelReports<SYS_SELECT_LOGResult> excel = new ExcelReports<SYS_SELECT_LOGResult>(list, 1, 1, delegated_user.id, physicalPath, "SYS_SELECT_LOG", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }



    }
}
