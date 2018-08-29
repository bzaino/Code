using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using ActivityType=Asa.Salt.Web.Services.Domain.ActivityType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class ActivityTypeRepository : Repository<ActivityType>, IRepository<ActivityType,int>
	{
		public ActivityTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		