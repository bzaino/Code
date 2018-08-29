using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefEmailTemplate=Domain.RefEmailTemplate;
namespace Data.Repositories
{   
	public partial class RefEmailTemplateRepository : Repository<RefEmailTemplate>
	{
		public RefEmailTemplateRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		