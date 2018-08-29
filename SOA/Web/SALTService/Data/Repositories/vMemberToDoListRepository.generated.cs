using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using vMemberToDoList=Asa.Salt.Web.Services.Domain.vMemberToDoList;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class vMemberToDoListRepository : Repository<vMemberToDoList>, IRepository<vMemberToDoList,int>
	{
		public vMemberToDoListRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		