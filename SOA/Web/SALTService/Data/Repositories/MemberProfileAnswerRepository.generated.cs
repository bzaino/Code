using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberProfileAnswer=Asa.Salt.Web.Services.Domain.MemberProfileAnswer;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberProfileAnswerRepository : Repository<MemberProfileAnswer>, IRepository<MemberProfileAnswer,int>
	{
		public MemberProfileAnswerRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		