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
    public class ProjectRequirementChange : Controller
    {

        private IHostingEnvironment _env;
        public ProjectRequirementChange(IHostingEnvironment env)
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

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2", null, 1).ToList();
            Settings settingsForApprove = new Settings();


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
            settingsForApprove.actionName = "UpdateSettingsOfGrid";
            settingsForApprove.checkBoxClass = "UserSettingsCheckbox";
            settingsForApprove.controllerName = "Common";
            settingsForApprove.flowWindowName = "UserSettings";
            settingsForApprove.storedProcedure = "APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2";
            settingsForApprove.widthClass = "UserSettingsWidth";
            settingsForApprove.positionClass = "PositionClassFor";
            settingsForApprove.gridSettings = grid_settings;
            ViewBag.UserSettings = settingsForApprove;


            ViewData["Columns_Array"] = grid_settings;//homeGridSettings;

            return View();
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, bool? only_active, bool? is_not_for_approve)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2"))
            {
                x = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result>>(HttpContext.Session.GetString("APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2", "").FirstOrDefault().event_id;
                x = portalDMTOS.APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2(event_id, delegated_user.id, au.id, only_active, is_not_for_approve).ToList();
                HttpContext.Session.SetString("APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result> list = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result> excel =
            new ExcelReports<APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result>(list, 1, 1, delegated_user.id, physicalPath, "APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }




        [AppAuthorizeAttribute]
        [HttpPost]
        public string ApproveRequest(string item_id_list)
        {
            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = "";
            //string error = portalDMTOS.APL_APPROVE_PROJECT_REQUIREMENT_CHANGE_REQUEST2(item_id_list, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;

        }



        [AppAuthorizeAttribute]
        [HttpPost]
        public string DeclineRequest(string item_id_list, string note)
        {
            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_DECLINE_PROJECT_REQUIREMENT_CHANGE_REQUEST2(item_id_list, note, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;

        }


        [AppAuthorizeAttribute]
        [HttpPost]
        public string ApproveRequestNPS(string item_id_list, int is_nps)
        {
            //SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.APL_UPDATE_PROJECT_REQUIREMENT_CHANGE_REQUEST_STATE2(item_id_list, is_nps, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;

        }


    }
}