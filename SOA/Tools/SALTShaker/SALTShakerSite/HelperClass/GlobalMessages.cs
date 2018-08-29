using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SALTShaker
{
    public static class GlobalMessages
    {
        //define current user
        //IPrincipal CurrentUser;
        //Catch Exception errors
        public const string sMSG_WARNING = "Oops that was unexpected: {0}";
        public const string sMSG_NOCOURSE_WARNING = "There are no courses associated with this account.";
        public const string sMSG_NO_ORGANIZATION_WARNING = "There are no Organizations associated with this account.";
        public const string sMSG_NOALERTS_WARNING = "There are no alerts associated with this account.";
        public const string sMSG_NOACTIVITYHISTORY_WARNING = "There is no activity history associated with this account.";
        public const string sMSG_UPDATE_SUCCESS = "Successfully updated! ";
        public const string sMSG_REGSOURCE_UPDATE_SUCCESS = "The Registration Source was successfully updated! ";
        public const string sMSG_REGSOURCE_CREATE_SUCCESS = "The new Registration Source was successfully created!";
        public const string sMSG_REFCAMPAIGN_UPDATE_SUCCESS = "The Campaign was successfully updated! ";
        public const string sMSG_REFCAMPAIGN_CREATE_SUCCESS = "The new Campaign was successfully created!";
        public const string sMSG_NO_USERROLES_WARNING = "There are no User Roles set up in the database.";
        //EMPTY paramater errror
        public const string sMSG_EMPTYPARAM = "The value for {0} is empty or has not been provided. Please enter a value in {0} and try again.";
        //ButtonSearch_Click
        public const string sMSG_SEARCH_FAILED = "Can not find the user account, Please enter either user name or full name.";
        public const string sMSG_MEMBERID_ISNULL = "Selected Member {0} is invalid or was not supplied! Member data could not be retrieved.";
        public const string sMSG_REFORGANIZATIONID_ISNULL = "RefOrganizationID is Null!";

        public const string sMSG_MEMBERID_NOTFOUND_OR_SERVICE_DOWN = "The member {0} was not found in {1}. Either the member has not yet been added to {1} or the service is currently unavailable. {2} ";
        public const string sMSG_NORECORDSFOUND = "No records matched your search. Please try again.";
        public const string sMSG_NORECORDFOUND_MEMBERID = "No record found by the specified Member ID. Please provide a valid Member ID.";
        public const string sMSG_NORECORDFOUND_USERNAME = "No record found by the specified Email Address. Please verify the email used.";
        public const string sMSG_INVALITD_MEMBERID = "Invalid Member Id please try another Id or select display all";
        public const string sMSG_MEMBERID_TOO_MANY_DIGITS = "The member id has too many digits. Please try again.";
        public const string sMSG_AT_LEAST_ONE_VALUE_REQUIRED = "Please at least enter one of the below fields...";
        public const string sMSG_ENTER_VALID_SEARCH = "Please enter valid search criteria";
        public const string sMSG_SELECT_FIRST_NAME = "Please select first!";
        //Active Directory
        public const string sCUSTOM_ERRORCODE = " Error Code: 4911";
        public const string sMSG_EMAILNOTFOUND = "User With email address {0} was not found {1}";
        public const string sMSG_USER_ACTACTIVATE = "{0}'s SALT account was {1}";
        public const string sMSG_USER_IMFO_CHANGED_SUCCESS = "Member {0}'s email address and user name has been changed to {1}";
        public const string sMSG_USER_IMFO_CHANGED_FAILDED = "No member matching email address {0} was found. Update could not be completed {1}";
        public const string sMSG_VALUE_ISNULL = "{0} can not be null or empty please add a value to {0} and try again. {1}";
    }
}
