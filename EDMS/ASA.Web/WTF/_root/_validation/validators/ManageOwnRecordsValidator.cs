using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.WTF;

namespace ASA.Web.WTF.validators
{
    /// <summary>
    /// Core records validator. Applied to ALL data transaction when running in a user mode context. 
    /// User that is currently active/logged-in may only modify thier own records. 
    /// </summary>
    public class ManageOwnRecordsValidator : ContextActionValidatorBase, IContextActionValidator
    {
        #region IContextActionValidator Members

        public bool Validate(
            IContextDataObject oldState, 
            IContextDataObject newState, 
            Dictionary<object, object> validationParams = null)
        {
            // To defend against internal tampering or someone doing something wierd in the code we make sure
            // the old state and new state records have matching memberIds.
            //
            // NOTE: This is fine for now but we may want to strenghen this to an internal hash or key which would
            // prevent sloppy/malicious API consumers from cirumventing this core validation rule when enabled. 
            //
            // PSS: A hash would also give the added benefit of quick object state comparision between old and new records
            // which could be used to abort an update call that has no real changes. 
            if (oldState.MemberId.ToString() == newState.MemberId.ToString()
                && SiteMember.Account.Id.ToString() == oldState.Id.ToString())
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
