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
    public partial class APL_SELECT_PROJECTS_LOGISTICAL_NETWORK_OBJECTSResult {

        public APL_SELECT_PROJECTS_LOGISTICAL_NETWORK_OBJECTSResult()
        {
            OnCreated();
        }

        public virtual System.Nullable<int> row_count
        {
            get;
            set;
        }

        public virtual System.Nullable<long> rank
        {
            get;
            set;
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string logistical_network_object
        {
            get;
            set;
        }

        public virtual System.Nullable<double> project_logistical_network_object_start
        {
            get;
            set;
        }

        public virtual System.Nullable<double> project_logistical_network_object_finish
        {
            get;
            set;
        }

        public virtual string receiver_description
        {
            get;
            set;
        }

        public virtual string project_descrtiption
        {
            get;
            set;
        }

        public virtual string @checked
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}