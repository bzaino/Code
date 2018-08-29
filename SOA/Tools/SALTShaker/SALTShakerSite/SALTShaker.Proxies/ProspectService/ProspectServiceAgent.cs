using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using SALTShaker.Proxies.PROSPECTService;

namespace SALTShaker.Proxies
{
    public static class ProspectServiceAgent
    {
        private static readonly ILog Log = LogManager.GetLogger("ProspectServiceProxy");
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static ProspectMemberContract GetUserByUserId(int userId)
        {
            using (var client = new ProspectServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByUserId(userId));
            }
        }

        /// <summary>
        /// Gets the user Communications History.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static IEnumerable<ProspectMemberCommunicationContract> GetCommunicationsHistory(int ProspectMemberID)
        {
            using (var client = new ProspectServiceProxy())
            {
                return client.Execute(proxy => proxy.GetCommunicationsHistory(ProspectMemberID));
            }
        }


        public static IEnumerable<prv_MemberCommunicationHistoryContract> GetCommunications(int ProspectMemberID)
        {
            using (var client = new ProspectServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberCommunications(ProspectMemberID));
            }
        }

	/// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        public static ProspectMemberContract GetUserByUserName(string userName)
        {
            using (var client = new ProspectServiceProxy())
            {
                return client.Execute(proxy => proxy.GetuserByUsername(userName));
            }
        }

        public static IEnumerable<ProspectRefDataContracts> GetProspectRefData()
        {
            try
            {
                using (var client = new ProspectServiceProxy())
                {
                    return client.Execute(proxy => proxy.GetProspectRefData());
                }
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("Prospect Service Agent: Execution of {0} complete. {1}", "GetProspectRefData", ex.Message));
            }
            return null;
        }

        /// <summary>
        /// SynchronizeSaltMember with the Prospect Member data
        /// </summary>
        /// <param name="saltMemberID">SALT mender id</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last Name</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="invitationToken">Invitation Token</param>
        /// <param name="organizationExternalID">Organization's External organization id</param>
        /// <param name="memberOrganizationID">Id of the row in the MemberOrganization table</param>
        /// <returns>bool</returns>
        public static bool SynchronizeSaltMember(int saltMemberID, string firstName, string lastName, string emailAddress, Guid? invitationToken, int? organizationExternalID, int? memberOrganizationID)
        {
            bool bReturn = true;
            try
            {
                using (var client = new ProspectServiceProxy())
                {
                    return client.Execute(proxy => proxy.SynchronizeSaltMember((int)saltMemberID, (string)firstName, (string)lastName, (string)emailAddress, (Guid?)invitationToken, (int?)organizationExternalID, (int?)memberOrganizationID));
                }
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("Prospect Service Agent: Execution of {0} complete. {1}", "SynchronizeSaltMember", ex.Message));
                bReturn = false;
            }

            return bReturn;
        }



        public static class Async
        {

            public static List<ProspectMemberContract> GetUsersBySearchParms(String FirstName, String LastName, AsyncCallback callback)
            {
                try
                {
                    using (var client = new ProspectServiceProxy())
                    {
                        return client.ExecuteAsync(proxy => proxy.GetUsersBySearchParms(FirstName, LastName), callback, null).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(string.Format("Prospect Service Agent: Execution of {0} complete. {1}", "GetUsersBySearchParms", ex.Message ));
                }
                return new List<ProspectMemberContract>();
            }

            public static bool UpdateCommunicationPreferences(string userName, bool emailContact, bool phoneContact, bool mailContact, string modifierUserName, AsyncCallback callback)
            {
                using (var client = new ProspectServiceProxy())
                {
                    return client.ExecuteAsync(proxy => proxy.UpdateCommunicationPreferences(userName, emailContact, phoneContact, mailContact, modifierUserName), callback, null);
                }
            }

            public static bool AddCommunication(int propectMemberID, string CreatedBy, Nullable<int> CommChannelID, Nullable<int> CommDirectionID, Nullable<int> CallResultsID, Nullable<int> CommSourceID, Nullable<int> CommTypeID, string CommText, AsyncCallback callback)
            {
                using (var client = new ProspectServiceProxy())
                {
                    return client.ExecuteAsync(proxy => proxy.AddCommunication(propectMemberID, CreatedBy, CommChannelID, CommDirectionID, CallResultsID, CommSourceID, CommTypeID, CommText), callback, null);
                }
            }

        }
    }
}
