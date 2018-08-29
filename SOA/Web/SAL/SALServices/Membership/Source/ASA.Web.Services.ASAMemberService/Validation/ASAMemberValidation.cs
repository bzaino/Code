using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    public class ASAMemberValidation
    {
        //public static bool ValidateMemberId(string memberId)
        //{
        //        bool bValid = false;
        //        ASAMemberModel member = new ASAMemberModel();
        //        member.MembershipId = memberId;
        //        if (memberId != null && member.IsValid("Member Id"))
        //        {
        //            bValid = true;
        //        }
	
        //        return bValid;
        //}

        //public static bool ValidateInputSurveyList(SurveyListModel sList)
        //{
        //    bool bValid = false;
        //    if (sList != null && sList.Surveys != null)
        //    {
        //        bValid = true;
        //        foreach (SurveyModel survey in sList.Surveys)
        //        {
        //            bValid &= survey.IsValid();
        //            if (!bValid)
        //                break;
        //        }
        //    }

        //    return bValid;
        //}

        public static bool ValidateASAMember(ASAMemberModel member)
        {
            bool bValid = false;
            if (member != null)
                bValid = member.IsValid();
            return bValid;
        }
    }
}
