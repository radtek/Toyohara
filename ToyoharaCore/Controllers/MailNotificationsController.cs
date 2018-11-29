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
    public class MailNotifications : Controller
    {

        private IHostingEnvironment _env;
        public MailNotifications(IHostingEnvironment env)
        {
            _env = env;
        }


        //===============Index==================
        [AppAuthorizeAttribute]
        [FAQAttribute]
        public IActionResult Index()
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            ViewBag.AddMailNotification = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "AddMailNotification", delegated_user.id, null).FirstOrDefault().column0;
            ViewBag.EditMailNotification = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "EditMailNotification ", delegated_user.id, null).FirstOrDefault().column0;
            ViewBag.DeleteMailNotification = portalDMTOS.UI_GET_ACTION_ROLE("Projects/Objects", "DeleteMailNotification", delegated_user.id, null).FirstOrDefault().column0;

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "UI_SELECT_MAIL_NOTIFICATIONS", null, 1).ToList();
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
            settings.storedProcedure = "UI_SELECT_MAIL_NOTIFICATIONS";
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
            List<UI_SELECT_MAIL_NOTIFICATIONSResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("MailNotifications" + "UI_SELECT_MAIL_NOTIFICATIONS"))
            {
                x = JsonConvert.DeserializeObject<List<UI_SELECT_MAIL_NOTIFICATIONSResult>>(HttpContext.Session.GetString("MailNotifications" + "UI_SELECT_MAIL_NOTIFICATIONS"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "UI_SELECT_MAIL_NOTIFICATIONS", "").FirstOrDefault().event_id;
                x = portalDMTOS.UI_SELECT_MAIL_NOTIFICATIONS(delegated_user.id, au.id,event_id,  null).ToList();
                HttpContext.Session.SetString("MailNotifications" + "UI_SELECT_MAIL_NOTIFICATIONS", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => m, (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<UI_SELECT_MAIL_NOTIFICATIONSResult> list = JsonConvert.DeserializeObject<List<UI_SELECT_MAIL_NOTIFICATIONSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<UI_SELECT_MAIL_NOTIFICATIONSResult> excel =
            new ExcelReports<UI_SELECT_MAIL_NOTIFICATIONSResult>(list, 1, 1, delegated_user.id, physicalPath, "UI_SELECT_MAIL_NOTIFICATIONS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }

        [AppAuthorizeAttribute]
        public string DeleteMailNotification(int? id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            string error = portalDMTOS.UI_DELETE_MAIL_NOTIFICATION(id, delegated_user.id, au.id).FirstOrDefault().error_description;
            return error;
        }
    }
}