using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using VLCUserResponse=Asa.Salt.Web.Services.Domain.VLCUserResponse;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCUserResponseRepository : Repository<VLCUserResponse>, IRepository<VLCUserResponse,int>
	{
		public VLCUserResponseRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		