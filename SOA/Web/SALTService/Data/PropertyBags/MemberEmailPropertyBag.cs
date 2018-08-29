using System.Globalization;
using System.Linq;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;

namespace Asa.Salt.Web.Services.Data.PropertyBags
{
    public class MemberEmailPropertyBag : PropertyBag<string, string>
    {
        /// <summary>
        /// The member repository
        /// </summary>
        private readonly IRepository<Domain.Member, int> _memberRepository;

        /// <summary>
        /// The db context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberEmailPropertyBag"/> class.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public MemberEmailPropertyBag(int userId)
            : base()
        {
            _dbContext = new SALTEntities();

            _memberRepository = new Repository<Domain.Member>(_dbContext);

            var member = _memberRepository.Get(m => m.MemberId == userId,null,"School").FirstOrDefault();

            Add("cst_name_cp", () =>  member.FirstName );
            Add("cst_key", () => member.MemberId.ToString(CultureInfo.InvariantCulture));
            Add("ind_first_name", () => member.FirstName);
            Add("cst_recno", () => member.MemberId.ToString(CultureInfo.InvariantCulture));
            Add("ind_add_date", () => member.MemberStartDate.ToShortDateString());
            Add("cst_org_name_dn", () => member.School!=null?member.School.SchoolName:"N/A");

            
        }


    }



}

