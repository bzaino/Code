using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefGradeLevel=Domain.RefGradeLevel;
namespace Data.Repositories
{   
	public partial class RefGradeLevelRepository : Repository<RefGradeLevel>
	{
		public RefGradeLevelRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		