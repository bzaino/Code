using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefOrganization=Asa.Salt.Web.Services.Domain.RefOrganization;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefOrganizationRepository : Repository<RefOrganization>, IRepository<RefOrganization,int>
	{
		public RefOrganizationRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		