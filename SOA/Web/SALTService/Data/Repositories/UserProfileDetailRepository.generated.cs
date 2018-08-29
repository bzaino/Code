using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Configuration.Caching;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using UserProfileDetail=Asa.Salt.Web.Services.Domain.UserProfileDetail;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class UserProfileDetailRepository : Repository<UserProfileDetail>, IRepository<UserProfileDetail,int>
	{
		public UserProfileDetailRepository(SALTEntities context,ICacheProvider<SALTEntities, UserProfileDetail> cacheProvider, IApplicationCachingConfiguration cachingConfiguration) : base(context,cacheProvider,cachingConfiguration)
		{

		}
	}
}
		