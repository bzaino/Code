using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberActivityHistory=Asa.Salt.Web.Services.Domain.MemberActivityHistory;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberActivityHistoryRepository : Repository<MemberActivityHistory>, IRepository<MemberActivityHistory,int>
	{
		public MemberActivityHistoryRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		