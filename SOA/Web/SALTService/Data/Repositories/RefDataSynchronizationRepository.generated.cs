using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Domain;
using Data.Model;

using RefDataSynchronization=Domain.RefDataSynchronization;
namespace Data.Repositories
{   
	public partial class RefDataSynchronizationRepository : Repository<RefDataSynchronization>
	{
		public RefDataSynchronizationRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		