using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using SurveyResponse=Asa.Salt.Web.Services.Domain.SurveyResponse;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class SurveyResponseRepository : Repository<SurveyResponse>, IRepository<SurveyResponse,int>
	{
		public SurveyResponseRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		