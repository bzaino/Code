using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    [DataContract]
    public class MemberOrganizationData : IMemberOrganization, IContextDataObject, IPrimary
    {
        public MemberOrganizationData()
        {
        }

        private MemberOrganizationData(SerializationInfo info, StreamingContext context)
        {
            _memberId = info.GetString("_memberId");

            _organizationId = info.GetInt32("_organizationId");
            _organizationId = info.GetInt32("_oeCode");
            _organizationId = info.GetInt32("_branchCode");
            _expectedGraduationYear = info.GetInt32("_expectedGraduationYear");
            _reportingId = info.GetString("_reportingId");
            _isOrganizationDeleted = info.GetBoolean("_isOrganizationDeleted");

            _isPrimary = info.GetBoolean("_isPrimary" );
            _providerKeys = (Dictionary<String, Object>)info.GetValue("_providerKeys", typeof(Dictionary<String, Object>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_memberId", _memberId);
            info.AddValue("_organizationId", _organizationId);
            info.AddValue("_oeCode", _oeCode);
            info.AddValue("_branchCode", _branchCode);
            info.AddValue("_expectedGraduationYear", _expectedGraduationYear);
            info.AddValue("_reportingId", _reportingId);
            info.AddValue("_isOrganizationDeleted", _isOrganizationDeleted);
            info.AddValue("_isPrimary", _isPrimary);
            info.AddValue("_providerKeys", _providerKeys);
        }

        private int _organizationId;
        public int OrganizationId
        {
            get { return _organizationId; }
            set { _organizationId = value; }
        }

        private string _oeCode;
        public string OECode
        {
            get { return _oeCode; }
            set { _oeCode = value; }
        }

        private string _branchCode;
        public string BranchCode
        {
            get { return _branchCode; }
            set { _branchCode = value; }
        }

        private int? _expectedGraduationYear;
        public int? ExpectedGraduationYear
        {
            get { return _expectedGraduationYear; }
            set { _expectedGraduationYear = value; }
        }

        private string _reportingId;
        public string ReportingId
        {
            get { return _reportingId; }
            set { _reportingId = value; }
        }

        private bool _isOrganizationDeleted;
        public bool IsOrganizationDeleted
        {
            get { return _isOrganizationDeleted; }
            set { _isOrganizationDeleted = value; }
        }
        private object _memberId;
        public object MemberId
        {
            get { return _memberId; }
            set { _memberId = value; }
        }

        private object _id;
        public object Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Boolean _isPrimary;
        public Boolean IsPrimary
        {

            get { return _isPrimary; }
            set { _isPrimary = value; }
        }

        private Dictionary<String, Object> _providerKeys;
        public Dictionary<string, object> ProviderKeys
        {
            get { return _providerKeys; }
            set { _providerKeys = value; }
        }
    }
}
