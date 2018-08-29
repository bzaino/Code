using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefToDoStatus=Asa.Salt.Web.Services.Domain.RefToDoStatus;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefToDoStatusRepository : Repository<RefToDoStatus>, IRepository<RefToDoStatus,int>
	{
		public RefToDoStatusRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		