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

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings2 = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LOGISTICAL_NETWORK_OBJECTS", null, 1).ToList();

            for (int i = 0; i < grid_settings2.Count; i++)
            {
                grid_settings2[i].global_visible = grid_settings2[i].global_visible == null ? true : grid_settings2[i].global_visible;
                grid_settings2[i].is_visible = grid_settings2[i].is_visible == null ? true : grid_settings2[i].is_visible;
                grid_settings2[i].global_editable = grid_settings2[i].global_editable == null ? true : grid_settings2[i].global_editable;
                ViewData["CK_UI_" + grid_settings2[i].field_description+ "_grid_settings2"] = grid_settings2[i].is_visible & grid_settings2[i].global_visible;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_width"] = grid_settings2[i].width;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_ru"] = grid_settings2[i].russian_field_description;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_pos"] = grid_settings2[i].number;
                ViewData["CK_UI_" + grid_settings2[i].field_description + "_grid_settings2" + "_edit"] = grid_settings2[i].global_editable;
            }
            UpdateGridCardModel gridCardModel = new UpdateGridCardModel("GridCard", portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, 
                "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), "Карточка", "Grid", true, true, "APL_UPDATE_PROJECT2", "find_row_id", null);
            // { FlowWindowName = "GridCard", FlowWindowRussianName = "Карточка", GridSettings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), Bindning=false, Close_window=false, GridId="GridCard", StoredProcedure= "APL_UPDATE_PROJECT2" };
            ViewBag.GridCard = gridCardModel;
            return View();
        }

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public IActionResult Objects(int? project_id, string project_description, string link_information_param)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            ViewBag.InsertObject = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertObject", delegated_user.id, link_information_param).FirstOrDefault().column0;
            ViewBag.InsertSubobject = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "InsertSubobject", delegated_user.id, link_information_param).FirstOrDefault().column0;

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_OBJECTS_AND_SUBOBJECTS", null, 1).ToList();
            UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
            link_info.description.Replace("[short_description]", project_description);
            HttpContext.Session.SetString("link_info",JsonConvert.SerializeObject(link_info));

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


            //UpdateGridCardModel gridCardModel = new UpdateGridCardModel("GridCard", portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id,
            //    "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), "Карточка", "Grid", true, true, "APL_UPDATE_PROJECT2", "find_row_id");
            // { FlowWindowName = "GridCard", FlowWindowRussianName = "Карточка", GridSettings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), Bindning=false, Close_window=false, GridId="GridCard", StoredProcedure= "APL_UPDATE_PROJECT2" };
            //ViewBag.GridCard = gridCardModel;
            ViewBag.project_id = project_id;

            UpdateGridCardModel gridCardModel = new UpdateGridCardModel("GridCard", portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id,
            "APL_SELECT_PROJECT_LIST_INFO2", null, 2).ToList(), "Карточка", "Grid", true, true, "APL_UPDATE_PROJECT2", "find_row_id",null);

            return View();
        }
        

       //        [HttpGet]
       //        [AppAuthorizeAttribute]
       //        public object GetDataObjects(DataSourceLoadOptions loadOptions, )         //{
       //            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

       //        PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

       //        List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
       //            if (!rebind && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
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
       //    x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, ekk_guid_list, delegated_user.id, event_id).ToList();
       //    HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
       //                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
       //                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
       //            }

       //int[] selectedRecordMass = null;
       //            if (selectedRecord != null && selectedRecord != "")
       //                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
       //            if (Convert.ToBoolean(showSelected))
       //                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
       //            return DataSourceLoader.Load(x, loadOptions);//d;
       //        }

       //[HttpGet]
       //[AppAuthorizeAttribute]
       //public object Get(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, bool? only_new, bool? show_classified, bool rebind, string ekk_guid_list)

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
                x = portalDMTOS.APL_SELECT_PROJECT_LIST_INFO2(au.id, delegated_user.id, event_id,null, hide_closed, show_mine).ToList();
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

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", "PRC_SELECT_ORDER_ITEMS_GKI" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\Templates\\ExcelTemplates", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);
            //ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel = 
            //    new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x,1,1, grid_settings, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null,()=> portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, delegated_user.id, event_id).ToList());

            ExcelReports<APL_SELECT_PROJECT_LIST_INFO2Result> excel =
            new ExcelReports<APL_SELECT_PROJECT_LIST_INFO2Result>(x, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_LIST_INFO2", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

            return Convert.ToString(guid);
        }


    }
}