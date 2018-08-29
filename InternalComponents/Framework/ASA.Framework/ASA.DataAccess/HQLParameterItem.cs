///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	HQLParameterItem.cs
//
//  Description:
//  This file contains all required classes, structs and enums for using HQL in NHibernate
//  
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;

namespace ASA.DataAccess
{

    /// <summary>
    /// This class is to gather the necessary info to build out a proper HQL string for use with
    /// NHibernate
    /// </summary>
    [CLSCompliant(false)]
    public class HQLParameterItem : CriteriaItemBase
    {
        #region Member variables

        private criteriaType _searchTypeField;
        private String _subObjectTypeField;
        private String _logicalOperator;

        #endregion
                
        public HQLParameterItem(String fieldName, Object fieldValue, criteriaType searchType, string logicalOperator, string subObjectType)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._searchTypeField = searchType;
            this._logicalOperator = logicalOperator;
            this._subObjectTypeField = subObjectType;

            //if Like search, set value based on type of search
            switch (searchType)
            {
                case criteriaType.Like:
                case criteriaType.LikeNoCase:
                    this._fieldValueField = "%" + fieldValue + "%";
                    break;

                case criteriaType.LikeEndsWith:
                    this._fieldValueField = "%" + fieldValue;
                    break;

                case criteriaType.LikeStartsWith:
                    this._fieldValueField = fieldValue + "%";
                    break;
            }
        }

        /// <summary>
        /// search type
        /// </summary>
        public criteriaType SearchType
        {
            get { return this._searchTypeField; }
            set { this._searchTypeField = value; }
        }

        /// <summary>
        /// subObject Type
        /// </summary>
        public String SubObjectType
        {
            get { return this._subObjectTypeField; }
            set { this._subObjectTypeField = value; }
        }
                
        public string LogicalOperator
        {
            get { return this._logicalOperator; }
            set { this._logicalOperator = value; }
        }
    }
}
///////////////////////////////////////////////////////////////////////////////

