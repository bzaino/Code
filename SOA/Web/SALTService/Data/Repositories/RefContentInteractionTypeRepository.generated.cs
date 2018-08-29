using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefContentInteractionType=Asa.Salt.Web.Services.Domain.RefContentInteractionType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefContentInteractionTypeRepository : Repository<RefContentInteractionType>, IRepository<RefContentInteractionType,int>
	{
		public RefContentInteractionTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		