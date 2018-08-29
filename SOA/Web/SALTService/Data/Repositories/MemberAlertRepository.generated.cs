using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberAlert=Asa.Salt.Web.Services.Domain.MemberAlert;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberAlertRepository : Repository<MemberAlert>, IRepository<MemberAlert,int>
	{
		public MemberAlertRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		