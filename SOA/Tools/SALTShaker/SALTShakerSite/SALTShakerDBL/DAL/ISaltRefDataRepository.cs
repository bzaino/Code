using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.DAL
{
    public interface ISaltRefDataRepository : IDisposable
    {
        IEnumerable<RefRegistrationSourceModel> GetAllRefRegistrationSources();
        IEnumerable<vMemberRoleModel> GetAllRefUserRoles();
    }
}
