using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using LessonStep=Asa.Salt.Web.Services.Domain.LessonStep;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LessonStepRepository : Repository<LessonStep>, IRepository<LessonStep,int>
	{
		public LessonStepRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		