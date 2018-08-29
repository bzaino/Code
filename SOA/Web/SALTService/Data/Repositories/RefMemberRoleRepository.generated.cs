using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefMemberRole=Asa.Salt.Web.Services.Domain.RefMemberRole;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefMemberRoleRepository : Repository<RefMemberRole>, IRepository<RefMemberRole,int>
	{
		public RefMemberRoleRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		