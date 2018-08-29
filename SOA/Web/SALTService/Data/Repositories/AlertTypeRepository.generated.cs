using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using AlertType=Asa.Salt.Web.Services.Domain.AlertType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class AlertTypeRepository : Repository<AlertType>, IRepository<AlertType,int>
	{
		public AlertTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		