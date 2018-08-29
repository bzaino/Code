using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using VLCQuestionText=Asa.Salt.Web.Services.Domain.VLCQuestionText;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCQuestionTextRepository : Repository<VLCQuestionText>, IRepository<VLCQuestionText,int>
	{
		public VLCQuestionTextRepository(SALTEntities context,ICacheProvider<SALTEntities, VLCQuestionText> cacheProvider) : base(context,cacheProvider)
		{

		}
	}
}
		