using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefAlertType=Domain.RefAlertType;
namespace Data.Repositories
{   
	public partial class RefAlertTypeRepository : Repository<RefAlertType>
	{
		public RefAlertTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		