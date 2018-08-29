using System;
using System.Linq;
using System.Data.Objects;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Data.Model;
using Asa.Salt.Web.Services.Data.Model.Database;

using PaymentReminder=Asa.Salt.Web.Services.Domain.PaymentReminder;
namespace Asa.Salt.Web.Services.Data.Repositories
{   
	public partial class PaymentReminderRepository : Repository<PaymentReminder>, IRepository<PaymentReminder,int>
	{
		public PaymentReminderRepository(SALTEntities context) : base(context)
		{

		}
	}
}
		