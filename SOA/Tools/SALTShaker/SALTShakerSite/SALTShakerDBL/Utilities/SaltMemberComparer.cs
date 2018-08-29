using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.DAL.Utilities
{
    public class SaltMemberComparer : IEqualityComparer<SaltMemberModel>
    {
        public Nullable<int> MemberID { get; set; }

        public Nullable<int> SALTMemberID { get; set; }

        public String EmailAddress { get; set; }

        public static readonly SaltMemberComparer Instance = new SaltMemberComparer();

        // We don't need any more instances
        private SaltMemberComparer() { }

        public int GetHashCode(SaltMemberModel p)
        {
            //Get hash code for the MemberID field if it is not null.
            //int hashMemberID = MemberID == null ? 0 : MemberID.GetHashCode();

            //Get hash code for the Code field.
            //int hashSALTMemberID = SALTMemberID.GetHashCode();

            int hashEmailAddress = p.EmailAddress.GetHashCode();

            return hashEmailAddress;

            //Calculate the hash code for the product.
            //return hashMemberID ^ hashSALTMemberID;
        }

        public bool Equals(SaltMemberModel s, SaltMemberModel p)
        {
            if (Object.ReferenceEquals(s, p))
            {
                return true;
            }
            if (Object.ReferenceEquals(s, null) ||
                Object.ReferenceEquals(p, null))
            {
                return false;
            }
            bool comparisonResult = (s.EmailAddress == p.EmailAddress); //|| (s.SALTMemberID == p.MemberID);
            Console.WriteLine(s.EmailAddress + ", " + p.EmailAddress);

            return comparisonResult;
        }
    }
}
