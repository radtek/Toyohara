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
    public class SVR_loadingController : Controller
    {

        private IHostingEnvironment _env;
        public SVR_loadingController(IHostingEnvironment env)
        {
            _env = env;
        }

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> Index(bool? showSelected)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_SVR", null, 1).ToList();
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
            settings.storedProcedure = "OMC_SELECT_SVR";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;


            ViewData["Columns_Array"] = grid_settings;//homeGridSettings;
            ViewBag.NOT_CLOSE_PROJECTS_FOR_DD = portalDMTOS.UI_SELECT_DROPDOWN("NOT_CLOSE_PROJECTS", null, delegated_user.id, au.id);



            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow",
                    "[{Name:\"loading_id\", Value:31}, " +
                    "{Name:\"project_id\", Value:null},  " +
                    "{Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
                    "{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
               "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
               "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "OMC_INSERT_SVR_LOADING_ITEM", 
               "OMC_SELECT_SVR_LOADING_ITEM", 
               "OMC_SELECT_SVR_TEMPLATE", 
               "Шаблон загрузки СВР",
               //"PRC_SELECT_ORDER_ITEMS_GKI", 
               31, "ReportGk", "", //1, 5, 1, 5, 
               0
                , "");
            ViewBag.FileUploader = fileUploader;
            return View();
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_SVRResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_SVR"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_SVRResult>>(HttpContext.Session.GetString("OMC_SELECT_SVR"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_SVR", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_SVR(event_id, null, delegated_user.id, au.id).ToList();
                HttpContext.Session.SetString("OMC_SELECT_SVRResult", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_SVRResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_SVRResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);



            ExcelReports<OMC_SELECT_SVRResult> excel =
            new ExcelReports<OMC_SELECT_SVRResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_SVR", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }



        [HttpGet]
        public ActionResult UpdateFileUploader(string project_id, string project_description)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));


            if (project_description == "" | project_description == null)
            { project_description = ""; }
            else
            { project_description = "Загрузка СВР по проекту " + project_description; }

            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow",
                   "[{Name:\"loading_id\", Value:31}, " +
                   "{Name:\"project_id\", Value:" + project_id + "},  " +
                   "{Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
                   "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
              "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
              "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "OMC_INSERT_SVR_LOADING_ITEM", "OMC_SELECT_SVR_LOADING_ITEM",
              "OMC_SELECT_SVR_TEMPLATE",
               "Шаблон загрузки СВР",
              //"PRC_SELECT_ORDER_ITEMS_GKI", 
              31, "ReportGk","", //1, 4, 1, 5, 
              0
               , project_description);

            return PartialView("FileUploader", fileUploader);
        }


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> SVR_items(int svr_id, bool? showSelected, string link_information_param)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_SVR_ITEMS", null, 1).ToList();
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
            settings.storedProcedure = "OMC_SELECT_SVR_ITEMS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;


            ViewData["Columns_Array"] = grid_settings;
            ViewData["svr_id"] = svr_id;

            return View();
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportSVRitems(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int svr_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_SVR_ITEMSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_SVR_ITEMS"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_SVR_ITEMSResult>>(HttpContext.Session.GetString("OMC_SELECT_SVR_ITEMS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_SVR_ITEMS", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_SVR_ITEMS(event_id, svr_id, delegated_user.id, au.id).ToList();
                HttpContext.Session.SetString("OMC_SELECT_SVR_ITEMSResult", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_SVR_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_SVR_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_SVR_ITEMSResult> excel =
            new ExcelReports<OMC_SELECT_SVR_ITEMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_SVR_ITEMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }





    }
}