using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using LoanType=Asa.Salt.Web.Services.Domain.LoanType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LoanTypeRepository : Repository<LoanType>, IRepository<LoanType,int>
	{
		public LoanTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		