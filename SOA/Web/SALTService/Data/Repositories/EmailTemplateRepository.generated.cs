using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Configuration.Caching;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;

using EmailTemplate=Asa.Salt.Web.Services.Domain.EmailTemplate;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class EmailTemplateRepository : Repository<EmailTemplate>, IRepository<EmailTemplate,int>
	{
		public EmailTemplateRepository(SALTEntities context,ICacheProvider<SALTEntities, EmailTemplate> cacheProvider, IApplicationCachingConfiguration cachingConfiguration) : base(context,cacheProvider,cachingConfiguration)
		{

		}
	}
}
		