using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using Lesson=Asa.Salt.Web.Services.Domain.Lesson;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LessonRepository : Repository<Lesson>, IRepository<Lesson,int>
	{
		public LessonRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		