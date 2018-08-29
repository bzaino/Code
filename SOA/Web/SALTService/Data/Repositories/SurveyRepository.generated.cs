using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using Survey=Asa.Salt.Web.Services.Domain.Survey;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class SurveyRepository : Repository<Survey>, IRepository<Survey,int>
	{
		public SurveyRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		