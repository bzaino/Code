using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefCampaign=Asa.Salt.Web.Services.Domain.RefCampaign;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefCampaignRepository : Repository<RefCampaign>, IRepository<RefCampaign,int>
	{
		public RefCampaignRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		