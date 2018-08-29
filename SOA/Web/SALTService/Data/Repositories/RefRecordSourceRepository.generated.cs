using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefRecordSource=Domain.RefRecordSource;
namespace Data.Repositories
{   
	public partial class RefRecordSourceRepository : Repository<RefRecordSource>
	{
		public RefRecordSourceRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		