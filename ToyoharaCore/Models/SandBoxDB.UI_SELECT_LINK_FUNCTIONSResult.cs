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
    public partial class UI_SELECT_LINK_FUNCTIONSResult {

        public UI_SELECT_LINK_FUNCTIONSResult()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual System.Nullable<int> section_id
        {
            get;
            set;
        }

        public virtual string code
        {
            get;
            set;
        }

        public virtual string section_description
        {
            get;
            set;
        }

        public virtual string title
        {
            get;
            set;
        }

        public virtual string stored_procedure
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual string modification_user
        {
            get;
            set;
        }

        public virtual string modification_date
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
