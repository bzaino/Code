using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.WTF.Integration;

namespace ASA.Web.WTF
{
    public class ContextActionValidatorBase
    {
        SiteMember _siteMember;
        public ContextActionValidatorBase() { }

        private void createCredentials()
        {
            _siteMember = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").GetMember();
        }

        protected SiteMember SiteMember
        {
            get
            {
                if (_siteMember == null)
                {
                    createCredentials();
                }

                return _siteMember;
            }
        }
    }
}
