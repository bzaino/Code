using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using Member=Asa.Salt.Web.Services.Domain.Member;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberRepository : Repository<Member>, IRepository<Member,int>
	{
		public MemberRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		