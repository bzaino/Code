namespace Asa.Salt.Web.Services.Jobs.Interfaces
{
    /// <summary>
    /// Interface for supporting the Timer Host Service.
    /// </summary>
    public interface ISupportTimerService
    {
        /// <summary>
        /// Operation which is invoked when the timer interval has expired.
        /// </summary>
        void OnTimerElapsed();

    }
}
