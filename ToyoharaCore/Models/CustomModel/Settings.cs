using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToyoharaCore.Models.CustomModel
{
    public class Settings
    {
        //public List<GridSettings> gridSettings { get; set; }
        public List<UI_SELECT_GRID_SETTINGSResult> gridSettings { get; set; }

        public string flowWindowName { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public string storedProcedure { get; set; }
        public string checkBoxClass { get; set; }
        public string widthClass { get; set; }
        public string positionClass { get; set; }
        public string parsialDivName { get; set; }
        public string openParsialDivFunction { get; set; }

        public Settings(List<UI_SELECT_GRID_SETTINGSResult> gridSettings, string flowWindowName, string controllerName,
                        string actionName, string storedProcedure, string checkBoxClass, string widthClass, string positionClass, string parsialDivName="", string openParsialDivFunction="")
        {   
            this.gridSettings = gridSettings;
            this.flowWindowName = flowWindowName;
            this.controllerName = controllerName;
            this.actionName = actionName;
            this.storedProcedure = storedProcedure;
            this.checkBoxClass = checkBoxClass;
            this.widthClass = widthClass;
            this.positionClass = positionClass;
            this.parsialDivName = parsialDivName;
            this.openParsialDivFunction = openParsialDivFunction;
        }
        
        public Settings() { gridSettings = new List<UI_SELECT_GRID_SETTINGSResult>(); }

    }
}