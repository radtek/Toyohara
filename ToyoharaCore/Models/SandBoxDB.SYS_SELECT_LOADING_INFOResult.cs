﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 29.10.2018 9:10:23
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
    public partial class SYS_SELECT_LOADING_INFOResult {

        public SYS_SELECT_LOADING_INFOResult()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual System.Nullable<int> loading_state_id
        {
            get;
            set;
        }

        public virtual string loading_state_description
        {
            get;
            set;
        }

        public virtual int allow_commit
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual System.Nullable<int> total_cnt
        {
            get;
            set;
        }

        public virtual System.Nullable<int> error_cnt
        {
            get;
            set;
        }

        public virtual System.Nullable<int> warning_cnt
        {
            get;
            set;
        }

        public virtual string info_html
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
