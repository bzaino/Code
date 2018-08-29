using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefEnrollmentStatu=Domain.RefEnrollmentStatu;
namespace Data.Repositories
{   
	public partial class RefEnrollmentStatuRepository : Repository<RefEnrollmentStatu>
	{
		public RefEnrollmentStatuRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		