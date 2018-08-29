using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using ASA.Log.ServiceLogger;
using ASA.Web.Services.ASAMemberService.DataContracts;

using HttpsRequestWrapper;

namespace ASA.Web.Services.ASAMemberService
{
    public class QualtricsTA
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.ASAMemberService.QualtricsTA";

        /// <summary>
        /// The log
        /// </summary>
        private static readonly IASALog Log = ASALogManager.GetLogger(Classname);

        public QualtricsTA()
        {
        }
        /// <summary>
        /// setup a task to call Qualtric's Target Audience API createUser, so we don't have to wait for it to finish.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public void CreateUser(ASAMemberModel member)
        {
            const string logMethodName = ".CreateUser(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");

            Task.Factory.StartNew(() => { CreateUserProcess(member); });

            Log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// setup a task to call Qualtric's Target Audience API updateUser, so we don't have to wait for it to finish.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public void UpdateUser(ASAMemberModel member)
        {
            const string logMethodName = ".UpdateUser(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");

            Task.Factory.StartNew(() => { UpdateUserProcess(member); });

            Log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Will call the Qualtric's Target Audience API createUser.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        internal void CreateUserProcess(ASAMemberModel member)
        {
            const string logMethodName = ".CreateUserProcess(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");

            string QTA_Process = Common.Config.QTA_Process;

            if (QTA_Process.ToLower() == "on")
            {
                Log.Info(logMethodName + "Qualtrics TA Process on - request attempt will be made");

                //https://survey.qualtrics.com/WRAPI/Contacts/api.php?Request=createContact&Format=JSON&Version=2.3&User=APIDevelopment&Token=ZyEeqfR9oFI9bTOu1tzWNWQ43my7elCNXBe6Ql8b&LibraryID=GR_6r0To7ELRwsmMfP&ListID=CG_dbUMDRYesJ0MCot&Email=tlesliepdv4-2016-02-03-08@asa.org&FirstName=Todd&LastName=Leslie&ExternalDataRef=Id12346&ED[Contact]=true&ED[Org0]=Org-99999&ED[Org1]=Org-77777&ED[Org2]=Org-55555&ED[Org3]=Org-33333&ED[Org4]=Org-11111&ED[Org5]=&ED[Org6]=&ED[Org7]=&ED[Org8]=&ED[Org9]=
                String urlToRequest = BuildUrlString("createContact");
                urlToRequest += BuildDataString(member);

                Log.Debug(logMethodName + "Request - " + urlToRequest);

                HttpsRequestProvider httpsRequestProvider = new HttpsRequestProvider();
                string responseString = httpsRequestProvider.sendRequestToQualtricsTA(urlToRequest);
            }
            else
            {
                Log.Info(logMethodName + "Qualtrics TA Process off - request not made");
            }

            Log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Will call the Qualtric's Target Audience API getContactByInfoFields if user exists
        /// then updateContact will be call otherwise createContact will be called.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        internal void UpdateUserProcess(ASAMemberModel member)
        {
            const string logMethodName = ".UpdateUserProcess(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");

            string QTA_Process = Common.Config.QTA_Process;

            if (QTA_Process.ToLower() == "on")
            {
                Log.Info(logMethodName + "Qualtrics TA Process on - request attempt will be made");

                //"https://survey.qualtrics.com/WRAPI/Contacts/api.php?Request=getContactByInfoFields&Format=JSON&Version=2.3&User=APIDevelopment&Token=ZyEeqfR9oFI9bTOu1tzWNWQ43my7elCNXBe6Ql8b&ExternalDataRef=1934";
                String urlToRequest = BuildUrlString("getContactByInfoFields");
                String dataPart = String.Format("&ExternalDataRef={0}", member.MembershipId);
                urlToRequest += dataPart;

                Log.Debug(logMethodName + "Request - " + urlToRequest);

                HttpsRequestProvider httpsRequestProvider = new HttpsRequestProvider();
                string responseString = httpsRequestProvider.sendRequestToQualtricsTA(urlToRequest);

                //covert response to JSON object
                JObject responseAsJSON = new JObject();
                responseAsJSON = JObject.Parse(responseString);

                string returnStatus = responseAsJSON["Meta"].Any() ? (string)responseAsJSON["Meta"]["Status"] : String.Empty;
                string contactId = responseAsJSON["Result"].Any() ? (string)responseAsJSON["Result"][0]["ContactId"] : String.Empty;

                if (returnStatus == "Success" && contactId != String.Empty) //update existing user
                {

                    //https://survey.qualtrics.com/WRAPI/Contacts/api.php?Request=updateContact&Format=JSON&Version=2.3&User=APIDevelopment&Token=ZyEeqfR9oFI9bTOu1tzWNWQ43my7elCNXBe6Ql8b&LibraryID=GR_6r0To7ELRwsmMfP&ListID=CG_dbUMDRYesJ0MCot&ContactID=_0HA3IIsMONMkCkR&Email=somename%2540example.com

                    urlToRequest = BuildUrlString("updateContact");
                    dataPart = String.Format("&ContactID={0}", contactId);
                    dataPart += BuildDataString(member);
                    urlToRequest += dataPart;

                    Log.Debug(logMethodName + "Request - " + urlToRequest);

                    responseString = httpsRequestProvider.sendRequestToQualtricsTA(urlToRequest);
                }
                else
                {
                    //no user found so insert/create one.
                    CreateUserProcess(member);
                }
            }
            else
            {
                Log.Info(logMethodName + "Qualtrics TA Process off - request not made");
            }

            Log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Build the constant parts of the url string for Quatrics TA call.
        /// </summary>
        /// <returns>string containing the first part of url</returns>
        public string BuildUrlString(string apiRequestName)
        {
            const string logMethodName = ".BuildUrlString() - ";
            Log.Debug(logMethodName + "Begin Method");

            string QTA_URL = Common.Config.QTA_URL;
            string QTA_User = Common.Config.QTA_User;
            string QTA_Token = Common.Config.QTA_Token;
            string QTA_API_Version = Common.Config.QTA_API_Version;
            string QTA_Library_ID = Common.Config.QTA_Library_ID;
            string QTA_List_ID = Common.Config.QTA_List_ID;

            string urlToRequest = String.Format("{0}?Request={1}&Format=JSON&Version={2}&User={3}&Token={4}&LibraryID={5}&ListID={6}", QTA_URL, apiRequestName, QTA_API_Version, QTA_User, QTA_Token, QTA_Library_ID, QTA_List_ID);

            Log.Debug(logMethodName + "End Method");

            return urlToRequest;
        }

        /// <summary>
        /// Build the constant parts of the url string for Quatrics TA call.
        /// </summary>
        /// <returns>string to containing the common data components of url parameter list</returns>
        public string BuildDataString(ASAMemberModel member)
        {
            string emailAddress = member.PrimaryEmailKey;
            string firstName = member.FirstName;
            string lastName = member.LastName;
            string membershipId = member.MembershipId;
            string contact = member.ContactFrequency == true ? "no" : "yes"; //do not contact true-no(do not contact) false-yes(contact)

            List<string> listOfOrganizationNames = new List<string>();
            List<string> listOfOrganizationTypes = new List<string>();
            foreach (MemberOrganizationModel organization in member.Organizations)
            {
                listOfOrganizationNames.Add(organization.OrganizationName);
                listOfOrganizationTypes.Add(organization.OrganizationTypeExternalId);
            }

            //need to have ten organization names and types in the array
            string[] organizationNames = new string[10];
            string[] organizationTypes = new string[10];

            for (int i = 0; i < 10; i++)
            {
                if (i < listOfOrganizationNames.Count)
                {
                    organizationNames[i] = listOfOrganizationNames[i];
                    organizationTypes[i] = listOfOrganizationTypes[i];
                }
                else
                {
                    organizationNames[i] = string.Empty;
                    organizationTypes[i] = string.Empty;
                }
            }

            string dataPart = String.Format("&Email={0}&FirstName={1}&LastName={2}&ExternalDataRef={3}&ED[Contact]={4}&ED[Org0]={5}&ED[Org0Type]={6}&ED[Org1]={7}&ED[Org1Type]={8}&ED[Org2]={9}&ED[Org2Type]={10}&ED[Org3]={11}&ED[Org3Type]={12}&ED[Org4]={13}&ED[Org4Type]={14}&ED[Org5]={15}&ED[Org5Type]={16}&ED[Org6]={17}&ED[Org6Type]={18}&ED[Org7]={19}&ED[Org7Type]={20}&ED[Org8]={21}&ED[Org8Type]={22}&ED[Org9]={23}&ED[Org9Type]={24}",
                                emailAddress,
                                firstName,
                                lastName,
                                membershipId,
                                contact,
                                organizationNames[0],
                                organizationTypes[0],
                                organizationNames[1],
                                organizationTypes[1],
                                organizationNames[2],
                                organizationTypes[2],
                                organizationNames[3],
                                organizationTypes[3],
                                organizationNames[4],
                                organizationTypes[4],
                                organizationNames[5],
                                organizationTypes[5],
                                organizationNames[6],
                                organizationTypes[6],
                                organizationNames[7],
                                organizationTypes[7],
                                organizationNames[8],
                                organizationTypes[8],
                                organizationNames[9],
                                organizationTypes[9]);

            return dataPart;
        }
    }
}