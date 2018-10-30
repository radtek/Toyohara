//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using ToyoharaCore.Models;
//using ToyoharaCore.Attributes;
//using Newtonsoft.Json;
//using Microsoft.AspNetCore.Http;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Mvc;
//using ToyoharaCore.Models.CustomModel;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using System.IO;
//using System.Web;
//using Microsoft.AspNetCore.Hosting;




//namespace ToyoharaCore.Controllers
//{
//    public class Home2Controller : Controller
//    {

//        private IHostingEnvironment _env;
//        public Home2Controller(IHostingEnvironment env)
//        {
//            _env = env;
//        }

//        private static List<GridSettings> homeGridSettings = new List<GridSettings> {
//            new GridSettings(true,200,1,"Код","code"),
//            new GridSettings(true,200,2,"Проект","project_description"),
//            new GridSettings(true,200,3,"Наименование МТР","item_description"),
//            new GridSettings(true,200,4,"Технические<br/>характеристики","item_additional_properties"),
//            new GridSettings(true,200,5,"Код СЗС","order_code"),
//            new GridSettings(true,200,6, "Номер СЗС","order_number"),
//            new GridSettings(true,200,7, "Ответсвенный за закупку","supply_manager"),
//            new GridSettings(true,200,8, "Примечание","order_item_note"),
//            new GridSettings(true,200,9, "Дата последнего<br/>изменения позиции","order_item_modification_date"),
//            new GridSettings(true,200,10,"Код ГК","gki_code"),
//            new GridSettings(true,200,11, "Дата установки кода ГК", "gki_code_date"),
//            new GridSettings(true,200,12,"Номер заявки в ГК","gki_order_number"),
//            new GridSettings(true,200,13,"Дата заявки в ГК","gki_order_date"),
//            new GridSettings(true,200,14,"Статус","gki_state"),
//            new GridSettings(true,200,15,"Дата установки статуса","gki_state_date"),
//            new GridSettings(true,200,16,"Комментарий по статусу","note"),
//            new GridSettings(true,200,17,"Эксперт НСИ","gki_user")
//        };

//        [AppAuthorizeAttribute]
//        public async Task<IActionResult> Index(bool? showSelected)
//        {
//            var webRoot = _env.WebRootPath;
//            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
//            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

//            SYS_AUTHORIZE_USER2Result au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
//            // HttpContext.Response.Cookies.Append("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au));//, new CookieOptions() { HttpOnly = false });

//            //SYS_SELECT_DELEGATING_USERSMultipleResult firstResult= portalDMTOS.SYS_SELECT_DELEGATING_USERS(807, 478).SYS_SELECT_DELEGATING_USERSResults2;
//            //SYS_SELECT_DELEGATING_USERSMultipleResult secondResult = portalDMTOS.SYS_SELECT_DELEGATING_USERS(807, 807);

//            List<SYS_SELECT_DELEGATING_USERSResult> first = portalDMTOS.SYS_SELECT_DELEGATING_USERS(au.id, au.delegating_user_id);

//            List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
//            List<UI_SELECT_GRID_AUTO_SETTINGSResult> grid_auto_settings = portalDMTOS.UI_SELECT_GRID_AUTO_SETTINGS(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();

//            Settings settings = new Settings();

//            for (int i = 0; i < homeGridSettings.Count; i++)
//            {
//                string field_description = homeGridSettings[i].ColumnName;
//                UI_SELECT_GRID_SETTINGS2Result LINQ_UI = grid_settings.Where(o => o.field_description == field_description).FirstOrDefault();

//                bool? is_visible = homeGridSettings[i].ColumnVisible;
//                int? column_width = homeGridSettings[i].ColumnWidth;
//                int? column_position = homeGridSettings[i].СolumnPosition;

//                if (LINQ_UI != null)
//                {
//                    if (LINQ_UI.width != null) column_width = LINQ_UI.width.Value;
//                    if (LINQ_UI.number != null) column_position = LINQ_UI.number.Value;
//                    if (!LINQ_UI.is_visible.Value) is_visible = false;

//                }
//                //settings.gridSettings.Add(new GridSettings { ColumnName = field_description, СolumnPosition = column_position, ColumnRussianName = homeGridSettings[i].ColumnRussianName, ColumnVisible = is_visible, ColumnWidth = column_width });

//                //settings.gridSettings = JsonConvert.DeserializeObject<List<GridSettings>>(settings.jsonGridSettings);
//                ViewData["CK_UI_" + field_description] = is_visible;
//                ViewData["CK_UI_" + field_description + "_width"] = column_width;
//                ViewData["CK_UI_" + field_description + "_ru"] = homeGridSettings[i].ColumnRussianName;
//                ViewData["CK_UI_" + field_description + "_pos"] = column_position;


//                UI_SELECT_GRID_AUTO_SETTINGSResult LINQ_UI_auto = grid_auto_settings.Where(o => o.field_description == field_description).FirstOrDefault();
//                ViewData["CK_UI_" + field_description + "_edit"] = false;
//                if (LINQ_UI_auto != null)
//                {
//                    ViewData["CK_UI_" + field_description + "_edit"] = true;
//                    if (!LINQ_UI_auto.editable.Value) ViewData["CK_UI_" + field_description + "_edit"] = false;
//                }


//            }
//            settings.actionName = "UpdateSettingsOfGrid";
//            settings.checkBoxClass = "UserSettingsCheckbox";
//            settings.controllerName = "Common";
//            settings.flowWindowName = "UserSettings";
//            settings.storedProcedure = "PRC_SELECT_ORDER_ITEMS_GKI";
//            settings.widthClass = "UserSettingsWidth";
//            settings.positionClass = "PositionClass";

//            ViewBag.Settings = settings;

//            ViewData["Columns_Array"] = homeGridSettings;
//            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dd = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI_STATES_FOR_DD();
//            ViewBag.SYS_SELECT_ROLES_FOR_DD = portalDMTOS.SYS_SELECT_ROLES_FOR_DD(au.delegating_user_id);

//            //ViewBag.FAQ =portalDMTOS.UI_SELECT_LINK_PA// portalDMTOS.UI_SELECT_LINK_PAGE_NOTE(this.ToString(), "Index", au.delegating_user_id).FirstOrDefault().http_text;
//            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));

//            ViewBag.FAQ = portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, au.delegating_user_id, au.id);

//            //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
//            //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));

//            ViewBag.control = this.ToString();
//            ViewBag.view = "Index";

//            //List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dd=portalDMTOS.PRC
//            ViewBag.StateDropdown = JsonConvert.SerializeObject(dd);

//            //ViewBag.FAQ = "<bold>Справка!</bold>";
//            return View();
//            //redi
//        }

//        [HttpGet]
//        [AppAuthorizeAttribute]
//        public object Get(DataSourceLoadOptions loadOptions, string test, bool? showSelected, string selectedRecord, bool? only_new, 
//                          bool? show_classified, bool rebind, string filters, string sorts)
//        {            
//            SYS_AUTHORIZE_USER2Result au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
//            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
//            int? skip = loadOptions.Skip;
//            List<PRC_SELECT_ORDER_ITEMS_GKI_with_pagingResult> proc_paging = 
//            portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI_with_paging(ref skip, loadOptions.Take, sorts, selectedRecord, filters, show_classified, only_new, au.delegating_user_id);

//            loadOptions.RequireTotalCount = true;
//            loadOptions.Skip = 0;
//            var d = DataSourceLoader.Load(proc_paging, loadOptions);   
//            if(proc_paging.FirstOrDefault()!=null && proc_paging.FirstOrDefault().row_count!=null)
//                d.totalCount = Convert.ToInt32(proc_paging.FirstOrDefault().row_count);
//           //string doc = "";
//           //if (filters!=null)
//           //doc = JsonConvert.DeserializeXmlNode(filters).InnerText;            

//            return d;//DataSourceLoader.Load(proc_paging, loadOptions);//d;
//        }


//        [HttpPost]
//        [AppAuthorizeAttribute]
//        public string ExcelReport(DataSourceLoadOptions loadOptions, string test, bool? showSelected, string selectedRecord, bool? only_new, bool? show_classified)
//        {
//            //DataSourceLoadOptions loadOptions = new DataSourceLoadOptions();
//            // only_new = only_new == null ? true : only_new;
//            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
//            SYS_AUTHORIZE_USER2Result au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));

//            //var x = gridSettings(portalDMTOS, false,only_new, au.delegating_user_id);
//            List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
//            if (HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
//                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")
//                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")
//                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")) == show_classified
//                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")) == only_new)
//            {
//                x = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult"));
//            }
//            else
//            {
//                int? event_id = null;
//                x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, au.delegating_user_id, event_id).ToList();
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
//            }

//            int[] selectedRecordMass = null;
//            if (selectedRecord != null && selectedRecord != "")
//                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
//            if (Convert.ToBoolean(showSelected))
//                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
//            //var m = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(false, true, au.delegating_user_id).ToList();
//            //var d = DataSourceLoader.Load(portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(false, true, au.delegating_user_id).ToList(), loadOptions).data;

//            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
//            string j = JsonConvert.SerializeObject(loadrResults.data);
//            List<PRC_SELECT_ORDER_ITEMS_GKIResult> list = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(j);

//            string templatePath = Path.Combine("C:\\ToyoharaSVN\\Разработка\\ToyoharaCore\\ToyoharaCore\\AppData\\Templates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
//            Guid guid = Guid.NewGuid();
//            string physicalPath = Path.Combine("C:\\ToyoharaSVN\\Разработка\\ToyoharaCore\\ToyoharaCore\\AppData\\Templates\\", guid + ".xlsx");
//            System.IO.File.Copy(templatePath, physicalPath);

//           // ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x, homeGridSettings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI");
//          //  excel.ExcelReport();
//            //using (ExcelPackage ep = new ExcelPackage(new FileInfo(physicalPath)))
//            //{
//            //    ExcelWorksheet ew1 = ep.Workbook.Worksheets[0];//.Add("First");

//            //    //for (int i = 0, k = 1; i < list.Count; i++, k = 1)
//            //    //{
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].code, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].project_description, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].item_description, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].item_additional_properties, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, Convert.ToString(list[i].order_code), "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].order_number, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].supply_manager, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].order_item_note, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, Convert.ToString(list[i].order_item_modification_date), "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].gki_code, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, Convert.ToString(list[i].gki_code_date), "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].gki_order_number, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, Convert.ToString(list[i].gki_order_date), "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].gki_state, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, Convert.ToString(list[i].gki_state_date), "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].note, "center");
//            //    //    SetValueInCellExport(ew1, "html", i + 2, ref k, list[i].gki_user, "center");
//            //    //}
//            //    // ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> ex = new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(true, false, au.delegating_user_id), homeGridSettings,ew1,"proc_name", new PRC_SELECT_ORDER_ITEMS_GKIResult());
//            //    // ex.ExcelReport();
//            //    ep.Save();
//            //    ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x, homeGridSettings, ew1, "PRC_SELECT_ORDER_ITEMS_GKI");
//            //    excel.ExcelReport();

//            //}
//            // var result = new FileContentResult(physicalPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");


//            return guid + ".xlsx";
//        }

//        [HttpPut]
//        [AppAuthorizeAttribute]
//        public IActionResult Put(int key, string values, bool? show_classified, bool? only_new)
//        {
//            SYS_AUTHORIZE_USER2Result au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));


//            //var employee = _data.Employees.First(a => a.ID == key);
//            //JsonConvert.PopulateObject(values, employee);

//            // JsonConvert.PopulateObject(values, d);
//            // if (!TryValidateModel(employee))
//            //    return BadRequest(ModelState.GetFullErrorMessage());

//            // _data.SaveChanges();

//            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

//            List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
//            if (HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
//                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")
//                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")
//                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")) == show_classified
//                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")) == only_new)
//            {
//                x = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult"));
//            }
//            else
//            {
//                int? event_id = null;
//                x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, au.delegating_user_id, event_id).ToList();
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
//                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
//            }

//            PRC_SELECT_ORDER_ITEMS_GKIResult x2 = x.Where(y => y.id == key).FirstOrDefault();


//            //PRC_SELECT_ORDER_ITEMS_GKIResult x = gridSettings(portalDMTOS, false, true, au.delegating_user_id).Where(y => y.id == key).FirstOrDefault();
//            JsonConvert.PopulateObject(values, x2);

//            //HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));

//            portalDMTOS.PRC_UPDATE_ORDER_ITEM_GKI(key, x2.gki_code, x2.gki_state_id, x2.gki_order_number, x2.note, au.delegating_user_id, au.id);
//            //return BadRequest("fasfdaf");

//            return Ok();
//        }

//        [HttpGet]
//        [AppAuthorizeAttribute]
//        public PhysicalFileResult ReturnFile(string physicalPath)
//        {
//            if (System.IO.File.Exists("C:\\ToyoharaSVN\\Разработка\\ToyoharaCore\\ToyoharaCore\\AppData\\Templates\\" + physicalPath))
//                return PhysicalFile("C:\\ToyoharaSVN\\Разработка\\ToyoharaCore\\ToyoharaCore\\AppData\\Templates\\" + physicalPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Excel.xlsx");

//            // return File(physicalPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
//            return null;
//        }
//        public static void SetValueInCellExport(ExcelWorksheet ew1, string type, int rowCount, ref int columnCount, string value, string position)
//        {
//            if (type == "html")
//                ew1.Cells[rowCount, columnCount].Value = CommonMethods.HtmlToText(value);
//            if (type == "string")
//                ew1.Cells[rowCount, columnCount].Value = (CommonMethods.ObjectToString(value));
//            if (type == "double")
//                ew1.Cells[rowCount, columnCount].Value = double.Parse(CommonMethods.HtmlToText(CommonMethods.ObjectToString(value)).Replace(".", ","));

//            ew1.Cells[rowCount, columnCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//            ew1.Cells[rowCount, columnCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
//            ew1.Cells[rowCount, columnCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
//            ew1.Cells[rowCount, columnCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
//            ew1.Cells[rowCount, columnCount].Style.WrapText = true;
//            ew1.Cells[rowCount, columnCount].Style.Font.Name = "Calibri";
//            ew1.Cells[rowCount, columnCount].Style.Font.Size = 8;
//            if (position == "center")
//                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            if (position == "left")
//                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
//            if (position == "right")
//                ew1.Cells[rowCount, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

//            ew1.Cells[rowCount, columnCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            columnCount++;
//        }
//        public static string CommaFind(string str)
//        {
//            string str1 = "#,##0.";
//            if (str.IndexOf(",") > -1)
//            {
//                foreach (char n in str.Substring(str.IndexOf(",") + 1))
//                { str1 = str1 + "0"; }
//            }
//            else
//                str1 = "#,##0";
//            return str1;
//        }


//    }
//}
