using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Configuration.Caching;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using RefUserProfileItem=Asa.Salt.Web.Services.Domain.RefUserProfileItem;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefUserProfileItemRepository : Repository<RefUserProfileItem>, IRepository<RefUserProfileItem,int>
	{
		public RefUserProfileItemRepository(SALTEntities context,ICacheProvider<SALTEntities, RefUserProfileItem> cacheProvider, IApplicationCachingConfiguration cachingConfiguration) : base(context,cacheProvider,cachingConfiguration)
		{

		}
	}
}
		