using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Configuration.Caching;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using RefUserProfileUserProfileItem=Asa.Salt.Web.Services.Domain.RefUserProfileUserProfileItem;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefUserProfileUserProfileItemRepository : Repository<RefUserProfileUserProfileItem>, IRepository<RefUserProfileUserProfileItem,int>
	{
		public RefUserProfileUserProfileItemRepository(SALTEntities context,ICacheProvider<SALTEntities, RefUserProfileUserProfileItem> cacheProvider, IApplicationCachingConfiguration cachingConfiguration) : base(context,cacheProvider,cachingConfiguration)
		{

		}
	}
}
		