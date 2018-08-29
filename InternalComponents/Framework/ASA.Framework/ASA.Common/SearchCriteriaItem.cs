///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	QueryListCriteria.cs
//
//  Description:
//  This file contains all required code for creating the criteria used to 
//  retrieve data from the DB.
//  
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Common
{
    public class SearchCriteriaItem : CriterionBase
    {

        /// <summary>
        /// Use this constructor to give fieldname, value and the criteria type
        /// other than Between, Like and IN
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="fieldValue">field value</param>
        /// <param name="critType">criteria type</param>
        public SearchCriteriaItem(String fieldName, Object fieldValue, criteriaType critType, bool isParentObjectField)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._critTypeField = critType;
            this._isParentObjectField = isParentObjectField;
            this._subObjectTypeField = "";
        }

        public SearchCriteriaItem(String fieldName, Object fieldValue, criteriaType critType, bool isParentObjectField, string subObjectType)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._critTypeField = critType;
            this._isParentObjectField = isParentObjectField;
            this._subObjectTypeField = subObjectType;
        }

       

      

        /// <summary>
        /// criteria type
        /// </summary>
        private criteriaType _critTypeField;
        public criteriaType CriteriaType
        {
            get { return this._critTypeField; }
            set { this._critTypeField = value; }
        }

        
    }
}
