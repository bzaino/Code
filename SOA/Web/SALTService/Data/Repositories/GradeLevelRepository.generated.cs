using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using GradeLevel=Asa.Salt.Web.Services.Domain.GradeLevel;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class GradeLevelRepository : Repository<GradeLevel>, IRepository<GradeLevel,int>
	{
		public GradeLevelRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		