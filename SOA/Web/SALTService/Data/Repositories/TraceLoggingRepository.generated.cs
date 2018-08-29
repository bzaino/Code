using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Domain;
using Data.Model;

using TraceLogging=Domain.TraceLogging;
namespace Data.Repositories
{   
	public partial class TraceLoggingRepository : Repository<TraceLogging>
	{
		public TraceLoggingRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		