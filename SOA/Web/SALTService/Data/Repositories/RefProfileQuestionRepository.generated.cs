using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefProfileQuestion=Asa.Salt.Web.Services.Domain.RefProfileQuestion;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefProfileQuestionRepository : Repository<RefProfileQuestion>, IRepository<RefProfileQuestion,int>
	{
		public RefProfileQuestionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		