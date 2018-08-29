using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using RecordSource=Asa.Salt.Web.Services.Domain.RecordSource;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class RecordSourceRepository : Repository<RecordSource>, IRepository<RecordSource,int>
	{
		public RecordSourceRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		