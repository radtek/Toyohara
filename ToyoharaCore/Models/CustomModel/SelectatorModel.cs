using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{
    public class SelectatorModel
    {   public SelectatorModel(List<APL_SELECT_PROJECT_STATES_FOR_DDResult> SelectatorList, string SelectatorId, string SelectatorClass, string Multiple) {
            this.SelectatorList = SelectatorList;
            this.SelectatorId = SelectatorId;
            this.SelectatorClass = SelectatorClass;
            this.Multiple = Multiple;
        }
        public List<APL_SELECT_PROJECT_STATES_FOR_DDResult> SelectatorList { get; set; }
        public string SelectatorId { get; set; }
        public string SelectatorClass { get; set; }
        public string Multiple { get; set; }
    }
}
