using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RefLessonLookupDataType=Asa.Salt.Web.Services.Domain.RefLessonLookupDataType;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RefLessonLookupDataTypeRepository : Repository<RefLessonLookupDataType>, IRepository<RefLessonLookupDataType,int>
	{
		public RefLessonLookupDataTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		