using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using SurveyOption=Asa.Salt.Web.Services.Domain.SurveyOption;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class SurveyOptionRepository : Repository<SurveyOption>, IRepository<SurveyOption,int>
	{
		public SurveyOptionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		