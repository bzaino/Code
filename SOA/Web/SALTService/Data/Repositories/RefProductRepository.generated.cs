using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefProduct=Asa.Salt.Web.Services.Domain.RefProduct;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefProductRepository : Repository<RefProduct>, IRepository<RefProduct,int>
	{
		public RefProductRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		