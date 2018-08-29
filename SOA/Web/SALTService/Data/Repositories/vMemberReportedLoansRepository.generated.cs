using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using vMemberReportedLoans=Asa.Salt.Web.Services.Domain.vMemberReportedLoans;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class vMemberReportedLoansRepository : Repository<vMemberReportedLoans>, IRepository<vMemberReportedLoans,int>
	{
		public vMemberReportedLoansRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		