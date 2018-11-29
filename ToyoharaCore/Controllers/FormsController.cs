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
        public string ExcelReport(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, bool? hide_archive)
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
                x = portalDMTOS.OMC_SELECT_FORMS(event_id, delegated_user.id, au.id, null, hide_archive).ToList();
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

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORMSResult> excel =
            new ExcelReports<OMC_SELECT_FORMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }






//===============SVRItems==================


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> SVRItems(int? form_id, bool? showSelected, string link_information_param)
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


            ViewBag.InsertArchive = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "InsertArchive", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportDeliveryTypeDifference = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ReportDeliveryTypeDifference", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.GroupUpdate = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "GroupUpdate", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportDiff = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ReportDiff", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ManualCommitChanges = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ManualCommitChanges", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;


            ViewBag.VisibleDeclineComment = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "DeclineComment", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleAddInfo = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "AddInfo", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisiblePriceManager = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "PriceManager", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleForPricing = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ForPricing", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleDeliveryType = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "DeliveryType", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportSubcontractorItems = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ReportSubcontractorItems", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;


            

            ViewBag.PriceManager = portalDMTOS.UI_SELECT_DROPDOWN("PRICE_MANAGERS", null, delegated_user.id, au.id);
            ViewBag.DeliveryType = portalDMTOS.UI_SELECT_DROPDOWN("DELIVERY_TYPES", null, delegated_user.id, au.id);

            ViewData["Columns_Array"] = grid_settings;
            ViewData["form_id"] = form_id;



            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow",
                    "[{Name:\"loading_id\", Value:32}, " +
                    "{Name:\"form_id\", Value:"+Convert.ToString(form_id) +"},  " +
                    "{Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
                    "{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
               "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
               "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "OMC_INSERT_FORM_DELIVERY_TYPE_LOADING_ITEM", "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMS", 
               "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE", 
               "Шаблон загрузки типов поставки",
               //"EMPTY", 
               32, "ReportGk", "", //1, 3, 1, 3, 
               0
                , "Загрузка типов поставки для СВР (код - "+ form_id.ToString() + ")");
            ViewBag.FileUploader = fileUploader;


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

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORM_SVR_ITEMSResult> excel =
            new ExcelReports<OMC_SELECT_FORM_SVR_ITEMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_ITEMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }


        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportSVRitemsSubcontractorForm(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMSResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMS", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMS(event_id, delegated_user.id, au.id, form_id).ToList();
            HttpContext.Session.SetString("OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMS", JsonConvert.SerializeObject(x));


            string j = JsonConvert.SerializeObject(x);
            List<OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMS" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            int rowCount = 5;
            ExcelReports<OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMSResult> excel =
            new ExcelReports<OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMSResult>(list, rowCount, 1, delegated_user.id, physicalPath, "OMC_REPORT_FORM_SVR_SUBCONTRACTOR_ITEMS", 0, null);

            foreach (var a in list)
            {
                excel.SetValueInCellExport(excel.workSheet, rowCount, 1, a.id, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 2, a.local_estimate_number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 3, a.project_documentation_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 4, a.number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 5, a.resource_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 6, a.ekk_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 7, a.description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 8, a.unit_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 9, a.quantity, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 10, a.price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 11, a.summa, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 12, a.turnover, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 13, a.phase, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 14, a.price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 15, a.supplier_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 16, a.delivery_base, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 17, a.price_source_number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 18, a.comment, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 19, a.price_source_link, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 20, a.price_source, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 21, a.price_source_year, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 22, a.delivery_to_zhd_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 23, a.delivery_from_zhd_to_object_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 24, a.delivery_auto_to_object_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 25, a.prr_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 26, a.package_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 27, a.storage_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 28, a.total_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 29, a.summa, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 30, a.total_summa, null);
                rowCount++;
            }
            excel.ep.Save();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }


       
        [HttpPost]
        [AppAuthorizeAttribute]
        public string ReportDeliveryTypeDifferenceSVR(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCEResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCE", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCE(event_id, delegated_user.id, au.id, form_id).ToList();
 
            string j = JsonConvert.SerializeObject(x);
            List<OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCEResult> list = JsonConvert.DeserializeObject<List<OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCEResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);


            ExcelReports<OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCEResult> excel =
            new ExcelReports<OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCEResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_REPORT_FORM_SVR_DELIVERY_TYPE_DIFFERENCE", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }


        [AppAuthorize]
        [HttpPost]
        public string InsertArchive(int form_id, int form_type_id)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = "";
            if (form_type_id == 5)
            {
                errors = portalDMTOS.OMC_INSERT_SVR_ARCHIVE(form_id, delegated_user.id, user.id).FirstOrDefault().error_description;
            }
            if (form_type_id == 4)
            {
                errors = portalDMTOS.OMC_INSERT_RSS_ARCHIVE(form_id, delegated_user.id, user.id).FirstOrDefault().error_description;
            }
            
            return errors;
        }


        [AppAuthorize]
        [HttpPost]
        public string GroupUpdate(int form_type_id, string selectedRecord, bool? isDeclineComment, string DeclineComment, bool? isAddInfo, string AddInfo, int? PriceManager, bool? ForPricing, int? DeliveryType)
        {
            if (isDeclineComment == true & DeclineComment == null)
                DeclineComment = "";
            if (isAddInfo == true & AddInfo == null)
                AddInfo = "";

            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = "";
            if (form_type_id == 5)
            {
                errors = portalDMTOS.OMC_UPDATE_FORM_ITEM_PARAMS(null, selectedRecord, DeclineComment, ForPricing, PriceManager, AddInfo, DeliveryType, delegated_user.id, user.id).FirstOrDefault().error_description;
            }
            if (form_type_id == 4)
            {
                errors = portalDMTOS.OMC_UPDATE_FORM_ITEM_PARAMS(selectedRecord, null, DeclineComment, ForPricing, PriceManager, AddInfo, DeliveryType, delegated_user.id, user.id).FirstOrDefault().error_description;
            }

            return errors;
        }



        
        [HttpPost]
        [AppAuthorizeAttribute]
        public string SVRDeliveryTypeTemplate(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATEResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE(event_id, form_id, delegated_user.id, au.id).ToList();


            string j = JsonConvert.SerializeObject(x);
            List<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATEResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATEResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            int rowCount = 4;
            ExcelReports<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATEResult> excel =
            new ExcelReports<OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATEResult>(list, rowCount, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE", 0, null);

            foreach (var a in list)
            {
                excel.SetValueInCellExport(excel.workSheet, rowCount, 1, a.item_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 2, a.phase, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 3, a.local_estimate_number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 4, a.project_documentation_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 5, a.number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 6, a.resource_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 7, a.ekk_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 8, a.description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 9, a.unit_description, null );
                excel.SetValueInCellExport(excel.workSheet, rowCount, 10, a.quantity, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 11, a.add_info, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 12, a.svr_delivery_type, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 13, a.suggested_delivery_type_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 14, a.delivery_type_1, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 15, a.delivery_type_2, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 16, a.delivery_type_3, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 17, a.delivery_type_4, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 18, a.delivery_type_5, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 19, a.delivery_type_6, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 20, a.delivery_type_7, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 21, a.delivery_type_8, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 22, a.delivery_type_9, null);
                rowCount++;
            }
            excel.ep.Save();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }

        //===============RSSItems==================

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> RSSItems(int? form_id, bool? showSelected, string link_information_param)
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



            ViewBag.InsertArchive = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "InsertArchive", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportDeliveryTypeDifference = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "ReportDeliveryTypeDifference", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.GroupUpdate = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "GroupUpdate", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportDiff = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItems", "ReportDiff", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;

            ViewBag.VisibleDeclineComment = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "DeclineComment", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleAddInfo = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "AddInfo", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisiblePriceManager = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "PriceManager", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleForPricing = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "ForPricing", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.VisibleDeliveryType = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "DeliveryType", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;
            ViewBag.ReportSubcontractorItems = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItems", "ReportSubcontractorItems", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;

            ViewBag.PriceManager = portalDMTOS.UI_SELECT_DROPDOWN("PRICE_MANAGERS", null, delegated_user.id, au.id);
            ViewBag.DeliveryType = portalDMTOS.UI_SELECT_DROPDOWN("DELIVERY_TYPES", null, delegated_user.id, au.id);


            ViewData["Columns_Array"] = grid_settings;
            ViewData["form_id"] = form_id;


            FileUploader fileUploader = new FileUploader("Uploader", "UploadFileAccept", "/Common/UploadFile", "ReportGk", "upload_class", "UploaderFlowWindow",
                    "[{Name:\"loading_id\", Value:33}, " +
                    "{Name:\"form_id\", Value:" + Convert.ToString(form_id) + "},  " +
                    "{Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
                    "{ Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]",
               "[{Name:\"loading_id\", Value:\"\"}, {Name:\"loading_id\", Value:null}, {Name:\"user_id\", Value:" + Convert.ToString(delegated_user.id) + "}," +
               "{Name:\"real_user_id\", Value:" + Convert.ToString(au.id) + "}]", "OMC_INSERT_FORM_DELIVERY_TYPE_LOADING_ITEM", "OMC_SELECT_FORM_RSS_DELIVERY_TYPE_LOADING_ITEMS", 
               "OMC_SELECT_FORM_SVR_DELIVERY_TYPE_TEMPLATE",
               "Шаблон загрузки типов поставки",
               //"EMPTY", 
               33, "ReportGk", "", //1, 3, 1, 3, 
               0
                , "Загрузка типов поставки для РСС (код - " + form_id.ToString() + ")");
            ViewBag.FileUploader = fileUploader;






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
            List<OMC_SELECT_FORM_RSS_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_RSS_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORM_RSS_ITEMSResult> excel =
            new ExcelReports<OMC_SELECT_FORM_RSS_ITEMSResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_RSS_ITEMS", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelReportRSSitemsSubcontractorForm(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMSResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMS", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMS(event_id, delegated_user.id, au.id, form_id).ToList();
            HttpContext.Session.SetString("OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMS", JsonConvert.SerializeObject(x));


            string j = JsonConvert.SerializeObject(x);
            List<OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMSResult> list = JsonConvert.DeserializeObject<List<OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMSResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMS" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            int rowCount = 5;
            ExcelReports<OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMSResult> excel =
            new ExcelReports<OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMSResult>(list, rowCount, 1, delegated_user.id, physicalPath, "OMC_REPORT_FORM_RSS_SUBCONTRACTOR_ITEMS", 0, null);

            foreach (var a in list)
            {
                excel.SetValueInCellExport(excel.workSheet, rowCount, 1, a.code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 2, a.object_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 3, a.subobject_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 4, a.subcontractor_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 5, a.lno_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 6, a.project_documentation_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 7, a.revision_number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 8, a.ekk_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 9, a.description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 10, a.package_contents, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 11, a.additional_properties, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 12, a.manufacturer, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 13, a.unit_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 14, a.quantity, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 15, a.mass_per_unit, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 16, a.mass_size, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 17, a.summary, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 18, a.summary2, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 19, a.price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 20, a.supplier_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 21, a.delivery_base, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 22, a.price_source_number, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 23, a.comment, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 24, a.price_source_link, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 25, a.price_source, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 26, a.price_source_year, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 27, a.delivery_to_zhd_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 28, a.delivery_from_zhd_to_object_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 29, a.delivery_auto_to_object_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 30, a.prr_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 31, a.package_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 32, a.storage_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 33, a.total_price, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 34, a.summa, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 35, a.total_summa, null);
                rowCount++;
            }


            excel.ep.Save();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }



        [HttpPost]
        [AppAuthorizeAttribute]
        public string ReportDeliveryTypeDifferenceRSS(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCE", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCE(event_id, delegated_user.id, au.id, form_id).ToList();

            string j = JsonConvert.SerializeObject(x);
            List<OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult> list = JsonConvert.DeserializeObject<List<OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);


            ExcelReports<OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult> excel =
            new ExcelReports<OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCE", 0, null);
            excel.ExcelReport();

            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }


        //===============SVRItemsDiff==================

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> SVRItemsDiff(int? form_id, bool? showSelected, string link_information_param)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS_DIFF", null, 1).ToList();
            Settings settings = new Settings();


            ViewData["columns"] = "";
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


                ViewData["columns"] += "columns.AddFor(m => m." + grid_settings[i].field_description + ").HeaderCellTemplate(Convert.ToString(ViewData[\"CK_UI_" + grid_settings[i].field_description + "_ru\"])).Visible((bool)ViewData[\"CK_UI_" + grid_settings[i].field_description + "\"]).Width((int)ViewData[\"CK_UI_" + grid_settings[i].field_description + "_width\"]).VisibleIndex((int)ViewData[\"CK_UI_" + grid_settings[i].field_description + "_pos\"]).AllowEditing((bool)ViewData[\"CK_UI_" + grid_settings[i].field_description + "_edit\"]);";

            }

        

            ViewBag.CommitChanges = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItemsDiff", "CommitChanges", delegated_user.id, form_id.ToString()).FirstOrDefault().column0;

            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "OMC_SELECT_FORM_SVR_ITEMS_DIFF";
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
        public string ExcelReportSVRItemsDiff(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_FORM_SVR_ITEMS_DIFF"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult>>(HttpContext.Session.GetString("OMC_SELECT_FORM_SVR_ITEMS_DIFF"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS_DIFF", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_FORM_SVR_ITEMS_DIFF(event_id, delegated_user.id, au.id, form_id).ToList();
                HttpContext.Session.SetString("OMC_SELECT_FORM_SVR_ITEMS_DIFF", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => Convert.ToString(m), (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult> excel =
            new ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_DIFFResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_ITEMS_DIFF", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid+".xlsx");
        }



        
        [AppAuthorize]
        [HttpPost]
        public string CommitChangesSVR(int form_id, string FunctionsRecords)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.OMC_COMMIT_SVR_CHANGES(form_id, FunctionsRecords, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }






        [HttpPost]
        [AppAuthorizeAttribute]
        public string RSSDeliveryTypeTemplate(int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATEResult> x = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATE", "").FirstOrDefault().event_id;
            x = portalDMTOS.OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATE(event_id, form_id, delegated_user.id, au.id).ToList();


            string j = JsonConvert.SerializeObject(x);
            List<OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATEResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATEResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATE" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            int rowCount = 4;
            ExcelReports<OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATEResult> excel =
            new ExcelReports<OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATEResult>(list, rowCount, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_RSS_DELIVERY_TYPE_TEMPLATE", 0, null);

            //excel.ExcelReport(null, 26);

            foreach (var a in list)
            {
                excel.SetValueInCellExport(excel.workSheet, rowCount, 1, a.item_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 2, a.object_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 3, a.subobject_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 4, a.subcontractor_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 5, a.lno_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 6, a.project_documentation_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 7, a.ekk_code, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 8, a.description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 9, a.additional_properties, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 10, a.package_contents, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 11, a.manufacturer, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 12, a.unit_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 13, a.quantity, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 14, a.summary, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 15, a.summary2, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 16, a.rss_delivery_type_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 17, a.suggested_delivery_type_description, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 18, a.delivery_type_1, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 19, a.delivery_type_2, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 20, a.delivery_type_3, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 21, a.delivery_type_4, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 22, a.delivery_type_5, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 23, a.delivery_type_6, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 24, a.delivery_type_7, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 25, a.delivery_type_8, null);
                excel.SetValueInCellExport(excel.workSheet, rowCount, 26, a.delivery_type_9, null);

                rowCount++;
            }
            excel.ep.Save();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }






        //===============RSSItemsDiff==================

        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> RSSItemsDiff(int? form_id, bool? showSelected, string link_information_param)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_RSS_ITEMS_DIFF", null, 1).ToList();
            Settings settings = new Settings();

            ViewData["columns"] = "";
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

                ViewData["columns"] += "columns.AddFor(m => m."+ grid_settings[i].field_description + ").HeaderCellTemplate(Convert.ToString(ViewData[\"CK_UI_"+ grid_settings[i].field_description + "_ru\"])).Visible((bool)ViewData[\"CK_UI_"+grid_settings[i].field_description+"\"]).Width((int)ViewData[\"CK_UI_"+ grid_settings[i].field_description + "_width\"]).VisibleIndex((int)ViewData[\"CK_UI_"+ grid_settings[i].field_description + "_pos\"]).AllowEditing((bool)ViewData[\"CK_UI_"+ grid_settings[i].field_description + "_edit\"]);";
            }

            ViewBag.CommitChanges = portalDMTOS.UI_GET_ACTION_ROLE("Forms/RSSItemsDiff", "CommitChanges", delegated_user.id, null).FirstOrDefault().column0;

            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckbox";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettings";
            settings.storedProcedure = "OMC_SELECT_FORM_RSS_ITEMS_DIFF";
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
        public string ExcelReportRSSItemsDiff(DataSourceLoadOptions loadOptions, bool? showSelected, string selectedRecord, int form_id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            List<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult> x = null;
            int? event_id = null;
            if (HttpContext.Session.Keys.Contains("OMC_SELECT_FORM_RSS_ITEMS_DIFF"))
            {
                x = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult>>(HttpContext.Session.GetString("OMC_SELECT_FORM_RSS_ITEMS_DIFF"));
            }
            else
            {
                event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_RSS_ITEMS_DIFF", "").FirstOrDefault().event_id;
                x = portalDMTOS.OMC_SELECT_FORM_RSS_ITEMS_DIFF(event_id, delegated_user.id, au.id, form_id).ToList();
                HttpContext.Session.SetString("OMC_SELECT_FORM_RSS_ITEMS_DIFF", JsonConvert.SerializeObject(x));
            }

            int[] selectedRecordMass = null;
            if (selectedRecord != null && selectedRecord != "")
                selectedRecordMass = selectedRecord.Split(',').Select(Int32.Parse).ToArray();
            if (Convert.ToBoolean(showSelected))
                x = x.Join(selectedRecordMass, y => y.id, m => Convert.ToString(m), (y, m) => y).ToList();
            DevExtreme.AspNet.Data.ResponseModel.LoadResult loadrResults = DataSourceLoader.Load(x, loadOptions);
            string j = JsonConvert.SerializeObject(loadrResults.data);
            List<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult> list = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult>>(j);

            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);

            ExcelReports<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult> excel =
            new ExcelReports<OMC_SELECT_FORM_RSS_ITEMS_DIFFResult>(list, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_RSS_ITEMS_DIFF", 0, null);
            excel.ExcelReport();
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");
        }




        [AppAuthorize]
        [HttpPost]
        public string CommitChangesRSS(int form_id, string FunctionsRecords)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.OMC_COMMIT_RSS_CHANGES(form_id, FunctionsRecords, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }




        //===============SVRItemsManualDiff==================


        [AppAuthorizeAttribute]
        [FAQAttribute]
        public async Task<IActionResult> SVRItemsManualDiff(int? form_id, string link_information_param)
        {
            var webRoot = _env.WebRootPath;
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            IEnumerable<APL_SELECT_PROJECT_STATES_FOR_DDResult> apl = await portalDMTOS.APL_SELECT_PROJECT_STATES_FOR_DDAsync();

            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult delegated_user = JsonConvert.DeserializeObject<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS_ADDED", null, 1).ToList();
            Settings settings = new Settings();

            for (int i = 0; i < grid_settings.Count; i++)
            {
                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;

                ViewData["add_CK_UI_" + grid_settings[i].field_description] = grid_settings[i].is_visible & grid_settings[i].global_visible;
                ViewData["add_CK_UI_" + grid_settings[i].field_description + "_width"] = grid_settings[i].width;
                ViewData["add_CK_UI_" + grid_settings[i].field_description + "_ru"] = grid_settings[i].russian_field_description;
                ViewData["add_CK_UI_" + grid_settings[i].field_description + "_pos"] = grid_settings[i].number;
                ViewData["add_CK_UI_" + grid_settings[i].field_description + "_edit"] = grid_settings[i].global_editable;

            }
            settings.actionName = "UpdateSettingsOfGrid";
            settings.checkBoxClass = "UserSettingsCheckboxAdded";
            settings.controllerName = "Common";
            settings.flowWindowName = "UserSettingsAdded";
            settings.storedProcedure = "OMC_SELECT_FORM_SVR_ITEMS_ADDED";
            settings.widthClass = "UserSettingsWidthAdded";
            settings.positionClass = "PositionClassAdded";
            settings.gridSettings = grid_settings;
            ViewBag.SettingsAdded = settings;



            List<UI_SELECT_GRID_SETTINGSResult> grid_settingsDeleted = portalDMTOS.UI_SELECT_GRID_SETTINGS(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS_DELETED", null, 1).ToList();
            Settings settingsDeleted = new Settings();

            for (int i = 0; i < grid_settingsDeleted.Count; i++)
            {
                grid_settingsDeleted[i].global_visible = grid_settingsDeleted[i].global_visible == null ? true : grid_settingsDeleted[i].global_visible;
                grid_settingsDeleted[i].is_visible = grid_settingsDeleted[i].is_visible == null ? true : grid_settingsDeleted[i].is_visible;
                grid_settingsDeleted[i].global_editable = grid_settingsDeleted[i].global_editable == null ? true : grid_settingsDeleted[i].global_editable;

                ViewData["del_CK_UI_" + grid_settingsDeleted[i].field_description] = grid_settingsDeleted[i].is_visible & grid_settingsDeleted[i].global_visible;
                ViewData["del_CK_UI_" + grid_settingsDeleted[i].field_description + "_width"] = grid_settingsDeleted[i].width;
                ViewData["del_CK_UI_" + grid_settingsDeleted[i].field_description + "_ru"] = grid_settingsDeleted[i].russian_field_description;
                ViewData["del_CK_UI_" + grid_settingsDeleted[i].field_description + "_pos"] = grid_settingsDeleted[i].number;
                ViewData["del_CK_UI_" + grid_settingsDeleted[i].field_description + "_edit"] = grid_settingsDeleted[i].global_editable;

            }
            settingsDeleted.actionName = "UpdateSettingsOfGrid";
            settingsDeleted.checkBoxClass = "UserSettingsCheckboxDeleted";
            settingsDeleted.controllerName = "Common";
            settingsDeleted.flowWindowName = "UserSettingsDeleted";
            settingsDeleted.storedProcedure = "OMC_SELECT_FORM_SVR_ITEMS_DELETED";
            settingsDeleted.widthClass = "UserSettingsWidthDeleted";
            settingsDeleted.positionClass = "PositionClassDeleted";
            settingsDeleted.gridSettings = grid_settingsDeleted;
            ViewBag.SettingsDeleted = settingsDeleted;

            ViewBag.ManualCommitChanges = portalDMTOS.UI_GET_ACTION_ROLE("Forms/SVRItemsManualDiff", "ManualCommitChanges", delegated_user.id, null).FirstOrDefault().column0;
            //ViewData["Columns_Array"] = grid_settings;
            ViewData["form_id"] = form_id;


            return View();
        }




        [HttpPost]
        [AppAuthorizeAttribute]
        public string ExcelSVRItemsManualDiff(int form_id, bool? only_last_deleted, bool? only_last_added,DataSourceLoadOptions loadOptionsDeleted, DataSourceLoadOptions loadOptionsAdded)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));

            List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> xadded = null;
            List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> xdeleted = null;
            int? event_id = null;

            event_id = portalDMTOS.SYS_START_EVENT(delegated_user.id, "OMC_SELECT_FORM_SVR_ITEMS_ADDED", "").FirstOrDefault().event_id;

            xdeleted = portalDMTOS.OMC_SELECT_FORM_SVR_ITEMS_ADDED(event_id, delegated_user.id, au.id, form_id, only_last_added).ToList();
            string jdeleted = JsonConvert.SerializeObject(xdeleted);
            List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> listdeleted = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult>>(jdeleted);

            xadded = portalDMTOS.OMC_SELECT_FORM_SVR_ITEMS_ADDED(event_id, delegated_user.id, au.id, form_id, only_last_added).ToList();
            string jadded = JsonConvert.SerializeObject(xadded);
            List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> listadded = JsonConvert.DeserializeObject<List<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult>>(jadded);



            string templatePath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExcelTemplates", "EMPTY" + ".xlsx");
            Guid guid = Guid.NewGuid();
            string physicalPath = Path.Combine(_env.ContentRootPath + "\\wwwroot\\AppData\\ExportFiles", guid + ".xlsx");
            System.IO.File.Copy(templatePath, physicalPath);


            ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> excel =  new ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult>(listdeleted, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_ITEMS_DELETED", 0, null);
            excel.ExcelReport();
            ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult> excel2 = new ExcelReports<OMC_SELECT_FORM_SVR_ITEMS_ADDEDResult>(listadded, 1, 1, delegated_user.id, physicalPath, "OMC_SELECT_FORM_SVR_ITEMS_ADDED", 1, null);
            excel2.ExcelReport();
  
            if (event_id != null)
                portalDMTOS.SYS_FINISH_EVENT(event_id, physicalPath);
            return Convert.ToString("ExportFiles\\" + guid + ".xlsx");


        }




        [AppAuthorize]
        [HttpPost]
        public string ManualCommitChanges(int deleted_item_id, int added_item_id)
        {
            SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContext.Session.GetString("deleagting_user"));
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string errors = portalDMTOS.OMC_COMMIT_SVR_CHANGES_MANUAL(deleted_item_id, added_item_id, delegated_user.id, user.id).FirstOrDefault().error_description;
            return errors;
        }

    }
}