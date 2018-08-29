namespace Asa.Salt.Web.Common.Types.Unity
{
    public interface ILazyResolver<out TType>
    {
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns></returns>
        TType Resolve();

        /// <summary>
        /// Resolves the specified named instance.
        /// </summary>
        /// <param name="namedInstance">The named instance.</param>
        /// <returns></returns>
        TType Resolve(string namedInstance);
    }
}
