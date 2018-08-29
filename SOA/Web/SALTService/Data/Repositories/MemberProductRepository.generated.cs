using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberProduct=Asa.Salt.Web.Services.Domain.MemberProduct;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberProductRepository : Repository<MemberProduct>, IRepository<MemberProduct,int>
	{
		public MemberProductRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		