using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefSource=Asa.Salt.Web.Services.Domain.RefSource;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefSourceRepository : Repository<RefSource>, IRepository<RefSource,int>
	{
		public RefSourceRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		