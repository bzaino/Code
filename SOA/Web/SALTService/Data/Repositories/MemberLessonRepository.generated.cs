using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberLesson=Asa.Salt.Web.Services.Domain.MemberLesson;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberLessonRepository : Repository<MemberLesson>, IRepository<MemberLesson,int>
	{
		public MemberLessonRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		