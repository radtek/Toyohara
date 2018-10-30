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
    public class FormsController : Controller
    {

        private IHostingEnvironment _env;
        public FormsController(IHostingEnvironment env)
        {
            _env = env;
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

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORMS", null, 1).ToList();
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
            settings.storedProcedure = "OMC_SELECT_FORMS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;


            ViewData["Columns_Array"] = grid_settings;//homeGridSettings;


            return View();
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_FORMSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_FORMS"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_FORMSResult>>(HttpContext.Session.GetString("OMC_SELECT_FORMS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORMS", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_FORMS(event_id, delegated_user.id, au.id, null,null).ToList();
                HttpContext.Session.SetString("OMC_SELECT_FORMSResult", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_FORMSResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORMSResult> excel =
            new ExcelReports<OMC_SELECT_FORMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString(guid);
        }






//===============SVRItems==================


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> SVRItems(int form_id, bool? showSelected)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS", null, 1).ToList();
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
            settings.storedProcedure = "OMC_SELECT_FORM_SVR_ITEMS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;


            ViewData["Columns_Array"] = grid_settings;
            ViewData["form_id"] = form_id;

            return View();
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportSVRitems(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int form_id, bool? show_only_for_pricing)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_FORM_SVR_ITEMSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_FORM_SVR_ITEMS"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMSResult>>(HttpContext.Session.GetString("OMC_SELECT_FORM_SVR_ITEMS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_FORM_SVR_ITEMS(event_id,  delegated_user.id, au.id, form_id, null, show_only_for_pricing).ToList();
                HttpContext.Session.SetString("OMC_SELECT_FORM_SVR_ITEMS", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_FORM_SVR_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORM_SVR_ITEMSResult> excel =
            new ExcelReports<OMC_SELECT_FORM_SVR_ITEMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_ITEMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString(guid);
        }












//===============RSSItems==================

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> RSSItems(int form_id, bool? showSelected)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_RSS_ITEMS", null, 1).ToList();
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
            settings.storedProcedure = "OMC_SELECT_FORM_RSS_ITEMS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;


            ViewData["Columns_Array"] = grid_settings;
            ViewData["form_id"] = form_id;

            return View();
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportRSSitems(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int form_id, bool? show_only_for_pricing)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_FORM_RSS_ITEMSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_FORM_RSS_ITEMS"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_RSS_ITEMSResult>>(HttpContext.Session.GetString("OMC_SELECT_FORM_RSS_ITEMS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_RSS_ITEMS", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_FORM_RSS_ITEMS(event_id,  delegated_user.id, au.id,form_id,null,show_only_for_pricing).ToList();
                HttpContext.Session.SetString("OMC_SELECT_FORM_RSS_ITEMS", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_SVR_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_SVR_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_SVR_ITEMSResult> excel =
            new ExcelReports<OMC_SELECT_SVR_ITEMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_RSS_ITEMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString(guid);
        }








    }
}