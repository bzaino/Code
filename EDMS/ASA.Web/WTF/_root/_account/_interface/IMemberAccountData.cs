using System;

namespace ASA.Web.WTF
{
    public interface IMemberAccountData : IContextDataObject
    {
        new Object MemberId { get; }
        new Object Id { get; }
        //Core Identifiers
        String Username { get; }

        //Persistent Values
        DateTime RegistrationDate { get; }
        DateTime Created { get; }

        //User Managed Values
        String PasswordQuestion { get; }

        //System Managed Status
        Boolean IsApproved { get;}
        Boolean IsLockedOut { get; }
        Boolean IsOnline { get; }

        //History Flags
        DateTime LastActivity { get; }
        DateTime LastLockout { get; }
        DateTime LastLogin { get; }
        DateTime LastPasswordChange { get; }
    }
}