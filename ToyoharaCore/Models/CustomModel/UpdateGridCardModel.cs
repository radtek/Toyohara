using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{
    public class UpdateGridCardModel
    {   public UpdateGridCardModel(string FlowWindowName, List<UI_SELECT_GRID_SETTINGSResult> GridSettings, string FlowWindowRussianName, string GridId,
        bool Binding, bool Close_window, string StoredProcedure, string Id_func, List<ProcedureParam> AdditionalParams) {
            this.FlowWindowName = FlowWindowName;
            this.GridSettings = GridSettings;
            this.FlowWindowRussianName = FlowWindowRussianName;
            this.GridId = GridId;
            this.Bindning = Bindning;
            this.Close_window = Close_window;
            this.StoredProcedure = StoredProcedure;
            this.Id_func = Id_func;
            this.AdditionalParams = AdditionalParams;
        }
        public List<ProcedureParam> AdditionalParams { get; set; }
        public string FlowWindowName { get; set; }
        public List<UI_SELECT_GRID_SETTINGSResult> GridSettings { get; set; }
        public string FlowWindowRussianName { get; set; }
        public string GridId { get; set; }
        public bool Bindning { get; set; }
        public bool Close_window { get; set; }
        public string StoredProcedure { get; set; }
        public string Id_func { get; set; }
    }
}
