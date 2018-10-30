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
    [AppAuthorizeAttribute]
    public class FunctionsController : Controller
    {
        private IHostingEnvironment _env;
        public FunctionsController(IHostingEnvironment env)
        {
            _env = env;
        }
        //{   private SYS_AUTHORIZE_USERResult user { get; set; }
        //    private APL_SELECT_PROJECT_STATES_FOR_DDResult  delegated_user { get; set; }
        //private FunctionsController()
        //{

        //    this.user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
        //    this.delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
        //}

        [FAQAttribute]
        public IActionResult Index()
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            ViewBag.FunctionSection = portalDMTOS.UI_SELECT_LINK_SECTIONS_FOR_DD(delegated_user.id);

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "UI_SELECT_LINK_FUNCTIONS",null,1).ToList();
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
            settings.storedProcedure = "UI_SELECT_LINK_FUNCTIONS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;
            return View();
        }

        //[HttpGet]
        //public object GetFunctions(DataSourceLoadOptions loadOptions, bool rebind, bool? showSelected, string selectedRecord)
        //{
        //    UI_SELECT_LINKResult link_info;
        //    PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
        //    try
        //    {
        //        link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
        //        //ViewBag.FAQ = portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id);
        //        //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
        //        //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));
        //        //  return DataSourceLoader.Load(portalDMTOS.UI_SELECT_LINK_PAGE_NOTES(null,control, view, user_id).ToList(), loadOptions);//d;
        //    }
        //    catch { return BadRequest("Сессия оборвалась. Перезагрузите страницу"); }
        //    //SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
        //    //List<UI_SELECT_LINK_FUNCTIONSResult> list = portalDMTOS.UI_SELECT_LINK_FUNCTIONS(delegated_user.id);

        //    List<UI_SELECT_LINK_FUNCTIONSResult> x = null;
        //    if (!rebind && HttpContext.Session.Keys.Contains("UI_SELECT_LINK_FUNCTIONSResult"))               
        //    {
        //        x = JsonConvert.DeserializeObject<List<UI_SELECT_LINK_FUNCTIONSResult>>(HttpContext.Session.GetString("UI_SELECT_LINK_FUNCTIONSResult"));
        //    }
        //    else
        //    {
        //        APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
        //        x = portalDMTOS.UI_SELECT_LINK_FUNCTIONS(delegated_user.id).ToList();
        //        HttpContext.Session.SetString("UI_SELECT_LINK_FUNCTIONS", JsonConvert.SerializeObject(x));
        //    }

        //    int[] selectedRecordMass = null;
        //    if (selectedRecord != null && selectedRecord != "")
        //        selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
        //    if (Convert.ToBoolean(showSelected))
        //        x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
        //    return DataSourceLoader.Load(x, loadOptions);//d;
        //}

        [AppAuthorize]
        [HttpPost]
        public string RemoveFunctions(string FunctionsRecords)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = "";
            while (FunctionsRecords[0] == ',')
                FunctionsRecords = FunctionsRecords.Substring(1);
            foreach (int id in FunctionsRecords.Split(',').Select(x => Int32.Parse(x)))
            {
                string error = portalDMTOS.UI_DELETE_LINK_FUNCTION(id, delegated_user.id, user.id).FirstOrDefault().error_description;
                if (error != "" && error != null)
                {
                    if (errors != "")
                        errors = errors + "__" + error;
                    else
                        errors = errors + error;
                }
            }
            return errors;
        }

        [HttpPost]
        public string UpdateFunctions(int? id, int? section_id, string title, string stored_procedure, string description)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string error = "";
            if (id == null)
                error=portalDMTOS.UI_INSERT_LINK_FUNCTION(section_id, title, stored_procedure, description, delegated_user.id, user.id).FirstOrDefault().error_description;
            else
                error = portalDMTOS.UI_UPDATE_LINK_FUNCTION(id, section_id, title, stored_procedure, description, delegated_user.id, user.id).FirstOrDefault().error_description;
            return Convert.ToString(error);
        }

        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions,  bool? showSelected, string selectedRecord)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "UI_SELECT_LINK_FUNCTIONS").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_LINK_FUNCTIONSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("UI_SELECT_LINK_FUNCTIONS"))
            {
                x = JsonConvert.DeserializeObject<List<UI_SELECT_LINK_FUNCTIONSResult>>(HttpContext.Session.GetString("UI_SELECT_LINK_FUNCTIONS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "UI_SELECT_LINK_FUNCTIONS","").FirstOrDefault().event_id;
                x = portalDMTOS.UI_SELECT_LINK_FUNCTIONS(delegated_user.id).ToList();
                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<UI_SELECT_LINK_FUNCTIONSResult> list = JsonConvert.DeserializeObject<List<UI_SELECT_LINK_FUNCTIONSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<UI_SELECT_LINK_FUNCTIONSResult> excel =
            new ExcelReports<UI_SELECT_LINK_FUNCTIONSResult>(x, 1, 1, delegated_user.id, physicalPath, "UI_SELECT_LINK_FUNCTIONS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString(guid);
        }

    }
}