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
    public partial class PRC_SELECT_ORDER_ITEMS_GKIResult {

        public PRC_SELECT_ORDER_ITEMS_GKIResult()
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

        public virtual string item_description
        {
            get;
            set;
        }

        public virtual string item_additional_properties
        {
            get;
            set;
        }

        public virtual int order_code
        {
            get;
            set;
        }

        public virtual string order_number
        {
            get;
            set;
        }

        public virtual string supply_manager
        {
            get;
            set;
        }

        public virtual string order_item_note
        {
            get;
            set;
        }

        public virtual System.DateTime order_item_modification_date
        {
            get;
            set;
        }

        public virtual string gki_code
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> gki_code_date
        {
            get;
            set;
        }

        public virtual string gki_order_number
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> gki_order_date
        {
            get;
            set;
        }

        public virtual System.Nullable<int> gki_state_id
        {
            get;
            set;
        }

        public virtual string gki_state
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> gki_state_date
        {
            get;
            set;
        }

        public virtual string note
        {
            get;
            set;
        }

        public virtual string gki_user
        {
            get;
            set;
        }

        public virtual string color
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}
