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
    public class ProjectsController : Controller
    {
        private IHostingEnvironment _env;
        public ProjectsController(IHostingEnvironment env)
        {
            _env = env;
        }



        //==========Проекты==============

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public IActionResult Index(string link_information_param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            ViewBag.RoleAddUpdate = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Index", "InsertProject", delegated_user.id, link_information_param).FirstOrDefault().column0;
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2", null, 1).ToList();

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
            Settings settings = new Settings();
            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "APL_SELECT_PROJECT_LIST_INFO2";
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
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECT_LIST_INFO2Result> x = null;
            if (HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_LIST_INFO2Result")
                && HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_LIST_INFO2Result_hide_closed")
                && HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_LIST_INFO2Result_show_mine")
                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("APL_SELECT_PROJECT_LIST_INFO2Result_hide_closed")) == hide_closed
                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("APL_SELECT_PROJECT_LIST_INFO2Result_show_mine")) == show_mine)
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_LIST_INFO2Result>>(HttpContext.Session.GetString("APL_SELECT_PROJECT_LIST_INFO2Result"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2Result", Convert.ToString(hide_closed) + "," + Convert.ToString(show_mine) + "," + Convert.ToString(delegated_user.id)).FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_PROJECT_LIST_INFO2(au.id, delegated_user.id, event_id, null, hide_closed, show_mine).ToList();
                HttpContext.Session.SetString("APL_SELECT_PROJECT_LIST_INFO2Result", JsonConvert.SerializeObject(x));
                HttpContext.Session.SetString("APL_SELECT_PROJECT_LIST_INFO2Result_hide_closed", JsonConvert.SerializeObject(hide_closed));
                HttpContext.Session.SetString("APL_SELECT_PROJECT_LIST_INFO2Result_show_mine", JsonConvert.SerializeObject(show_mine));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<PRC_SELECT_ORDER_ITEMS_GKIResult> list = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<APL_SELECT_PROJECT_LIST_INFO2Result> excel =
            new ExcelReports<APL_SELECT_PROJECT_LIST_INFO2Result>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_LIST_INFO2", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }



        //==========Объекты и подобъекты==============


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public IActionResult Objects(int? project_id, string link_information_param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            ViewBag.InsertObject = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertObject", delegated_user.id, link_information_param).FirstOrDefault().column0;
            ViewBag.InsertSubobject = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertSubobject", delegated_user.id, link_information_param).FirstOrDefault().column0;
            ViewBag.ImportSubobjects = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "ImportSubobjects", delegated_user.id, link_information_param).FirstOrDefault().column0;
            ViewBag.EditObjectsAndSubobjects = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "EditObjectsAndSubobjects", delegated_user.id, link_information_param).FirstOrDefault().column0;
            ViewBag.DeleteSubobject = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "DeleteSubobject", delegated_user.id, link_information_param).FirstOrDefault().column0;


            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
            try { ViewBag.link_inf = link_info.description.Replace('"', ' '); } catch { ViewBag.link_inf = ""; }
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_OBJECTS_AND_SUBOBJECTS", null, 1).ToList();
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
            Settings settings = new Settings();
            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "APL_SELECT_OBJECTS_AND_SUBOBJECTS";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;
            ViewBag.project_id = project_id;

            return View();
        }


        [HttpGet]
        [AppAuthorizeAttribute]
        public IActionResult ImportSubobjects(int? object_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow", "[{Name:\"loading_id\", Value:30}, {Name:\"object_id\", Value:" + Convert.ToString(object_id) + "},  {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "},{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
             "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
             "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "APL_INSERT_PROJECT_LOADING_ITEM", "APL_SELECT_PROJECT_LOADING_ITEMS",
             "APL_SELECT_PROJECT_TEMPLATE",
             "Шаблон загрузки подобъектов",
             //"PRC_SELECT_ORDER_ITEMS_GKI", 
             30, "ReportGk", ""//, 1,1,1,2
             , 0, "Загрузка подобъектов"
             );
            //ViewBag.FileUploader = fileUploader;
            return PartialView("FileUploader", fileUploader);

        } 

        [AppAuthorize]
        [HttpPost]
        public string DeleteItem(string Records)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = "";
            while (Records[0] == ',') Records = Records.Substring(1);
            foreach (int id in Records.Split(',').Select(x => Int32.Parse(x)))
            {
                string error = portalDMTOS.APL_DELETE_SUBOBJECT(id, delegated_user.id, user.id).FirstOrDefault().error_description;
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
        [AppAuthorizeAttribute]        
        public string ExcelObject(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int? id, int? project_id)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult> x = null;
            if (HttpContext.Session.Keys.Contains("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult")
                && HttpContext.Session.Keys.Contains("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_id")
                && HttpContext.Session.Keys.Contains("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_project_id")
                & JsonConvert.DeserializeObject<int?>(HttpContext.Session.GetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_id")) == id
                & JsonConvert.DeserializeObject<int?>(HttpContext.Session.GetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_id")) == project_id
                )
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult>>(HttpContext.Session.GetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_OBJECTS_AND_SUBOBJECTSResult", Convert.ToString(id)).FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_OBJECTS_AND_SUBOBJECTS(au.id, delegated_user.id, event_id, id,project_id).ToList();
                HttpContext.Session.SetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult", JsonConvert.SerializeObject(x));
                HttpContext.Session.SetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_id", JsonConvert.SerializeObject(id));
                HttpContext.Session.SetString("APL_SELECT_OBJECTS_AND_SUBOBJECTSResult_project_id", JsonConvert.SerializeObject(project_id));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult> excel =
            new ExcelReports<APL_SELECT_OBJECTS_AND_SUBOBJECTSResult>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_OBJECTS_AND_SUBOBJECTS", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }




        //==========Пункты маршрута==============

        [HttpGet]
        [AppAuthorizeAttribute]
        public IActionResult RoadGridUpdate(string param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings3 = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LOGISTICAL_NETWORK_OBJECTS", Int32.Parse(param), 1).ToList();
            var dropdown = grid_settings3.Where(x => x.field_description == "lno_id").FirstOrDefault();
            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN(dropdown.dropdown, param, delegated_user.id, au.id).ToList();
            ViewBag.LnoDropdown = JsonConvert.SerializeObject(dropdown_info);
            for (int i = 0; i < grid_settings3.Count; i++)
            {
                if (grid_settings3[i].ui_type == "dropdown_list_id" | grid_settings3[i].ui_type == "dropdown_id" | grid_settings3[i].ui_type == "dropdown_txt_id" | grid_settings3[i].ui_type == "dropdown_txt_list_id")
                {
                    var description = grid_settings3.Where(x => x.ui_type != grid_settings3[i].ui_type & x.dropdown == grid_settings3[i].dropdown).FirstOrDefault();

                    if (Convert.ToBoolean(grid_settings3[i].global_editable))
                    {
                        grid_settings3[i].global_visible = true;
                        grid_settings3[i].is_visible = true;
                        description.is_visible = false;
                        description.global_visible = false;
                    }
                    grid_settings3[i].russian_field_description = description.russian_field_description;
                    grid_settings3[i].width = description.width;

                }
                grid_settings3[i].global_visible = grid_settings3[i].global_visible == null ? true : grid_settings3[i].global_visible;
                grid_settings3[i].is_visible = grid_settings3[i].is_visible == null ? true : grid_settings3[i].is_visible;
                grid_settings3[i].global_editable = grid_settings3[i].global_editable == null ? true : grid_settings3[i].global_editable;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3"] = grid_settings3[i].is_visible & grid_settings3[i].global_visible;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_width"] = grid_settings3[i].width;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_ru"] = grid_settings3[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_pos"] = grid_settings3[i].number;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_edit"] = grid_settings3[i].global_editable;
            }

            Settings settings3 = new Settings();
            settings3.actionName = "UpdateSettingsOfGrid";
            settings3.controllerName = "Common";
            settings3.checkBoxClass = "UserSettingsCheckbox3";
            settings3.flowWindowName = "UserSettings3";
            settings3.storedProcedure = "APL_SELECT_PROJECT_LOGISTICAL_NETWORK_OBJECTS";
            settings3.widthClass = "UserSettingsWidth3";
            settings3.positionClass = "PositionClass3";

            settings3.openParsialDivFunction = "ShowRoads";
            settings3.parsialDivName = "roads";


            settings3.gridSettings = grid_settings3;
            ViewBag.Settings3 = settings3;

            ViewBag.saveBtnVisible = grid_settings3.Where(x => Convert.ToBoolean(x.global_visible) & Convert.ToBoolean(x.is_visible)).Any(x => Convert.ToBoolean(x.global_editable));

            ViewBag.DeleteProjectLNO = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Index", "DeleteProjectLNO", delegated_user.id, param).FirstOrDefault().column0;

            return PartialView("~/Views/Projects/Shared/RoadsPartial.cshtml");

        }


        [AppAuthorize]
        [HttpPost]
        public string DeleteProjectLNO(string Records)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.APL_DELETE_PROJECT_LOGISTICAL_NETWORK_OBJECTS(Records, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelTechRoad(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int? id)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECTS_STATIONINGS2Result> x = null;
            if (HttpContext.Session.Keys.Contains("APL_SELECT_PROJECTS_STATIONINGS2Result")
                && HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_LIST_INFO2Result_hide_closed")
                && HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_LIST_INFO2Result_show_mine")
                & JsonConvert.DeserializeObject<int?>(HttpContext.Session.GetString("APL_SELECT_PROJECTS_STATIONINGS2Result_id")) == id)
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECTS_STATIONINGS2Result>>(HttpContext.Session.GetString("APL_SELECT_PROJECTS_STATIONINGS2Result"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2Result", Convert.ToString(id)).FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_PROJECTS_STATIONINGS2(au.id, delegated_user.id, event_id, id).ToList();
                HttpContext.Session.SetString("APL_SELECT_PROJECTS_STATIONINGS2Result", JsonConvert.SerializeObject(x));
                HttpContext.Session.SetString("APL_SELECT_PROJECTS_STATIONINGS2Result_id", JsonConvert.SerializeObject(id));

            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_PROJECTS_STATIONINGS2Result> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECTS_STATIONINGS2Result>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<APL_SELECT_PROJECTS_STATIONINGS2Result> excel =
            new ExcelReports<APL_SELECT_PROJECTS_STATIONINGS2Result>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECTS_STATIONINGS2", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }


        //==========Шифры====================

        [HttpGet]
        [AppAuthorizeAttribute]
        public ActionResult ShowHideCodesUpdateAdd(string param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings2 = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_DOCUMENTATIONS", null, 1).ToList();
            for (int i = 0; i < grid_settings2.Count; i++)
            {
                grid_settings2[i].global_visible = grid_settings2[i].global_visible == null ? true : grid_settings2[i].global_visible;
                grid_settings2[i].is_visible = grid_settings2[i].is_visible == null ? true : grid_settings2[i].is_visible;
                grid_settings2[i].global_editable = grid_settings2[i].global_editable == null ? true : grid_settings2[i].global_editable;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2"] = grid_settings2[i].is_visible & grid_settings2[i].global_visible;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_width"] = grid_settings2[i].width;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_ru"] = grid_settings2[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_pos"] = grid_settings2[i].number;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_edit"] = grid_settings2[i].global_editable;
            }

            Settings settings2 = new Settings();
            settings2.actionName = "UpdateSettingsOfGrid";
            settings2.controllerName = "Common";
            settings2.checkBoxClass = "UserSettingsCheckbox2";
            settings2.flowWindowName = "UserSettings2";
            settings2.storedProcedure = "APL_SELECT_PROJECT_DOCUMENTATIONS";
            settings2.widthClass = "UserSettingsWidth2";
            settings2.positionClass = "PositionClass2";
            settings2.openParsialDivFunction = "CodesShow";
            settings2.parsialDivName = "projectDocumentationCodes";
            settings2.gridSettings = grid_settings2;
            ViewBag.Settings2 = settings2;

            ViewBag.saveBtnVisible = grid_settings2.Where(x => Convert.ToBoolean(x.global_visible) & Convert.ToBoolean(x.is_visible)).Any(x => Convert.ToBoolean(x.global_editable));


            ViewBag.InsertProjectDocumentation = Convert.ToString(Convert.ToBoolean(portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertProjectDocumentation", delegated_user.id, param).FirstOrDefault().column0)).ToLower();
            ViewBag.InsertProjectDocumentationRevision = Convert.ToString(Convert.ToBoolean(portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertProjectDocumentationRevision", delegated_user.id, param).FirstOrDefault().column0)).ToLower();
            ViewBag.EditProjectDocumentations = Convert.ToString(Convert.ToBoolean(portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "EditProjectDocumentations", delegated_user.id, param).FirstOrDefault().column0)).ToLower();
            ViewBag.DeleteProjectDocumentations = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "DeleteProjectDocumentations", delegated_user.id, param).FirstOrDefault().column0;

            return PartialView("~/Views/Projects/Shared/ProjectDocumentationCodesPartial.cshtml");

        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string InsertProjectDocumentation(int? is_parent, int? id, int? subobject_id, string code, DateTime? date, DateTime? date_receive, int? project_documentation_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = "";
            if (is_parent == 1)
                error = portalDMTOS.APL_UPDATE_PROJECT_DOCUMENTATION2(id, subobject_id, code, date, date_receive, delegated_user.id, au.id).FirstOrDefault().error_description;
            if (is_parent == 0)
                error = portalDMTOS.APL_UPDATE_PROJECT_DOCUMENTATION_REVISION(id, project_documentation_id, date, date_receive, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }



        [AppAuthorize]
        [HttpPost]
        public string DeleteProjectDocumentations(string Records_parent0, string Records_parent1)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.APL_DELETE_PROJECT_DOCUMENTATIONS(Records_parent0, Records_parent1, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }




        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelCodesExport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int? subobject_id)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECT_DOCUMENTATIONSResult> x = null;
            if (HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_DOCUMENTATIONSResult")
                && HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_DOCUMENTATIONSResult_subobject_id")
                & JsonConvert.DeserializeObject<int?>(HttpContext.Session.GetString("APL_SELECT_PROJECT_DOCUMENTATIONSResult_subobject_id")) == subobject_id
                )
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_DOCUMENTATIONSResult>>(HttpContext.Session.GetString("APL_SELECT_PROJECT_DOCUMENTATIONSResult"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECT_DOCUMENTATIONSResult", Convert.ToString(subobject_id)).FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_PROJECT_DOCUMENTATIONS(au.id, delegated_user.id, event_id, subobject_id).ToList();
                HttpContext.Session.SetString("APL_SELECT_PROJECT_DOCUMENTATIONSResult", JsonConvert.SerializeObject(x));
                HttpContext.Session.SetString("APL_SELECT_PROJECT_DOCUMENTATIONSResult_subobject_id", JsonConvert.SerializeObject(subobject_id));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_PROJECT_DOCUMENTATIONSResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_DOCUMENTATIONSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<APL_SELECT_PROJECT_DOCUMENTATIONSResult> excel =
            new ExcelReports<APL_SELECT_PROJECT_DOCUMENTATIONSResult>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_DOCUMENTATIONS", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }

        //==========Тех.участки==============



        [HttpGet]
        [AppAuthorizeAttribute]
        public ActionResult UpdateTechRoadWindow(string param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings3 = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECTS_STATIONINGS2", Int32.Parse(param), 1).ToList();
            var dropdown = grid_settings3.Where(x => x.field_description == "subcontractor_id").FirstOrDefault();
            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN(dropdown.dropdown, param, delegated_user.id, au.id).ToList();
            ViewBag.SubDropdown = JsonConvert.SerializeObject(dropdown_info);
            for (int i = 0; i < grid_settings3.Count; i++)
            {
                if (grid_settings3[i].ui_type == "dropdown_list_id" | grid_settings3[i].ui_type == "dropdown_id" | grid_settings3[i].ui_type == "dropdown_txt_id" | grid_settings3[i].ui_type == "dropdown_txt_list_id")
                {
                    if (Convert.ToBoolean(grid_settings3[i].global_editable))
                    {
                        grid_settings3[i].global_visible = true;
                        grid_settings3[i].is_visible = true;

                        var description = grid_settings3.Where(x => x.ui_type != grid_settings3[i].ui_type & x.dropdown == grid_settings3[i].dropdown).FirstOrDefault();
                        description.is_visible = false;
                        description.global_visible = false;
                        grid_settings3[i].russian_field_description = description.russian_field_description;
                        grid_settings3[i].width = description.width;
                    }
                }
                grid_settings3[i].global_visible = grid_settings3[i].global_visible == null ? true : grid_settings3[i].global_visible;
                grid_settings3[i].is_visible = grid_settings3[i].is_visible == null ? true : grid_settings3[i].is_visible;
                grid_settings3[i].global_editable = grid_settings3[i].global_editable == null ? true : grid_settings3[i].global_editable;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3"] = grid_settings3[i].is_visible & grid_settings3[i].global_visible;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_width"] = grid_settings3[i].width;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_ru"] = grid_settings3[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_pos"] = grid_settings3[i].number;
                ViewData["CK_UI_" + grid_settings3[i].field_description + "_grid_settings3" + "_edit"] = grid_settings3[i].global_editable;
            }

            Settings settings3 = new Settings();
            settings3.actionName = "UpdateSettingsOfGrid";
            settings3.controllerName = "Common";
            settings3.checkBoxClass = "UserSettingsCheckbox3";
            settings3.flowWindowName = "UserSettings3";
            settings3.storedProcedure = "APL_SELECT_PROJECTS_STATIONINGS2";
            settings3.widthClass = "UserSettingsWidth3";
            settings3.positionClass = "PositionClass3";
            settings3.openParsialDivFunction = "ShowTechRoad";
            settings3.parsialDivName = "technical_road";
            settings3.gridSettings = grid_settings3;
            ViewBag.Settings3 = settings3;

            ViewBag.saveBtnVisible = grid_settings3.Where(x => Convert.ToBoolean(x.global_visible) & Convert.ToBoolean(x.is_visible)).Any(x => Convert.ToBoolean(x.global_editable));

            ViewBag.DeleteProjectStationings = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "DeleteProjectStationings", delegated_user.id, param).FirstOrDefault().column0;

            return PartialView("~/Views/Projects/Shared/TechRoadPartial.cshtml");
        }



        [AppAuthorize]
        [HttpPost]
        public string DeleteProjectStationings(string Records)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.APL_DELETE_PROJECT_STATIONINGS(Records, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }







        //==========Субподрядчики==============


        [HttpGet]
        [AppAuthorizeAttribute]
        public ActionResult UpdateSubcontractorWindow(int param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_OBJECT_SUBCONTRACTORS", param, 1).ToList();


            var dropdown = grid_settings.Where(x => x.field_description == "subcontractor_id").FirstOrDefault();
            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dropdown_info = portalDMTOS.UI_SELECT_DROPDOWN(dropdown.dropdown, param.ToString(), delegated_user.id, au.id).ToList();
            ViewBag.SubDropdown = JsonConvert.SerializeObject(dropdown_info);

            for (int i = 0; i < grid_settings.Count; i++)
            {
                if (grid_settings[i].ui_type == "dropdown_list_id" | grid_settings[i].ui_type == "dropdown_id" | grid_settings[i].ui_type == "dropdown_txt_id" | grid_settings[i].ui_type == "dropdown_txt_list_id")
                {
                    if (Convert.ToBoolean(grid_settings[i].global_editable))
                    {
                        grid_settings[i].global_visible = true;
                        grid_settings[i].is_visible = true;

                        var description = grid_settings.Where(x => x.ui_type != grid_settings[i].ui_type & x.dropdown == grid_settings[i].dropdown).FirstOrDefault();
                        description.is_visible = false;
                        description.global_visible = false;
                        grid_settings[i].russian_field_description = description.russian_field_description;
                        grid_settings[i].width = description.width;
                    }
                }
                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_subcontractor"] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_subcontractor" + "_width"] = grid_settings[i].width;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_subcontractor" + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_subcontractor" + "_pos"] = grid_settings[i].number;
                ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings_subcontractor" + "_edit"] = grid_settings[i].global_editable;
            }

            Settings settings = new Settings();
            settings.actionName = "UpdateSettingsOfGrid";
            settings.controllerName = "Common";
            settings.checkBoxClass = "UserSettingsCheckboxSubcontractors";
            settings.flowWindowName = "UserSettingsSubcontractors";
            settings.storedProcedure = "APL_SELECT_OBJECT_SUBCONTRACTORS";
            settings.widthClass = "UserSettingsWidthSubcontractors";
            settings.positionClass = "PositionClassSubcontractors";
            settings.openParsialDivFunction = "ShowSubcontractor";
            settings.parsialDivName = "subcontractors";
            settings.gridSettings = grid_settings;
            ViewBag.SettingsSubcontractors = settings;

            ViewBag.saveBtnVisible = grid_settings.Where(x => Convert.ToBoolean(x.global_visible) & Convert.ToBoolean(x.is_visible)).Any(x => Convert.ToBoolean(x.global_editable));

            ViewBag.DeleteObjectSubcontractors = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "DeleteObjectSubcontractors", delegated_user.id, param.ToString()).FirstOrDefault().column0;

            return PartialView("~/Views/Projects/Shared/SubcontractorsPartial.cshtml");
        }



        [AppAuthorize]
        [HttpPost]
        public string DeleteObjectSubcontractors(string Records)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.APL_DELETE_OBJECT_SUBCONTRACTORS(Records, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelSubcontractors(DataSourceLoadOptions loadOptions, int? id)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_OBJECT_SUBCONTRACTORSResult> x = null;
            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_OBJECT_SUBCONTRACTORS", Convert.ToString(id)).FirstOrDefault().event_id;
            x = portalDMTOS.APL_SELECT_OBJECT_SUBCONTRACTORS(au.id, delegated_user.id, event_id, id).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_OBJECT_SUBCONTRACTORSResult> list = JsonConvert.DeserializeObject<List<APL_SELECT_OBJECT_SUBCONTRACTORSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            ExcelReports<APL_SELECT_OBJECT_SUBCONTRACTORSResult> excel = new ExcelReports<APL_SELECT_OBJECT_SUBCONTRACTORSResult>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_OBJECT_SUBCONTRACTORS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }




    }
}