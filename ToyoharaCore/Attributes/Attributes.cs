using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ToyoharaCore.Attributes
{
    sealed public class AppAuthorizeAttribute : ActionFilterAttribute
    {

        SYS_AUTHORIZE_USERResult au;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {   
            //filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R");
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            string Host = System.Net.Dns.GetHostName();
            //filterContext.HttpContext.Session["system_message"] = oSandBoxDBEntities.SYS_SELECT_MESSAGES(null, null).FirstOrDefault();  



            if (filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R") == null)
            //if (filterContext.HttpContext.Request.Cookies["SYS_AUTHORIZE_USER2_R"] == null)
            {
                
                if (filterContext.HttpContext.User == null)
                {
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Denied", action = "Index2" }));                   
                    
                    //HttpContext.Current.Response.Redirect("/Denied/Index2");
                   // filterContext.HttpContext.Response.Redirect("");
                    filterContext.Result= new RedirectResult("/Home/Denied?username=");

                }
                else
                {

                    string Hosts = System.Net.Dns.GetHostName();
                    au = portalDMTOS.SYS_AUTHORIZE_USER(filterContext.HttpContext.User.Identity.Name, Hosts, Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"])).FirstOrDefault();
//||||||| .r426
//                     au = portalDMTOS.SYS_AUTHORIZE_USER(filterContext.HttpContext.User.Identity.Name, null, Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"])).FirstOrDefault();
////=======
//                     au = portalDMTOS.SYS_AUTHORIZE_USER(filterContext.HttpContext.User.Identity.Name, Hosts, Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"])).FirstOrDefault();
//>>>>>>> .r428

                    //SYS_AUTHORIZE_USER2Result au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2(filterContext.HttpContext.User.Identity.Name, null, Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"])).FirstOrDefault();

                    //SYS_AUTHORIZE_USER2Result au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2("OOOSGM\\kordemskaya_lo", null, Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"])).FirstOrDefault();
                    // SYS_AUTHORIZE_USER2Result au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2(filterContext.HttpContext.User.Identity.Name,au.delegating_user_id, null).FirstOrDefault();
                    //SYS_AUTHORIZE_USER2_R au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2("OOOSGM\\abasova_kr", null).FirstOrDefault();
                    //SYS_AUTHORIZE_USER2_R au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2("OOOSGM\\volchihina_ua", null).FirstOrDefault();
                    //SYS_AUTHORIZE_USER2_R au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2("OOOSGM\\zhukov_vs", null).FirstOrDefault();
                    //SYS_AUTHORIZE_USER2_R au = oSandBoxDBEntities.SYS_AUTHORIZE_USER2("OOOSGM\\suhoivanova_lg", null).FirstOrDefault();
                    
                    if (au != null && au.not_in_SGM == true)
                    {
                        //HttpContext.Current.Response.Redirect("/Denied/Index?username=" + filterContext.HttpContext.User.Identity.Name);
                        filterContext.Result = new RedirectResult("/Home/Denied?username=" + filterContext.HttpContext.User.Identity.Name);
                    }
                    else
                    {

                        if (filterContext.HttpContext.User.Identity.Name == null)
                        {
                            // filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Denied", action = "Index2" }));
                           // HttpContext.Current.Response.Redirect("/Denied/Index2");
                            filterContext.Result = new RedirectResult("/Home/Denied?username=");
                        }

                        else if (au != null)
                        {
                            List<APL_SELECT_PROJECT_STATES_FOR_DDResult> sduc = portalDMTOS.SYS_SELECT_DELEGATING_USERS2(au.id);
                           // filterContext.HttpContext.Response.Cookies.Append("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au.delegating_user_id));//, new CookieOptions() { HttpOnly = false });
                          //  filterContext.HttpContext.Response.Cookies.Append("SYS_SELECT_DELEGATING_USERS_R", Convert.ToString(JsonConvert.SerializeObject(sduc)));//, new CookieOptions() { HttpOnly = false });

                            filterContext.HttpContext.Session.SetString("SYS_AUTHORIZE_USER2_R", JsonConvert.SerializeObject(au)) ;
                            filterContext.HttpContext.Session.SetString("SYS_SELECT_DELEGATING_USERS_R",JsonConvert.SerializeObject(sduc));
                            filterContext.HttpContext.Session.SetString("deleagting_user", JsonConvert.SerializeObject(sduc.Where(x => x.id == au.id).FirstOrDefault()));
                            List<UI_SELECT_SITE_MENUResult> site_map = portalDMTOS.UI_SELECT_SITE_MENU(au.id).ToList();
                            filterContext.HttpContext.Session.SetString("site_map", JsonConvert.SerializeObject(site_map));
                            bool admin = portalDMTOS.SYS_SELECT_ROLES_BY_USER(sduc.Where(x => x.id == au.id).FirstOrDefault().id).ToList().Any(x => x.role_id == 1);

                            filterContext.HttpContext.Session.SetString("admin_role", JsonConvert.SerializeObject(admin));
                        }
                        else
                        {
                            //  filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Denied", action = "Index" }));
                           // HttpContext.Current.Response.Redirect("/Denied/Index?username=" + filterContext.HttpContext.User.Identity.Name);
                            filterContext.Result = new RedirectResult("/Home/Denied?username=" + filterContext.HttpContext.User.Identity.Name);
                        }
                    }
                }
            }

            //SYS_AUTHORIZE_USER2Result au2 = (SYS_AUTHORIZE_USER2Result)filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R");
            //SYS_AUTHORIZE_USER2Result au2 = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USER2Result>( filterContext.HttpContext.Request.Cookies["SYS_AUTHORIZE_USER2_R"]);
            //filterContext.HttpContext.Session.SetString("system_message", JsonConvert.SerializeObject(oSandBoxDBEntities.SYS_SELECT_MESSAGES(user.delegating_user_id, user.id).FirstOrDefault()));
            // filterContext.HttpContext.Response.Cookies.Append("system_message", Convert.ToString(JsonConvert.SerializeObject(oSandBoxDBEntities.SYS_SELECT_MESSAGES(user.delegating_user_id, user.id).FirstOrDefault())));

            //if (au != null | filterContext.HttpContext.Session.Keys.Contains("SYS_AUTHORIZE_USER2_R"))
            //{
            //    APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(filterContext.HttpContext.Session.GetString("deleagting_user"));
            //    //filterContext.HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE(filterContext.RouteData.Values["Controller"].ToString(), filterContext.RouteData.Values["Action"].ToString(), delegated_user.id).FirstOrDefault().http_text));



            //    SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
            //    filterContext.HttpContext.Session.SetString("message_panel", JsonConvert.SerializeObject(portalDMTOS.SYS_SELECT_MESSAGES(user.id, delegated_user.id).FirstOrDefault().description));
            //    UI_SELECT_LINKResult link_info=null;
            //    //UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContextAccessor.HttpContext.Session.GetString("link_info"));
            //    if (filterContext.RouteData.Values["Action"].ToString()!=null && filterContext.RouteData.Values["Controller"].ToString()!=null)
            //     link_info = portalDMTOS.UI_SELECT_LINK(filterContext.RouteData.Values["Action"].ToString(), filterContext.RouteData.Values["Controller"].ToString()).FirstOrDefault();
            //    if (link_info == null)
            //        link_info = portalDMTOS.UI_SELECT_LINK("Index", "Home").FirstOrDefault();
            //    filterContext.HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
            //    filterContext.HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, delegated_user.id, user.id).FirstOrDefault().http_text));

            //    //APL_SELECT_PROJECT_STATES_FOR_DDResult SYS_SELECT_ROLES_FOR_DD = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContextAccessor.HttpContext.Session.GetString("SYS_SELECT_ROLES_FOR_DD"));
            //    List<APL_SELECT_PROJECT_STATES_FOR_DDResult> SYS_SELECT_ROLES_FOR_DD = portalDMTOS.SYS_SELECT_ROLES_FOR_DD(delegated_user.id).ToList();
            //    filterContext.HttpContext.Session.SetString("SYS_SELECT_ROLES_FOR_DD", JsonConvert.SerializeObject(SYS_SELECT_ROLES_FOR_DD));
            //}
            base.OnActionExecuting(filterContext);
        }

    }
}
