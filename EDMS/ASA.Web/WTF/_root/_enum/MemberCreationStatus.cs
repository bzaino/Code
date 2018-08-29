namespace ASA.Web.WTF
{
    public enum MemberCreationStatus
    {
        Success,
        Error,
        DuplicateUserName,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        InvalidAnswer,
        InvalidQuestion,
        InvalidUserName,
        AdapterError,
        UserRejected,
        InvalidAddress,
        InvalidPhoneNumber,
        InvalidPin
    }
}