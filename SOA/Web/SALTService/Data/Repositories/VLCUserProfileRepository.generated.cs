using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using VLCUserProfile=Asa.Salt.Web.Services.Domain.VLCUserProfile;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCUserProfileRepository : Repository<VLCUserProfile>, IRepository<VLCUserProfile,int>
	{
		public VLCUserProfileRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		