using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefSALTSchoolType=Asa.Salt.Web.Services.Domain.RefSALTSchoolType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefSALTSchoolTypeRepository : Repository<RefSALTSchoolType>, IRepository<RefSALTSchoolType,int>
	{
		public RefSALTSchoolTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		