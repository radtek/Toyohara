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
    public partial class APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult {

        public APL_SELECT_PROJECT_REQUIREMENTS_FOR_CHANGEResult()
        {
            OnCreated();
        }

        public virtual string code
        {
            get;
            set;
        }

        public virtual string object_description
        {
            get;
            set;
        }

        public virtual string subobject_description
        {
            get;
            set;
        }

        public virtual string project_documentation
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> project_documentation_date_description
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> project_documentation_receive_date_description
        {
            get;
            set;
        }

        public virtual System.Nullable<int> revision_number_description
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> revision_number_date_description
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> revision_number_receive_date_description
        {
            get;
            set;
        }

        public virtual double start_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> finish_description
        {
            get;
            set;
        }

        public virtual string station_description
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual System.Nullable<int> additional_properties
        {
            get;
            set;
        }

        public virtual string package_contents
        {
            get;
            set;
        }

        public virtual string unit_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> quantity_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> mass_per_unit_description
        {
            get;
            set;
        }

        public virtual string mass_size
        {
            get;
            set;
        }

        public virtual string manufacturer_description
        {
            get;
            set;
        }

        public virtual string delivery_type_description
        {
            get;
            set;
        }

        public virtual string goods_type_description
        {
            get;
            set;
        }

        public virtual string summary
        {
            get;
            set;
        }

        public virtual string summary2
        {
            get;
            set;
        }

        public virtual string state_description
        {
            get;
            set;
        }

        public virtual string package_description
        {
            get;
            set;
        }

        public virtual string in_kd_description
        {
            get;
            set;
        }

        public virtual string psd_state_description
        {
            get;
            set;
        }

        public virtual string pr_source_description
        {
            get;
            set;
        }

        public virtual string subcontractor_description
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
