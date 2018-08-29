using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefMajor=Asa.Salt.Web.Services.Domain.RefMajor;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefMajorRepository : Repository<RefMajor>, IRepository<RefMajor,int>
	{
		public RefMajorRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		