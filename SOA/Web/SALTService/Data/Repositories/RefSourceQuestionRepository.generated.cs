using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefSourceQuestion=Asa.Salt.Web.Services.Domain.RefSourceQuestion;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefSourceQuestionRepository : Repository<RefSourceQuestion>, IRepository<RefSourceQuestion,int>
	{
		public RefSourceQuestionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		