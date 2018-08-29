using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefActivityType=Domain.RefActivityType;
namespace Data.Repositories
{   
	public partial class RefActivityTypeRepository : Repository<RefActivityType>
	{
		public RefActivityTypeRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		