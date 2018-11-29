using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{



    public class GridSettingsLog
    {
            public string russianName { get; set; }
            public int? width { get; set; }
            public bool? visible { get; set; }
            public int? position { get; set; }
    }

public class LogObject
    {


        public int real_user_id { get; set; }

        //public string id { get; set; }
        public Settings logSettings { get; set; }
        public GridSettingsLog id { get; set; }
        public GridSettingsLog object_type { get; set; }
        public GridSettingsLog object_id { get; set; }
        public GridSettingsLog object_description { get; set; }
        public GridSettingsLog action_type { get; set; }
        public GridSettingsLog action_description { get; set; }
        public GridSettingsLog action_date { get; set; }
        public GridSettingsLog action_user_id { get; set; }
        public GridSettingsLog action_user { get; set; }
        public GridSettingsLog action_real_user_id { get; set; }
        public GridSettingsLog action_real_user { get; set; }
        public GridSettingsLog attribute { get; set; }
        public GridSettingsLog last_value { get; set; }
        public GridSettingsLog new_value { get; set; }
        public GridSettingsLog add_attr_last_value { get; set; }
        public GridSettingsLog add_attr_new_value { get; set; }


        public LogObject(int real_user_id)
        {

            PortalDMTOSModel portalDMTOS = new PortalDMTOSModel();
            List<UI_SELECT_GRID_SETTINGSResult> grid_settings = portalDMTOS.UI_SELECT_GRID_SETTINGS(real_user_id, "SYS_SELECT_OBJECT_LOG", null, 1).ToList();

            for (int i = 0; i < grid_settings.Count; i++)
            {

                grid_settings[i].global_visible = grid_settings[i].global_visible == null ? true : grid_settings[i].global_visible;
                grid_settings[i].is_visible = grid_settings[i].is_visible == null ? true : grid_settings[i].is_visible;
                grid_settings[i].global_editable = grid_settings[i].global_editable == null ? true : grid_settings[i].global_editable;

                if (grid_settings[i].field_description == "id")
                {
                    this.id = new GridSettingsLog();
                    this.id.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.id.width = grid_settings[i].width;
                    this.id.russianName = grid_settings[i].russian_field_description;
                    this.id.position = grid_settings[i].number;
                }

                if (grid_settings[i].field_description == "object_type")
                {
                    this.object_type = new GridSettingsLog();
                    this.object_type.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.object_type.width = grid_settings[i].width;
                    this.object_type.russianName = grid_settings[i].russian_field_description;
                    this.object_type.position = grid_settings[i].number;
                    
                }
                if (grid_settings[i].field_description == "object_id")
                {
                    this.object_id = new GridSettingsLog();
                    this.object_id.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.object_id.width = grid_settings[i].width;
                    this.object_id.russianName = grid_settings[i].russian_field_description;
                    this.object_id.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "object_description")
                {
                    this.object_description = new GridSettingsLog();
                    this.object_description.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.object_description.width = grid_settings[i].width;
                    this.object_description.russianName = grid_settings[i].russian_field_description;
                    this.object_description.position = grid_settings[i].number;
                }
               
                if (grid_settings[i].field_description == "action_type")
                {
                    this.action_type = new GridSettingsLog();
                    this.action_type.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_type.width = grid_settings[i].width;
                    this.action_type.russianName = grid_settings[i].russian_field_description;
                    this.action_type.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "action_description")
                {
                    this.action_description = new GridSettingsLog();
                    this.action_description.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_description.width = grid_settings[i].width;
                    this.action_description.russianName = grid_settings[i].russian_field_description;
                    this.action_description.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "action_date")
                {
                    this.action_date = new GridSettingsLog();
                    this.action_date.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_date.width = grid_settings[i].width;
                    this.action_date.russianName = grid_settings[i].russian_field_description;
                    this.action_date.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "action_user_id")
                {
                    this.action_user_id = new GridSettingsLog();
                    this.action_user_id.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_user_id.width = grid_settings[i].width;
                    this.action_user_id.russianName = grid_settings[i].russian_field_description;
                    this.action_user_id.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "action_user")
                {
                    this.action_user = new GridSettingsLog();
                    this.action_user.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_user.width = grid_settings[i].width;
                    this.action_user.russianName = grid_settings[i].russian_field_description;
                    this.action_user.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "action_real_user_id")
                {
                    this.action_real_user_id = new GridSettingsLog();
                    this.action_real_user_id.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_real_user_id.width = grid_settings[i].width;
                    this.action_real_user_id.russianName = grid_settings[i].russian_field_description;
                    this.action_real_user_id.position = grid_settings[i].number;
                }

                if (grid_settings[i].field_description == "action_real_user")
                {
                    this.action_real_user = new GridSettingsLog();
                    this.action_real_user.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.action_real_user.width = grid_settings[i].width;
                    this.action_real_user.russianName = grid_settings[i].russian_field_description;
                    this.action_real_user.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "attribute")
                {
                    this.attribute = new GridSettingsLog();
                    this.attribute.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.attribute.width = grid_settings[i].width;
                    this.attribute.russianName = grid_settings[i].russian_field_description;
                    this.attribute.position = grid_settings[i].number;
                }
                if (grid_settings[i].field_description == "last_value")
                {
                    this.last_value = new GridSettingsLog();
                    this.last_value.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.last_value.width = grid_settings[i].width;
                    this.last_value.russianName = grid_settings[i].russian_field_description;
                    this.last_value.position = grid_settings[i].number;
                }

                if (grid_settings[i].field_description == "new_value")
                {
                    this.new_value = new GridSettingsLog();
                    this.new_value.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.new_value.width = grid_settings[i].width;
                    this.new_value.russianName = grid_settings[i].russian_field_description;
                    this.new_value.position = grid_settings[i].number;
                }

                if (grid_settings[i].field_description == "add_attr_last_value")
                {
                    this.add_attr_last_value = new GridSettingsLog();
                    this.add_attr_last_value.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.add_attr_last_value.width = grid_settings[i].width;
                    this.add_attr_last_value.russianName = grid_settings[i].russian_field_description;
                    this.add_attr_last_value.position = grid_settings[i].number;
                }

                if (grid_settings[i].field_description == "add_attr_new_value")
                {
                    this.add_attr_new_value = new GridSettingsLog();
                    this.add_attr_new_value.visible = grid_settings[i].is_visible & grid_settings[i].global_visible;
                    this.add_attr_new_value.width = grid_settings[i].width;
                    this.add_attr_new_value.russianName = grid_settings[i].russian_field_description;
                    this.add_attr_new_value.position = grid_settings[i].number;
                }
            }


            this.logSettings = new Settings();
            this.logSettings.actionName = "UpdateSettingsOfGrid";
            this.logSettings.checkBoxClass = "UserSettingsCheckbox";
            this.logSettings.controllerName = "Common";
            this.logSettings.flowWindowName = "LogUserSettings";
            this.logSettings.storedProcedure = "SYS_SELECT_OBJECT_LOG";
            this.logSettings.widthClass = "UserSettingsWidth";
            this.logSettings.positionClass = "PositionClass";
            this.logSettings.gridSettings = grid_settings;
        }



    }

}
