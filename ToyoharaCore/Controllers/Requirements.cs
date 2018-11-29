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


namespace ToyoharaCore.Controllers
{
    public class Requirements : Controller
    {

        private IHostingEnvironment _env;
        public Requirements(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public string SendRSSToDMTOS(int? project_id) {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error=portalDMTOS.APL_SEND_RSS_TO_DMTOS(project_id, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public string SetState9(int? project_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_UPDATE_PROJECT_REQUIREMENT_STATE_9_2(project_id, 9 , delegated_user.id, au.id).FirstOrDefault().error;
            return error;
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public string CancelState9(int? project_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_UPDATE_PROJECT_REQUIREMENT_STATE_9_2(project_id, -9 , delegated_user.id, au.id).FirstOrDefault().error;
            return error;
        }

        //===============Index==================


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> Index(bool? showSelected)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECTS_FOR_REQUIREMENTS", null, 1).ToList();
            Settings settings = new Settings();


            ViewBag.SendRSSToDMTOSBtn = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Index", "SendRSSToDMTOS", delegated_user.id, null).FirstOrDefault().column0;
            ViewBag.SetState9Btn = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Index", "SetState9", delegated_user.id, null).FirstOrDefault().column0;
            ViewBag.CancelState9Btn = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Index", "CancelState9", delegated_user.id, null).FirstOrDefault().column0;
            ViewBag.SetColorBtn = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Index", "SetColor", delegated_user.id, null).FirstOrDefault().column0;

            for (int i = 0; i < grid_settings.Count; i++)
            {

                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;

                ViewData["CK_UI_" + grid_settings[i].field_description] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_width"] = grid_settings[i].width;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_pos"] = grid_settings[i].number;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_edit"] = grid_settings[i].global_editable;

            }
            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "APL_SELECT_PROJECTS_FOR_REQUIREMENTS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;
            
            return View();
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, bool? hide_closed, bool? show_mine)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("Requirements"+"APL_SELECT_PROJECTS_FOR_REQUIREMENTS"))
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult>>(HttpContext.Session.GetString("Requirements" + "APL_SELECT_PROJECTS_FOR_REQUIREMENTS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECTS_FOR_REQUIREMENTS", "").FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_PROJECTS_FOR_REQUIREMENTS(event_id, delegated_user.id, au.id, hide_closed, show_mine).ToList();
                HttpContext.Session.SetString("Requirements" + "APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult> excel =
            new ExcelReports<APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult>(list, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECTS_FOR_REQUIREMENTS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }







        //===============Items==================







        








        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> Items(int? project_id, string link_information_param)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENTS", project_id, 1).ToList();
            Settings settings = new Settings();


            for (int i = 0; i < grid_settings.Count; i++)
            {

                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;

                ViewData["CK_UI_" + grid_settings[i].field_description] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_width"] = grid_settings[i].width;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_pos"] = grid_settings[i].number;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_edit"] = grid_settings[i].global_editable;

            }
            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "APL_SELECT_PROJECT_REQUIREMENTS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;
            ViewBag.ChangeState = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Items", "changeState", delegated_user.id, Convert.ToString(project_id)).FirstOrDefault().column0;
            ViewBag.LoadItems = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Items", "LoadItems", delegated_user.id, Convert.ToString(project_id)).FirstOrDefault().column0;
            ViewBag.ChangeItems = portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Items", "ChangeItems", delegated_user.id, Convert.ToString(project_id)).FirstOrDefault().column0;
            ViewBag.AddQuantity= portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Items", "AddQuantity", delegated_user.id, Convert.ToString(project_id)).FirstOrDefault().column0;
            ViewBag.SetColor= portalDMTOS.UI_GET_ACTION_ROLE("Requirements/Items", "SetColor", delegated_user.id, Convert.ToString(project_id)).FirstOrDefault().column0;
            ViewBag.Colors = portalDMTOS.UI_SELECT_GRID_COLORS().FirstOrDefault().color_list;


            List<UI_SELECT_GRID_SETTINGSResult> grid_settingsTotal = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2", project_id, 1).ToList();
            Settings settingsTotal = new Settings();


            for (int i = 0; i < grid_settingsTotal.Count; i++)
            {

                grid_settingsTotal[i].global_visible = grid_settingsTotal[i].global_visible == null ? true : grid_settingsTotal[i].global_visible;
                grid_settingsTotal[i].is_visible = grid_settingsTotal[i].is_visible == null ? true : grid_settingsTotal[i].is_visible;
                grid_settingsTotal[i].global_editable = grid_settingsTotal[i].global_editable == null ? true : grid_settingsTotal[i].global_editable;

                ViewData["CK_UI_Total_" + grid_settingsTotal[i].field_description] = grid_settingsTotal[i].is_visible & grid_settingsTotal[i].global_visible;
                ViewData["CK_UI_Total_" + grid_settingsTotal[i].field_description + "_width"] = grid_settingsTotal[i].width;
                ViewData["CK_UI_Total_" + grid_settingsTotal[i].field_description + "_ru"] = grid_settingsTotal[i].russian_field_description;
                ViewData["CK_UI_Total_" + grid_settingsTotal[i].field_description + "_pos"] = grid_settingsTotal[i].number;
                ViewData["CK_UI_Total_" + grid_settingsTotal[i].field_description + "_edit"] = grid_settingsTotal[i].global_editable;

            }
            settingsTotal.actionName = "UpdateSettingsOfGrid";
            settingsTotal.checkBoxClass = "UserSettingsCheckboxTotal";
            settingsTotal.controllerName = "Common";
            settingsTotal.flowWindowName = "UserSettingsTotal";
            settingsTotal.storedProcedure = "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2";
            settingsTotal.widthClass = "UserSettingsWidthTotal";
            settingsTotal.positionClass = "PositionClassTotal";
            settingsTotal.gridSettings = grid_settingsTotal;
            ViewBag.SettingsTotal = settingsTotal;


            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
            try { ViewBag.link_info = link_info.description.Replace('"', ' '); } catch { ViewBag.link_info = ""; }




            //UpdateGridCardModel gridCardModel = new UpdateGridCardModel(
            //    "GridCard", 
            //    portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENTS", null, 2).ToList(), 
            //    "Карточка", 
            //    "Grid", 
            //    true, 
            //    true,
            //    "APL_UPDATE_PROJECT_REQUIREMENT", 
            //    "find_row_id", 
            //    null);
            //// { FlowWindowName = "GridCard", FlowWindowRussianName = "Карточка", GridSettings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), Bindning=false, Close_window=false, GridId="GridCard", StoredProcedure= "APL_UPDATE_PROJECT2" };
            ////ViewBag.grid2EditAdd = Convert.ToString(grid_settings2.Any(x => x.global_editable == true)).ToLower();

            //ViewBag.GridCard = gridCardModel;





            ViewData["project_id"] = project_id;
            return View();
        }

        [AppAuthorizeAttribute]
        [HttpPost]
        public string AddQuantity(int? main_prslno_id, double? quantity)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_ADD_PRSLNO_QUANTITY(main_prslno_id, quantity, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }

        [AppAuthorizeAttribute]
        [HttpGet]
        public ActionResult StatePartialUpdate()
        {
            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> CHANGE_STATES=portalDMTOS.UI_SELECT_DROPDOWN("CHANGE_STATES", null, delegated_user.id, au.id).ToList();
            return PartialView("~/Views/Requirements/Shared/StatePartialUpdate.cshtml", CHANGE_STATES);
        }
        [AppAuthorizeAttribute]
        [HttpPost]
        public string StateUpdate(string id_list, int? state_id, string reason_change_state)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_UPDATE_PRSLNO_STATE(id_list, state_id, reason_change_state, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public string SetColor(string grid_type, string item_id_list, string color)
        {
            color = color.Replace("#", "");
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.UI_UPDATE_GRID_ROW_SETTINGS(grid_type, item_id_list, color, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportItems(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, DataSourceLoadOptions loadOptions2, bool? showSelected2, string selectedRecord2, int?project_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECT_REQUIREMENTSResult> x = null;
            List<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result> x2 = null;
            int? event_id = null;
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENTS", "").FirstOrDefault().event_id;


            if (HttpContext.Session.Keys.Contains("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTS"))
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENTSResult>>(HttpContext.Session.GetString("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTS"));
            }
            else
            {
                x = portalDMTOS.APL_SELECT_PROJECT_REQUIREMENTS(event_id, delegated_user.id, au.id,project_id ).ToList();
                HttpContext.Session.SetString("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTSResult", JsonConvert.SerializeObject(x));
            }


            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_PROJECT_REQUIREMENTSResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENTSResult>>(j);




            if (HttpContext.Session.Keys.Contains("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2"))
            {
                x2 = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result>>(HttpContext.Session.GetString("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2"));
            }
            else
            {
                x2 = portalDMTOS.APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2(event_id, delegated_user.id, au.id, project_id).ToList();
                HttpContext.Session.SetString("Requirements" + "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2", JsonConvert.SerializeObject(x));
            }


            int[] selectedRecordMass2 = null;
            if (selectedRecord2 != null && selectedRecord2 != "")
                selectedRecordMass2 = selectedRecord2.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x2 = x2.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults2 = DataSourceLoader.Load(x2, loadOptions2);
            string j2 = JsonConvert.SerializeObject(loadrResults2.data);
            List<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result> list2 = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result>>(j2);




            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<APL_SELECT_PROJECT_REQUIREMENTSResult> excel =new ExcelReports<APL_SELECT_PROJECT_REQUIREMENTSResult>(list, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_REQUIREMENTS", 0, null);
            excel.ExcelReport();


            ExcelReports<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result> excel2 = new ExcelReports<APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2Result>(list2, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_REQUIREMENTS_TOTAL_INFO2", 1, null);
            excel2.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);


            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");


        }






        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExportChangeItems(string id_list)
        {

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            int? event_id = null;
            List<APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult> x = portalDMTOS.APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGE(event_id, id_list, delegated_user.id, au.id).ToList();


            string j = JsonConvert.SerializeObject(x);
            List<APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "APL_SELECT_PROJECT_REQUIREMENT_TEMPLATE" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            int rowCount = 5;
            ExcelReports<APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult> excel =
            new ExcelReports<APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult>(list, rowCount, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_REQUIREMENT_TEMPLATE", 0, null);

            foreach (var a in list)
            {
                excel.SetValueInCellExport(excel.workSheet, rowCount, 1, a.code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 2, a.object_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 3, a.subcontractor_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 4, a.project_documentation, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 5, a.project_documentation_date_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 6, a.project_documentation_receive_date_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 7, a.revision_number_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 8, a.revision_number_date_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 9, a.revision_number_receive_date_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 10, a.start_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 11, a.finish_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 12, a.station_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 13, a.description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 14, a.additional_properties, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 15, a.package_contents, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 16, a.unit_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 17, a.quantity_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 18, a.mass_per_unit_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 19, a.mass_size, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 20, a.manufacturer_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 21, a.delivery_type_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 22, a.goods_type_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 23, a.summary, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 24, a.summary2, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 25, a.state_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 26, a.package_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 27, a.in_kd_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 28, a.psd_state_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 29, a.pr_source_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 30, a.subcontractor_description, null);

                rowCount++;
            }
            excel.ep.Save();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }

        
        [HttpGet]
        [AppAuthorizeAttribute]
        public IActionResult Import(int? project_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow", "[{Name:\"loading_id\", Value:2}, {Name:\"project_id\", Value:" + Convert.ToString(project_id) + "},  {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "},{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
             "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
             "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "APL_INSERT_PROJECT_REQUIREMENT_LOADING_ITEM", "APL_SELECT_PROJECT_REQUIREMENT_LOADING_ITEMS2",
             "APL_SELECT_PROJECT_REQUIREMENT_TEMPLATE",
             "Шаблон загрузки позиций проектной потребности",
             //"PRC_SELECT_ORDER_ITEMS_GKI", 
             2, "ReportGk", ""//, 1,1,1,2
             , 0, "Загрузка/изменение проектной потребности"
             );
            //ViewBag.FileUploader = fileUploader;
            return PartialView("FileUploader", fileUploader);

        }



    }
}