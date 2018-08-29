using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using VLCMemberProfile=Asa.Salt.Web.Services.Domain.VLCMemberProfile;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCMemberProfileRepository : Repository<VLCMemberProfile>, IRepository<VLCMemberProfile,int>
	{
		public VLCMemberProfileRepository(SALTEntities context,ICacheProvider<SALTEntities, VLCMemberProfile> cacheProvider) : base(context,cacheProvider)
		{

		}
	}
}
		