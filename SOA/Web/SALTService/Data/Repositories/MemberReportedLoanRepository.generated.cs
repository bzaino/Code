using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberReportedLoan=Asa.Salt.Web.Services.Domain.MemberReportedLoan;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberReportedLoanRepository : Repository<MemberReportedLoan>, IRepository<MemberReportedLoan,int>
	{
		public MemberReportedLoanRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		