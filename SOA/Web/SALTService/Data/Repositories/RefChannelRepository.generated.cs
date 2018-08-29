using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefChannel=Asa.Salt.Web.Services.Domain.RefChannel;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefChannelRepository : Repository<RefChannel>, IRepository<RefChannel,int>
	{
		public RefChannelRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		