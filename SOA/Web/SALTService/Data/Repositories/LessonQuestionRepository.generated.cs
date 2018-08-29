using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using LessonQuestion=Asa.Salt.Web.Services.Domain.LessonQuestion;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LessonQuestionRepository : Repository<LessonQuestion>, IRepository<LessonQuestion,int>
	{
		public LessonQuestionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		