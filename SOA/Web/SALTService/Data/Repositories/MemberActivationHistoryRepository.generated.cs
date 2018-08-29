using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberActivationHistory=Asa.Salt.Web.Services.Domain.MemberActivationHistory;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberActivationHistoryRepository : Repository<MemberActivationHistory>, IRepository<MemberActivationHistory,int>
	{
		public MemberActivationHistoryRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		