using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefProductType=Asa.Salt.Web.Services.Domain.RefProductType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefProductTypeRepository : Repository<RefProductType>, IRepository<RefProductType,int>
	{
		public RefProductTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		