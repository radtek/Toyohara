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
    public partial class SYS_SELECT_DELEGATING_USERResult {

        public SYS_SELECT_DELEGATING_USERResult()
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

        public virtual string login
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}