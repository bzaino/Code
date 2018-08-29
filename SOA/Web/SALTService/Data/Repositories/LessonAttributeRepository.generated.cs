using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using LessonAttribute=Asa.Salt.Web.Services.Domain.LessonAttribute;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class LessonAttributeRepository : Repository<LessonAttribute>, IRepository<LessonAttribute,int>
	{
		public LessonAttributeRepository(SALTEntities context,ICacheProvider<SALTEntities, LessonAttribute> cacheProvider) : base(context,cacheProvider)
		{

		}
	}
}
		