using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson3
{
    public class Favorite
    {
        /// <summary>
        /// Gets or sets the favorite id.
        /// </summary>
        /// <value>
        /// The favorite id.
        /// </value>
        public int FavoriteId { get; set; }

        /// <summary>
        /// Gets or sets the name of the repayment.
        /// </summary>
        /// <value>
        /// The name of the repayment.
        /// </value>
        public string RepaymentName { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }


        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="favorite">The favorite.</param>
        /// <returns></returns>
        public bool Equals(Favorite favorite)
        {
            return favorite.RepaymentName.Equals(this.RepaymentName, StringComparison.OrdinalIgnoreCase);
        }
    }
}