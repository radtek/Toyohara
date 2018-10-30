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
    public class HomeController : Controller
    {



        [AppAuthorizeAttribute]
        [FAQAttribute]
        public IActionResult Index(int? id)
        {
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();

            if (id != null)
                {
                    SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));

                    UI_SELECT_LINKResult link_info = new UI_SELECT_LINKResult { id = 0, description = "" };

                    HttpContext.Session.SetString("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au));
                    List<APL_SELECT_PROJECT_STATES_FOR_DDResult> sduc = portalDMTOS.SYS_SELECT_DELEGATING_USERS2(au.id).ToList<APL_SELECT_PROJECT_STATES_FOR_DDResult>();
                    HttpContext.Session.SetString("deleagting_user", JsonConvert.SerializeObject(sduc.Where(x => x.id == id).FirstOrDefault()));
                    HttpContext.Session.SetString("SYS_SELECT_DELEGATING_USERS_R", JsonConvert.SerializeObject(sduc));
                    List<UI_SELECT_SITE_MENUResult> site_map = portalDMTOS.UI_SELECT_SITE_MENU(id).ToList();
                    HttpContext.Session.SetString("site_map", JsonConvert.SerializeObject(site_map));
                    bool admin = portalDMTOS.SYS_SELECT_ROLES_BY_USER(sduc.Where(x => x.id == id).FirstOrDefault().id).ToList().Any(x => x.role_id == 1);
                    HttpContext.Session.SetString("admin_role", JsonConvert.SerializeObject(admin));
                    link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
                    HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, id, au.id).FirstOrDefault().http_text));
                    //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
                    //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));
                    HttpContext.Session.SetString("message_panel", JsonConvert.SerializeObject(portalDMTOS.SYS_SELECT_MESSAGES(id, au.id).FirstOrDefault().description));


                    ViewBag.SYS_SELECT_ROLES_FOR_DD = portalDMTOS.SYS_SELECT_ROLES_FOR_DD(id); ViewBag.Message = "Вы действуете в системе от имени другого пользователя!";

                }

                else
                { ViewBag.Message = "Добро пожаловать!"; //HttpContext.Session.Clear();

                UI_SELECT_LINKResult link_info = new UI_SELECT_LINKResult { id = 0, description = "" };
                link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
                SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
                List<APL_SELECT_PROJECT_STATES_FOR_DDResult> SYS_SELECT_ROLES_FOR_DD = JsonConvert.DeserializeObject<List<APL_SELECT_PROJECT_STATES_FOR_DDResult>>(HttpContext.Session.GetString("SYS_SELECT_ROLES_FOR_DD"));

                HttpContext.Session.Clear();
                List<APL_SELECT_PROJECT_STATES_FOR_DDResult> sduc = portalDMTOS.SYS_SELECT_DELEGATING_USERS2(au.id).ToList();

                HttpContext.Session.SetString("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au));
                HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
                HttpContext.Session.SetString("SYS_SELECT_ROLES_FOR_DD", JsonConvert.SerializeObject(SYS_SELECT_ROLES_FOR_DD));


                HttpContext.Session.SetString("deleagting_user", JsonConvert.SerializeObject(sduc.Where(x => x.id == au.id).FirstOrDefault()));
                HttpContext.Session.SetString("SYS_SELECT_DELEGATING_USERS_R", JsonConvert.SerializeObject(sduc));
                List<UI_SELECT_SITE_MENUResult> site_map = portalDMTOS.UI_SELECT_SITE_MENU(au.id).ToList();
                HttpContext.Session.SetString("site_map", JsonConvert.SerializeObject(site_map));
                bool admin = portalDMTOS.SYS_SELECT_ROLES_BY_USER(sduc.Where(x => x.id == au.id).FirstOrDefault().id).ToList().Any(x => x.role_id == 1);
                HttpContext.Session.SetString("admin_role", JsonConvert.SerializeObject(admin));
                HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, au.id, au.id).FirstOrDefault().http_text));
                //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
                //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));
                HttpContext.Session.SetString("message_panel", JsonConvert.SerializeObject(portalDMTOS.SYS_SELECT_MESSAGES(au.id, au.id).FirstOrDefault().description));

            }

            return View();
        }

        //[AppAuthorizeAttribute]
        //[FAQAttribute]
        //public ActionResult Delegate(int? id)
        //{
        //    SYS_AUTHORIZE_USERResult au = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));

        //    PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
        //    UI_SELECT_LINKResult link_info = new UI_SELECT_LINKResult { id = 0, description = "" };
        //    if (id != null)
        //    {
        //        HttpContext.Session.SetString("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au));
        //        List<APL_SELECT_PROJECT_STATES_FOR_DDResult> sduc = portalDMTOS.SYS_SELECT_DELEGATING_USERS2(au.id).ToList<APL_SELECT_PROJECT_STATES_FOR_DDResult>();
        //        HttpContext.Session.SetString("deleagting_user", JsonConvert.SerializeObject(sduc.Where(x => x.id == id).FirstOrDefault()));
        //        HttpContext.Session.SetString("SYS_SELECT_DELEGATING_USERS_R", JsonConvert.SerializeObject(sduc));
        //        List<UI_SELECT_SITE_MENUResult> site_map = portalDMTOS.UI_SELECT_SITE_MENU(id).ToList();
        //        HttpContext.Session.SetString("site_map", JsonConvert.SerializeObject(site_map));
        //        bool admin = portalDMTOS.SYS_SELECT_ROLES_BY_USER(sduc.Where(x => x.id == id).FirstOrDefault().id).ToList().Any(x => x.role_id == 1);
        //        HttpContext.Session.SetString("admin_role", JsonConvert.SerializeObject(admin));
        //        link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContext.Session.GetString("link_info"));
        //        HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, id, au.id).FirstOrDefault().http_text));
        //        //HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
        //        //HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, user_id).FirstOrDefault().http_text));
        //        HttpContext.Session.SetString("message_panel", JsonConvert.SerializeObject(portalDMTOS.SYS_SELECT_MESSAGES(id, au.id).FirstOrDefault().description));
        //    }

        //    ViewBag.SYS_SELECT_ROLES_FOR_DD = portalDMTOS.SYS_SELECT_ROLES_FOR_DD(id);
        //    // ViewBag.FAQ = portalDMTOS.UI_SELECT_LINK_PAGE_NOTE(this.ToString(), "Index", id).FirstOrDefault().http_text; 
        //    ViewBag.FAQ = portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, id, au.id).FirstOrDefault().http_text;
        //    return View();
        //}

        public ActionResult SignOut()
        {
            //Microsoft.AspNetCore.Identity
            //HttpContext.Session.Remove("SYS_AUTHORIZE_USER2_R");
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Denied(string username)
        {
            ViewData["info"] = "Вы не имеете доступа к системе. Для доступа необходимо оформить СЗ: <a href='/../../AppData/Templates/WordTemplates/Шаблон СЗ о предоставлении доступа к порталу ДМТОС.doc'>Cкачать шаблон СЗ</a>";
            return View();
        }
    }
}
