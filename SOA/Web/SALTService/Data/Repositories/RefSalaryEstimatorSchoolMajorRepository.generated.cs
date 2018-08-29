using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefSalaryEstimatorSchoolMajor=Asa.Salt.Web.Services.Domain.RefSalaryEstimatorSchoolMajor;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefSalaryEstimatorSchoolMajorRepository : Repository<RefSalaryEstimatorSchoolMajor>, IRepository<RefSalaryEstimatorSchoolMajor,int>
	{
		public RefSalaryEstimatorSchoolMajorRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		