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
    public partial class APL_SELECT_OBJECT_SUBCONTRACTORSResult {

        public APL_SELECT_OBJECT_SUBCONTRACTORSResult()
        {
            OnCreated();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual int subcontractor_id
        {
            get;
            set;
        }

        public virtual string subcontractor_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> start
        {
            get;
            set;
        }

        public virtual System.Nullable<double> finish
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}