using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using VLCQuestion=Asa.Salt.Web.Services.Domain.VLCQuestion;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class VLCQuestionRepository : Repository<VLCQuestion>, IRepository<VLCQuestion,int>
	{
		public VLCQuestionRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		