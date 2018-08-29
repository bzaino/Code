using System;

namespace ASA.Web.WTF
{
    public interface IMemberAccount : IUpdateable 
    {
        Object Id { get; }

        Object MemberId { get; }

        string Username { get; set; }

        Boolean IsAnonymous { get; }

        Boolean IsAuthenticated { get; }
        //Password Functions
        Boolean ChangeUsername(String newUsername);

        /// <summary>
        /// Get a copy of the data object instance.
        /// 
        /// Note: The framework will call it's providers/cache store to create a fully populated dataobject without any lazyloaded data. 
        /// </summary>
        MemberAccountData GetDataObject();

    }
}