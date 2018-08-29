namespace Asa.Salt.Web.Common.Types.Constants
{
   public static class LoanRecordSources
   {
        /// <summary>
        /// Loans imported from a NSLDS file.
        /// </summary>
        public const int Import = 1;

        /// <summary>
        /// Loans manually entered by the user.
        /// </summary>
        public const int Member = 2;

        /// <summary>
        /// Loans imported by the user in KWYO.
        /// </summary>
        public const int ImportedKWYO = 3;

        /// <summary>
        /// Loans manually entered by the user in KWYO.
        /// </summary>
        public const int MemberKWYO = 4;
        
   };
}
