namespace ASA.Web.Services.LessonsService.DataContracts.Lesson3
{
    public class RepaymentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepaymentOptions"/> class.
        /// </summary>
        public RepaymentOptions()
        {
            FasterRepayment = new FasterRepayment();
            LowerPayments = new LowerPayment();
            Deferment = new Deferment();
            StandardRepayment = new StandardRepayment();
        }

        /// <summary>
        /// Gets or sets faster repayment options achieved by paying more each month.
        /// </summary>
        /// <value>
        /// Faster repayment options.
        /// </value>
        public FasterRepayment FasterRepayment { get; set; }

        /// <summary>
        /// Gets or sets repayment deferment options.
        /// </summary>
        /// <value>
        /// Payment deferment options.
        /// </value>
        public Deferment Deferment { get; set; }

        /// <summary>
        /// Gets or sets the lower payments.
        /// </summary>
        /// <value>
        /// The lower payments.
        /// </value>
        public LowerPayment LowerPayments { get; set; }

        /// <summary>
        /// Gets or sets the standard repayment.
        /// </summary>
        /// <value>
        /// The standard repayment.
        /// </value>
        public StandardRepayment StandardRepayment { get; set; }
    }
}