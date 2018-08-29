using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using VLCQuestionResponse=Asa.Salt.Web.Services.Domain.VLCQuestionResponse;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCQuestionResponseRepository : Repository<VLCQuestionResponse>, IRepository<VLCQuestionResponse,int>
	{
		public VLCQuestionResponseRepository(SALTEntities context,ICacheProvider<SALTEntities, VLCQuestionResponse> cacheProvider) : base(context,cacheProvider)
		{

		}
	}
}
		