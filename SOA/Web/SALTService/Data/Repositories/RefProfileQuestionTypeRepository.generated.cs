using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefProfileQuestionType=Asa.Salt.Web.Services.Domain.RefProfileQuestionType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefProfileQuestionTypeRepository : Repository<RefProfileQuestionType>, IRepository<RefProfileQuestionType,int>
	{
		public RefProfileQuestionTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		