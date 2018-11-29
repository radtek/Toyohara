using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToyoharaCore.Models;

namespace ToyoharaCore.Models.CustomModel
{
    public class CommonMethods
    {
        //public static ViewDataDictionary ReturnSettings(List<UI_SELECT_GRID_SETTINGSResult> grid_settings, string add_to_end) {
        //    ViewDataDictionary viewDataDict;
        //    for (int i = 0; i < grid_settings.Count; i++)
        //    {
        //        if (grid_settings[i].ui_type == "dropdown_list_id" | grid_settings[i].ui_type == "dropdown_id" | grid_settings[i].ui_type == "dropdown_txt_id" | grid_settings[i].ui_type == "dropdown_txt_list_id")
        //        {
        //            if (Convert.ToBoolean(grid_settings[i].global_editable))
        //            {
        //                grid_settings[i].global_visible = true;
        //                grid_settings[i].is_visible = true;

        //                var description = grid_settings.Where(x => x.ui_type != grid_settings[i].ui_type & x.dropdown == grid_settings[i].dropdown).FirstOrDefault();
        //                description.is_visible = false;
        //                description.global_visible = false;
        //                grid_settings[i].russian_field_description = description.russian_field_description;
        //            }
        //        }
        //        grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
        //        grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
        //        grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;
        //        viewDataDict.Add(new KeyValuePair<string, object>("CK_UI_" + grid_settings[i].field_description + "_grid_settings3", grid_settings[i].is_visible & grid_settings[i].global_visible))


        //        ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings3"] = grid_settings[i].is_visible & grid_settings[i].global_visible;
        //        ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings3" + "_width"] = grid_settings[i].width;
        //        ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings3" + "_ru"] = grid_settings[i].russian_field_description;
        //        ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings3" + "_pos"] = grid_settings[i].number;
        //        ViewData["CK_UI_" + grid_settings[i].field_description + "_grid_settings3" + "_edit"] = grid_settings[i].global_editable;
        //    }
        //    ViewDataDictionary keyValuePairs = new ViewDataDictionary(keyValuePair);

        //}
        public static string ObjectToString(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        public static string HtmlToText(string htmlString)
        {
            if (htmlString == null) return String.Empty;
            char[] charArr = htmlString.ToCharArray();
            char[] result = { };
            Array.Resize(ref result, charArr.Length);
            bool CopyFlag = true;
            int j = 0;
            for (int i = 0; i < charArr.Length; i++)
            {
                if (charArr[i] == '<')
                {
                    CopyFlag = false;
                }
                if (CopyFlag) { result[j] = charArr[i]; j++; }
                if (charArr[i] == '>')
                {
                    CopyFlag = true;
                }
            }
            Array.Resize(ref result, j);

            return new string(result);
        }
    }
}
