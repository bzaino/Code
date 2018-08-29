using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using vMemberQuestionAnswer=Asa.Salt.Web.Services.Domain.vMemberQuestionAnswer;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class vMemberQuestionAnswerRepository : Repository<vMemberQuestionAnswer>, IRepository<vMemberQuestionAnswer,int>
	{
		public vMemberQuestionAnswerRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		