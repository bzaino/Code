using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefSalaryEstimatorSchool=Asa.Salt.Web.Services.Domain.RefSalaryEstimatorSchool;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefSalaryEstimatorSchoolRepository : Repository<RefSalaryEstimatorSchool>, IRepository<RefSalaryEstimatorSchool,int>
	{
		public RefSalaryEstimatorSchoolRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		