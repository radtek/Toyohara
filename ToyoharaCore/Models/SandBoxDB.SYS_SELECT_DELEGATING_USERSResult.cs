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
    public partial class SYS_SELECT_DELEGATING_USERSResult {

        public SYS_SELECT_DELEGATING_USERSResult()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual string short_description
        {
            get;
            set;
        }

        public virtual string login
        {
            get;
            set;
        }

        public virtual string C_code
        {
            get;
            set;
        }

        public virtual System.Nullable<int> division_id
        {
            get;
            set;
        }

        public virtual string position
        {
            get;
            set;
        }

        public virtual string mail
        {
            get;
            set;
        }

        public virtual string phone
        {
            get;
            set;
        }

        public virtual string short_description_reverse
        {
            get;
            set;
        }

        public virtual bool is_locked
        {
            get;
            set;
        }

        public virtual System.DateTime creation_date
        {
            get;
            set;
        }

        public virtual System.DateTime modification_date
        {
            get;
            set;
        }

        public virtual System.Nullable<bool> not_in_SGM
        {
            get;
            set;
        }

        public virtual int type
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
