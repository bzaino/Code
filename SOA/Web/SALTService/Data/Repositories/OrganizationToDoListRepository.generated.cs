using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using OrganizationToDoList=Asa.Salt.Web.Services.Domain.OrganizationToDoList;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class OrganizationToDoListRepository : Repository<OrganizationToDoList>, IRepository<OrganizationToDoList,int>
	{
		public OrganizationToDoListRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		