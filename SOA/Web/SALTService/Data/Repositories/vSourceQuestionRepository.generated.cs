using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using vSourceQuestion=Asa.Salt.Web.Services.Domain.vSourceQuestion;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class vSourceQuestionRepository : Repository<vSourceQuestion>, IRepository<vSourceQuestion,int>
	{
		public vSourceQuestionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		