///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	CriteriaItem.cs
//
//  Description:
//  This file contains all required classes, structs and enums for setting the criteria
//  on the NHibernate query objects.
//  
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using ASA.Common;

namespace ASA.DataAccess
{
    
    /// <summary>
    /// This class is to separate the NHibernate ICriterian from the DAO objects
    /// </summary>
    /// 
    [CLSCompliant(false)]
    public class CriteriaItem : CriteriaItemBase
    {
        /// <summary>
        /// Use this constructor to give fieldname, value and the criteria type
        /// other than Between, Like and IN
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="fieldValue">field value</param>
        /// <param name="critType">criteria type</param>
        public CriteriaItem(String fieldName, Object fieldValue, criteriaType critType)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._critTypeField = critType;
        }

        /// <summary>
        /// Use this constructor to give fieldname and the criteria type
        /// Used when checking Null/NotNull, Empty/Empty
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="fieldValue">field value</param>
        /// <param name="critType">criteria type</param>
        public CriteriaItem(String fieldName, criteriaType critType)
        {
            this._fieldNameField = fieldName;
            this._critTypeField = critType;
        }

        /// <summary>
        /// Use this constructor to give fieldname, field value and matchmode.
        /// To be used for LIKE queries
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="fieldValue">data table field value</param>
        /// <param name="matchmode">Matching mode</param>
        public CriteriaItem(String fieldName, Object fieldValue, criteriaType critType, ASA.Common.MatchMode matchmode)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._critTypeField = critType;

            //The idea is to hide NHibernate expression matchmode from the data access objects
            switch(matchmode)
            {
                case ASA.Common.MatchMode.Anywhere:
                    this._matchmodeField = NHibernate.Criterion.MatchMode.Anywhere;
                    break;
                case ASA.Common.MatchMode.End:
                    this._matchmodeField = NHibernate.Criterion.MatchMode.End;
                    break;
                case ASA.Common.MatchMode.Exact:
                    this._matchmodeField = NHibernate.Criterion.MatchMode.Exact;
                    break;
                case ASA.Common.MatchMode.Start:
                    this._matchmodeField = NHibernate.Criterion.MatchMode.Start;
                    break;
                default:
                    break;
            }
            
        }
        /// <summary>
        /// Use this constructor to use the between clause
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="lowValue">lower limit</param>
        /// <param name="highValue">upper limit</param>
        public CriteriaItem(String fieldName, Object lowValue, Object highValue)
        {
            this._fieldNameField = fieldName;
            this._lowValueField = lowValue;
            this._highValueField = highValue;
            this._critTypeField = criteriaType.Between;
        }

        /// <summary>
        /// Use this constructor for the field values to be in a list
        /// (similar to using IN in SQL)
        /// </summary>
        /// <param name="fieldName">data table field name</param>
        /// <param name="inList">inclusion list</param>
        public CriteriaItem(String fieldName, List<Object> inList)
        {
            this._fieldNameField = fieldName;
            this._inListField = inList;
            this._critTypeField = criteriaType.In;
        }

        public CriteriaItem(String fieldName, Object fieldValue, criteriaType critType, bool notQuery)
        {
            this._fieldNameField = fieldName;
            this._fieldValueField = fieldValue;
            this._critTypeField = critType;
            this._notQueryField = notQuery;
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

        /// <summary>
        /// lower limit
        /// </summary>
        private Object _lowValueField;

        public Object LowValue
        {
            get { return this._lowValueField; }
            set { this._lowValueField = value; }
        }

        /// <summary>
        /// upper limit
        /// </summary>
        private Object _highValueField;

        public Object HighValue
        {
            get { return this._highValueField; }
            set { this._highValueField = value; }
        }


        /// <summary>
        /// in list
        /// </summary>
        private IList<Object> _inListField;
        public IList<Object> InList
        {
            get { return this._inListField; }
            set { this._inListField = value; }
        }

        /// <summary>
        /// NHibernate Matchmode property
        /// </summary>
        private NHibernate.Criterion.MatchMode _matchmodeField;
        public NHibernate.Criterion.MatchMode MatchMode
        {
            get { return this._matchmodeField; }
            set { this._matchmodeField = value; }
        }

        /// <summary>
        /// Boolean value that ID's Criteria as a "NOT" query
        /// </summary>
        private bool _notQueryField;
        public bool NotQuery
        {
            get { return this._notQueryField; }
            set { this._notQueryField = value; }
        }

    }    
}
///////////////////////////////////////////////////////////////////////////////

