﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 29.11.2018 17:43:28
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;

namespace ToyoharaCore
{
    public partial class OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMSResult {

        public OMC_SELECT_FORM_SVR_DELIVERY_TYPE_LOADING_ITEMSResult()
        {
            OnCreated();
        }

        public virtual string item_code
        {
            get;
            set;
        }

        public virtual string phase
        {
            get;
            set;
        }

        public virtual string local_estimate_number
        {
            get;
            set;
        }

        public virtual string project_documentation_code
        {
            get;
            set;
        }

        public virtual string number
        {
            get;
            set;
        }

        public virtual string resource_code
        {
            get;
            set;
        }

        public virtual string ekk_code
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual string unit_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> quantity
        {
            get;
            set;
        }

        public virtual string add_info
        {
            get;
            set;
        }

        public virtual string svr_delivery_type
        {
            get;
            set;
        }

        public virtual string suggested_delivery_type_description
        {
            get;
            set;
        }

        public virtual string delivery_type_1
        {
            get;
            set;
        }

        public virtual string delivery_type_2
        {
            get;
            set;
        }

        public virtual string delivery_type_3
        {
            get;
            set;
        }

        public virtual string delivery_type_4
        {
            get;
            set;
        }

        public virtual string delivery_type_5
        {
            get;
            set;
        }

        public virtual string delivery_type_6
        {
            get;
            set;
        }

        public virtual string delivery_type_7
        {
            get;
            set;
        }

        public virtual string delivery_type_8
        {
            get;
            set;
        }

        public virtual string delivery_type_9
        {
            get;
            set;
        }

        public virtual string error_description
        {
            get;
            set;
        }

        public virtual string warning_description
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
