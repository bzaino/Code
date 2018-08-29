using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Common
{
    public class CriterionBase
    {
        /// <summary>
        /// field name
        /// </summary>
        protected String _fieldNameField;

        public String FieldName
        {
            get { return this._fieldNameField; }
            set { this._fieldNameField = value; }
        }
		
        /// <summary>
        /// field value
        /// </summary>
        protected Object _fieldValueField;

        public Object FieldValue
        {
            get { return this._fieldValueField; }
            set { this._fieldValueField = value; }
        }

        /// <summary>
        /// Field name of sub-object being searched on
        /// </summary>
        protected string _subObjectTypeField;
        public string SubObjectTypeField
        {
            get { return this._subObjectTypeField; }
            set { this._subObjectTypeField = value; }
        }		
    }

    public class Criterion : CriterionBase
    {
        public Criterion()
        {
			_businessRule = false;
        }
        
		string _criterionName;

        public string CriterionName
        {
            get { return _criterionName; }
            set { _criterionName = value; }
        }
        
        RelationalOperatorType _relationalOperator;

        public RelationalOperatorType RelationalOperator
        {
            get { return _relationalOperator; }
            set { _relationalOperator = value; }
        }
        
		LogicalOperatorType _logicalOperator;

		public LogicalOperatorType LogicalOperator
        {
            get { return _logicalOperator; }
            set { _logicalOperator = value; }
        }

        bool _businessRule;

        public bool BusinessRule
        {
            get { return _businessRule; }
            set { _businessRule = value; }
        }
    }
	
    public class Criteria : List<Criterion>
    {
        LogicalOperatorType _logicalOperator;

        public LogicalOperatorType LogicalOperator
        {
            get { return _logicalOperator; }
            set { _logicalOperator = value; }
        }
    }

    public class CriteriaList : List<Criteria>
    {	
		public CriteriaList(string name)
        {
            this._name = name;
            this._sortCriteria = new SortCriteria();
        }

		string _name;
	
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        int _maxEntities;

        public int MaxEntities
        {
            get { return _maxEntities; }
            set { _maxEntities = value; }
        }

        SortCriteria _sortCriteria;

        public SortCriteria SortCriteria
        {
            get { return _sortCriteria; }
            set { _sortCriteria = value; }
        }
       
    }

    public enum RelationalOperatorType : int
    {
        EQUALS = 0,
        LESSTHAN = 1,
        LESSTHAN_EQUAL = 2,
        GREATERTHAN = 3,
        GREATERTHAN_EQUAL = 4,
        IN = 5,
        STARTSWITH = 6,
        ENDSWITH = 7,
        CONTAINS = 8

    }

    public enum LogicalOperatorType : int
    {
        AND = 0,
        OR = 1
    }

    public sealed class PersSvcCriteriaNames
    {
        public const string PSC1 = "PSC1";
        public const string PSC1_1 = "PSC1_1";
        public const string PSC2 = "PSC2";
        public const string PSC3 = "PSC3";
        public const string PSC4 = "PSC4";
        public const string PSC5 = "PSC5";
        public const string PSC6 = "PSC6";
        public const string PSC7 = "PSC7";
        public const string PSC8 = "PSC8";
        public const string PSC9 = "PSC9";
        public const string PSC10 = "PSC10";
        public const string PSC11 = "PSC11";
		public const string PSC12 = "PSC12";
	}

   

	public sealed class LoanServiceCriteriaNames
	{
		public const string LoanCriteriaExternalLoanId = "LoanCriteriaExternalLoanId";
		public const string LoanCriteriaLoanId = "LoanCriteriaLoanId";
		public const string LoanCriteriaPersonId = "LoanCriteriaPersonId";
		public const string LoanCriteriaPersonIdRoleId = "LoanCriteriaPersonIdRoleId";
		public const string LoanCriteriaSsn = "LoanCriteriaSsn";
	}
    public sealed class OrgSvcCriteriaNames
    {
        public const string ExternalOrganizationId = "ExternalOrganizationId";
        public const string OrganizationId = "OrganizationId";
        public const string OrganizationIdList = "OrganizationIdList";
        public const string OPECode = "OPECode";
        public const string OPECodeandBranch = "OPECodeandBranch";
        public const string OPECodeandOrgName = "OPECodeandOrgName";
        public const string OPECodeandOrgNameandOrganizationTypeId = "OPECodeandOrgNameandOrganizationTypeId";
        public const string OPECodeandOrganizationTypeId = "OPECodeandOrganizationTypeId";
        public const string Branch = "Branch";
        public const string BranchandOrgName = "BranchandOrgName";
        public const string BranchandOrganizationTypeId = "BranchandOrganizationTypeId";
        public const string OrgName = "OrgName";
        public const string OrgNameandOrganizationTypeId = "OrgNameandOrganizationTypeId";
        public const string ParentOPECode = "ParentOPECode";
        public const string ParentOPECodeandOrganizationTypeId = "ParentOPECodeandOrganizationTypeId";
        public const string OrganizationTypeId = "OrganizationTypeId";
    }
}
