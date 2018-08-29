using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using LessonQuestionResponse=Asa.Salt.Web.Services.Domain.LessonQuestionResponse;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LessonQuestionResponseRepository : Repository<LessonQuestionResponse>, IRepository<LessonQuestionResponse,int>
	{
		public LessonQuestionResponseRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		