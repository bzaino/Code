using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefOccupation=Asa.Salt.Web.Services.Domain.RefOccupation;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefOccupationRepository : Repository<RefOccupation>, IRepository<RefOccupation,int>
	{
		public RefOccupationRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		