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
    public partial class APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result {

        public APL_SELECT_PROJECT_REQUIREMENT_CHANGE_REQUESTS2Result()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string code
        {
            get;
            set;
        }

        public virtual string project_description
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

        public virtual System.Nullable<int> revision_number
        {
            get;
            set;
        }

        public virtual string inventory_object_description
        {
            get;
            set;
        }

        public virtual string inventory_object_package_contents
        {
            get;
            set;
        }

        public virtual string inventory_object_additional_properties
        {
            get;
            set;
        }

        public virtual string inventory_object_manufacturer_description
        {
            get;
            set;
        }

        public virtual string unit
        {
            get;
            set;
        }

        public virtual string quantity
        {
            get;
            set;
        }

        public virtual string state
        {
            get;
            set;
        }

        public virtual string station
        {
            get;
            set;
        }

        public virtual string request_date
        {
            get;
            set;
        }

        public virtual string request_user
        {
            get;
            set;
        }

        public virtual string approve_date
        {
            get;
            set;
        }

        public virtual string approve_user
        {
            get;
            set;
        }

        public virtual string approve_state
        {
            get;
            set;
        }

        public virtual string approve_note
        {
            get;
            set;
        }

        public virtual string order_information
        {
            get;
            set;
        }

        public virtual string specification_information
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

        public virtual int is_not_for_approve
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
