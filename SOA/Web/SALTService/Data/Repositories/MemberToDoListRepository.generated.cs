using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using MemberToDoList=Asa.Salt.Web.Services.Domain.MemberToDoList;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class MemberToDoListRepository : Repository<MemberToDoList>, IRepository<MemberToDoList,int>
	{
		public MemberToDoListRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		