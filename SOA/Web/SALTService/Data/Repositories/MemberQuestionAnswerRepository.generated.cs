using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberQuestionAnswer=Asa.Salt.Web.Services.Domain.MemberQuestionAnswer;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberQuestionAnswerRepository : Repository<MemberQuestionAnswer>, IRepository<MemberQuestionAnswer,int>
	{
		public MemberQuestionAnswerRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		