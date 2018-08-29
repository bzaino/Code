using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Data.Caching.Interfaces;
using Domain;
using Data.Model;

using RefCourse=Domain.RefCourse;
namespace Data.Repositories
{   
	public partial class RefCourseRepository : Repository<RefCourse>
	{
		public RefCourseRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		