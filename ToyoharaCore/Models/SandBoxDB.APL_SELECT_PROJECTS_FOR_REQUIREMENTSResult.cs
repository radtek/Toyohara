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
    public partial class APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult {

        public APL_SELECT_PROJECTS_FOR_REQUIREMENTSResult()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string project_code
        {
            get;
            set;
        }

        public virtual string project_description
        {
            get;
            set;
        }

        public virtual string project_type
        {
            get;
            set;
        }

        public virtual string project_state
        {
            get;
            set;
        }

        public virtual System.Nullable<int> cnt_total
        {
            get;
            set;
        }

        public virtual System.Nullable<int> unsi_declined
        {
            get;
            set;
        }

        public virtual System.Nullable<int> dmtos_declined
        {
            get;
            set;
        }

        public virtual System.Nullable<int> not_in_kv
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}