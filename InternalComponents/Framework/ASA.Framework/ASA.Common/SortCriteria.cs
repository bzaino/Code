///////////////////////////////////////////////////////////////////////////////
//  WorkFile Name:	SortCriteria.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;

namespace ASA.Common
{
    /// <summary>
    /// This class hold list of sort ciriterion. 
    /// </summary>
    public class SortCriteria:List<SortCriterion>
    { 
    
    }


    /// <summary>
    /// This class hold sort ciriterion. 
    /// This includes the column name to sort by and the direction
    /// </summary>
    public class SortCriterion
    {
        private SortDirection sortDirection;

        /// <summary>
        /// Constructure for Sort Criteria Default Contructor
        /// </summary>

        public SortCriterion()
        {
            fieldName = String.Empty;
            sortDirection = SortDirection.Ascending;
        }

        /// <summary>
        /// sort direction property
        /// </summary>
        public SortDirection SortDirection
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }
        private string fieldName;

        /// <summary>
        /// Field name property
        /// </summary>
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        /// <summary>
        /// Constructure for Sort Criteria
        /// </summary>
        /// <param name="FieldName">Field name</param>
        /// <param name="Direction">Direction</param>
        public SortCriterion(string FieldName, SortDirection Direction)
        {
            fieldName = FieldName;
            sortDirection = Direction;
        }

       
        /// <summary>
        /// 
        /// </summary>
        protected string _subObjectTypeField;
        public string SubObjectTypeField
        {
            get { return this._subObjectTypeField; }
            set { this._subObjectTypeField = value; }
        }		
    }
}
///////////////////////////////////////////////////////////////////////////////

