using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Domain;
using Data.Model;

using ETLLogging=Domain.ETLLogging;
namespace Data.Repositories
{   
	public partial class ETLLoggingRepository : Repository<ETLLogging>
	{
		public ETLLoggingRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		