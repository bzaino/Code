using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefGeographicIndex=Asa.Salt.Web.Services.Domain.RefGeographicIndex;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefGeographicIndexRepository : Repository<RefGeographicIndex>, IRepository<RefGeographicIndex,int>
	{
		public RefGeographicIndexRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		