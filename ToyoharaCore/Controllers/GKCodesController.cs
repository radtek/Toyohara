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
    public class GKCodesController : Controller
    {

        private IHostingEnvironment _env;
        public GKCodesController(IHostingEnvironment env)
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
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "PRC_SELECT_ORDER_ITEMS_GKI",null,1).ToList();
            ViewBag.EkkCode = new EkkCodeModel(portalDMTOS.MDM_SELECT_INVENTORY_CLASSES_FOR_GRAPH().ToList(), "ekk_code_name", "ekk_text", true, "FullSearch");
            //ViewBag.EkkCode2 = new EkkCodeModel(portalDMTOS.MDM_SELECT_INVENTORY_CLASSES_FOR_GRAPH().ToList(), "ekk_code_name2");
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
            settings.storedProcedure = "PRC_SELECT_ORDER_ITEMS_GKI";
            settings.widthClass = "UserSettingsWidth";
            settings.positionClass = "PositionClass";
            settings.gridSettings = grid_settings;
            ViewBag.Settings = settings;
            ViewData["Columns_Array"] = grid_settings;//homeGridSettings;
            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> dd = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI_STATES_FOR_DD();
            ViewBag.StateDropdown = JsonConvert.SerializeObject(dd);


            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow", "[{Name:\"loading_id\", Value:30}, {Name:\"object_id\", Value:null},  {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "},{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
               "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
               "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "APL_INSERT_PROJECT_LOADING_ITEM", "APL_SELECT_PROJECT_LOADING_ITEMS", "ExcelInsert", "PRC_SELECT_ORDER_ITEMS_GKI", 30, "ReportGk", "", 1,1,1,2,0,"Лодер"
                );
            ViewBag.FileUploader = fileUploader;
            return View();
        }

        //[HttpGet]
        //[AppAuthorizeAttribute]
        //public object Get(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, bool? only_new, bool? show_classified, bool rebind, string ekk_guid_list)
        //{
        //    APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

        //    PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

        //    List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
        //    if (!rebind && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
        //        && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")
        //        && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")
        //        & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")) == show_classified
        //        & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")) == only_new)
        //    {
        //        x = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult"));
        //    }
        //    else
        //    {
        //        int? event_id = null;
        //        x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, ekk_guid_list, delegated_user.id, event_id).ToList();
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
        //    }

        //    int[] selectedRecordMass = null;
        //    if (selectedRecord != null && selectedRecord != "")
        //        selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
        //    if (Convert.ToBoolean(showSelected))
        //        x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
        //    return DataSourceLoader.Load(x, loadOptions);//d;
        //}


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions, string test, bool? showSelected, string selectedRecord, bool? only_new, bool? show_classified, string ekk_guid_list)
        {
            int? event_id = null;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //List<UI_SELECT_GRID_SETTINGS2Result> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS2(au.id, "PRC_SELECT_ORDER_ITEMS_GKI").ToList();
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
            if (HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")
                && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")
                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")) == show_classified
                & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")) == only_new)
            {
                x = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "PRC_SELECT_ORDER_ITEMS_GKI", Convert.ToString(show_classified) + "," + Convert.ToString(only_new) + "," + Convert.ToString(delegated_user.id)).FirstOrDefault().event_id;
                x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, ekk_guid_list, delegated_user.id, event_id).ToList();
                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
                HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
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

            ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult> excel =
            new ExcelReports<PRC_SELECT_ORDER_ITEMS_GKIResult>(x, 1, 1, delegated_user.id, physicalPath, "PRC_SELECT_ORDER_ITEMS_GKI", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);

                return Convert.ToString(guid);
        }

        //[HttpPut]
        //[AppAuthorizeAttribute]
        //public IActionResult Put(int key, string values, bool? show_classified, bool? only_new, string ekk_guid_list)
        //{
        //    SYS_AUTHORIZE_USER2Result au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
        //    APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

        //    PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

        //    List<PRC_SELECT_ORDER_ITEMS_GKIResult> x = null;
        //    if (HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult")
        //        && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")
        //        && HttpContext.Session.Keys.Contains("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")
        //        & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified")) == show_classified
        //        & JsonConvert.DeserializeObject<bool?>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new")) == only_new)
        //    {
        //        x = JsonConvert.DeserializeObject<List<PRC_SELECT_ORDER_ITEMS_GKIResult>>(HttpContext.Session.GetString("PRC_SELECT_ORDER_ITEMS_GKIResult"));
        //    }
        //    else
        //    {
        //        int? event_id = null;
        //        x = portalDMTOS.PRC_SELECT_ORDER_ITEMS_GKI(show_classified, only_new, ekk_guid_list, au.delegating_user_id, event_id).ToList();
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult", JsonConvert.SerializeObject(x));
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_only_new", JsonConvert.SerializeObject(only_new));
        //        HttpContext.Session.SetString("PRC_SELECT_ORDER_ITEMS_GKIResult_show_classified", JsonConvert.SerializeObject(show_classified));
        //    }

        //    PRC_SELECT_ORDER_ITEMS_GKIResult x2 = x.Where(y => y.id == key).FirstOrDefault();
        //    JsonConvert.PopulateObject(values, x2);
        //    portalDMTOS.PRC_UPDATE_ORDER_ITEM_GKI(key, x2.gki_code, x2.gki_state_id, x2.gki_order_number, x2.note, au.delegating_user_id, delegated_user.id);
        //    //return BadRequest("Сервер вернул ошибку, обратитесь в техническую поддержку");

        //    return Ok();
        //}



    }
}