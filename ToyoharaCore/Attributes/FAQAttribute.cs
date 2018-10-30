using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ToyoharaCore.Attributes
{
    sealed public class FAQAttribute : ActionFilterAttribute
    {

        SYS_AUTHORIZE_USERResult au;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {   
            //filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R");
            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();


            if (filterContext.HttpContext.Session.Keys.Contains("SYS_AUTHORIZE_USER2_R"))
            {
                APL_SELECT_PROJECT_STATES_FOR_DDResult delegated_user = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(filterContext.HttpContext.Session.GetString("deleagting_user"));
                //filterContext.HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(portalDMTOS.UI_SELECT_LINK_PAGE_NOTE(filterContext.RouteData.Values["Controller"].ToString(), filterContext.RouteData.Values["Action"].ToString(), delegated_user.id).FirstOrDefault().http_text));



                SYS_AUTHORIZE_USERResult user = JsonConvert.DeserializeObject<SYS_AUTHORIZE_USERResult>(filterContext.HttpContext.Session.GetString("SYS_AUTHORIZE_USER2_R"));
                filterContext.HttpContext.Session.SetString("message_panel", JsonConvert.SerializeObject(portalDMTOS.SYS_SELECT_MESSAGES(user.id, delegated_user.id).FirstOrDefault().description));
                UI_SELECT_LINKResult link_info=null;
                //UI_SELECT_LINKResult link_info = JsonConvert.DeserializeObject<UI_SELECT_LINKResult>(HttpContextAccessor.HttpContext.Session.GetString("link_info"));
                string action = filterContext.RouteData.Values["Action"].ToString();
                string controller = filterContext.RouteData.Values["Controller"].ToString();
                string link_information_param = null;
                if (filterContext.ActionArguments.Any(x => x.Key == "link_information_param"))
                    link_information_param = Convert.ToString(filterContext.ActionArguments["link_information_param"]);
                if (filterContext.RouteData.Values["Action"]!=null && filterContext.RouteData.Values["Controller"]!=null)
                 link_info = portalDMTOS.UI_SELECT_LINK(action, controller, link_information_param).FirstOrDefault();
                if (link_info == null)
                    link_info = portalDMTOS.UI_SELECT_LINK("Index", "Home", link_information_param).FirstOrDefault();
                filterContext.HttpContext.Session.SetString("link_info", JsonConvert.SerializeObject(link_info));
                var link_page_note=portalDMTOS.UI_SELECT_LINK_PAGE_NOTE2(link_info.id, delegated_user.id, user.id).FirstOrDefault().http_text;
                filterContext.HttpContext.Session.SetString("FAQ", JsonConvert.SerializeObject(link_page_note));

                //APL_SELECT_PROJECT_STATES_FOR_DDResult SYS_SELECT_ROLES_FOR_DD = JsonConvert.DeserializeObject<APL_SELECT_PROJECT_STATES_FOR_DDResult>(HttpContextAccessor.HttpContext.Session.GetString("SYS_SELECT_ROLES_FOR_DD"));
                List<APL_SELECT_PROJECT_STATES_FOR_DDResult> SYS_SELECT_ROLES_FOR_DD = portalDMTOS.SYS_SELECT_ROLES_FOR_DD(delegated_user.id).ToList();
                filterContext.HttpContext.Session.SetString("SYS_SELECT_ROLES_FOR_DD", JsonConvert.SerializeObject(SYS_SELECT_ROLES_FOR_DD));
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
