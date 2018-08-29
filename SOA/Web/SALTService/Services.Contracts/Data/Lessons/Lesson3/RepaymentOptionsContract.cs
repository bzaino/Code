using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(DefermentContract))]
    [KnownType(typeof(FasterRepaymentContract))]
    [KnownType(typeof(LowerPaymentContract))]
    [KnownType(typeof(StandardRepaymentContract))]
    public class RepaymentOptionsContract
    {
   
    }
}