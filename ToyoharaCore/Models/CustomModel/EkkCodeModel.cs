using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToyoharaCore.Models.CustomModel
{
    public class EkkCodeModel
    {
        public EkkCodeModel( List<MDM_SELECT_INVENTORY_CLASSES_FOR_GRAPHResult> Tree, string FlowWindowName, string EkkCodeTextId, bool HiddenFlag, string FullSearchName) {
            this.Tree = Tree;
            this.FlowWindowName = FlowWindowName;
            this.EkkCodeTextId = EkkCodeTextId;
            this.HiddenFlag = HiddenFlag;
            this.FullSearchName = FullSearchName;
        }
        public List<MDM_SELECT_INVENTORY_CLASSES_FOR_GRAPHResult> Tree {get;set;}
        public string FlowWindowName { get; set; }
        public string EkkCodeTextId { get; set; }
        public bool HiddenFlag { get; set; }
        public string FullSearchName { get; set; }


    }
}
