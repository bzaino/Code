using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.validators
{
    class MustbeLoggedInValidator : ContextActionValidatorBase, IContextActionValidator
    {
        #region IContextActionValidator Members

        public bool Validate(IContextDataObject oldState, IContextDataObject newState, Dictionary<object, object> validationParams = null)
        {
            //if (Credentials.Account.IsOnline && Credentials.Account.IsApproved && !Credentials.Account.IsLockedOut) //This does not work, for now
            if (SiteMember.Account.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
