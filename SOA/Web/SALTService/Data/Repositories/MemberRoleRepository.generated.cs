using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberRole=Asa.Salt.Web.Services.Domain.MemberRole;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberRoleRepository : Repository<MemberRole>, IRepository<MemberRole,int>
	{
		public MemberRoleRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		