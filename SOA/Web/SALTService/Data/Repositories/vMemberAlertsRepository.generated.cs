using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using vMemberAlerts=Asa.Salt.Web.Services.Domain.vMemberAlerts;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class vMemberAlertsRepository : Repository<vMemberAlerts>, IRepository<vMemberAlerts,int>
	{
		public vMemberAlertsRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		