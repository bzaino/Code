using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    /// <summary>
    /// This interface is reponsible for basic extended user data. Our only requirments
    /// are that each MemberProfile record has a uniqe ID.
    /// 
    /// The MemberProfile is used to store basic core user information like Name, Addreess, About Me, personal web links,
    /// signatures, etc. The profile is not used to store user preferences deterministc security data, click stream,
    /// analytiics or any other kind concept outside the core user profile. 
    ///
    /// </summary>
    /// 
    //TODO: REVIEW IF THIS INTEFACE IS NECCASSARY
    //Possible refactor of IMemberProfileData to IMemberProfile then remove IMemberProfileData
    public interface IMemberProfile : IUpdateable
    {
        Object Id { get; }
        string DisplayName { get; }

        MemberEmailData Email { get; set; }

        Nullable<short> YearOfBirth { get; set; }


    }

}
