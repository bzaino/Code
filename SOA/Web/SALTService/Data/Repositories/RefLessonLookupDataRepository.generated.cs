using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefLessonLookupData=Asa.Salt.Web.Services.Domain.RefLessonLookupData;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefLessonLookupDataRepository : Repository<RefLessonLookupData>, IRepository<RefLessonLookupData,int>
	{
		public RefLessonLookupDataRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		