using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefToDoType=Asa.Salt.Web.Services.Domain.RefToDoType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefToDoTypeRepository : Repository<RefToDoType>, IRepository<RefToDoType,int>
	{
		public RefToDoTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		