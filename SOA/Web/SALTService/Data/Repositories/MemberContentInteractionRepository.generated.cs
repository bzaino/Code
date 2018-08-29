using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberContentInteraction=Asa.Salt.Web.Services.Domain.MemberContentInteraction;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberContentInteractionRepository : Repository<MemberContentInteraction>, IRepository<MemberContentInteraction,int>
	{
		public MemberContentInteractionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		