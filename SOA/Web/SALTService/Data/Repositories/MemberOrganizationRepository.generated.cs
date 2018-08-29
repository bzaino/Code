using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberOrganization=Asa.Salt.Web.Services.Domain.MemberOrganization;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberOrganizationRepository : Repository<MemberOrganization>, IRepository<MemberOrganization,int>
	{
		public MemberOrganizationRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		