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
    public partial class OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult {

        public OMC_REPORT_FORM_RSS_DELIVERY_TYPE_DIFFERENCEResult()
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

        public virtual string subcontractor_description
        {
            get;
            set;
        }

        public virtual string lno_description
        {
            get;
            set;
        }

        public virtual string project_documentation_code
        {
            get;
            set;
        }

        public virtual string ekk_code
        {
            get;
            set;
        }

        public virtual string description
        {
            get;
            set;
        }

        public virtual string additional_properties
        {
            get;
            set;
        }

        public virtual string package_contents
        {
            get;
            set;
        }

        public virtual string manufacturer
        {
            get;
            set;
        }

        public virtual string unit_description
        {
            get;
            set;
        }

        public virtual System.Nullable<double> quantity
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

        public virtual string rss_delivery_type
        {
            get;
            set;
        }

        public virtual string suggested_delivery_type
        {
            get;
            set;
        }

        public virtual string delivery_type
        {
            get;
            set;
        }
    
        #region Extensibility Method Definitions

        partial void OnCreated();
        
        #endregion
    }

}