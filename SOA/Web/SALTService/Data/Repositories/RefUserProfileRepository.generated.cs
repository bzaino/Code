using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Configuration.Caching;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using RefUserProfile=Asa.Salt.Web.Services.Domain.RefUserProfile;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefUserProfileRepository : Repository<RefUserProfile>, IRepository<RefUserProfile,int>
	{
		public RefUserProfileRepository(SALTEntities context,ICacheProvider<SALTEntities, RefUserProfile> cacheProvider, IApplicationCachingConfiguration cachingConfiguration) : base(context,cacheProvider,cachingConfiguration)
		{

		}
	}
}
		