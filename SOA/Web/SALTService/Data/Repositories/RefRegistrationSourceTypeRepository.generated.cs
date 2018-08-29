using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefRegistrationSourceType=Asa.Salt.Web.Services.Domain.RefRegistrationSourceType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefRegistrationSourceTypeRepository : Repository<RefRegistrationSourceType>, IRepository<RefRegistrationSourceType,int>
	{
		public RefRegistrationSourceTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		