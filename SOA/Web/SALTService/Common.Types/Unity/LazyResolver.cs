using Microsoft.Practices.Unity;

namespace Asa.Salt.Web.Common.Types.Unity
{
    public class LazyResolver<TType> : ILazyResolver<TType>
    {
        /// <summary>
        /// The unity container
        /// </summary>
        readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyResolver{TType}"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public LazyResolver(IUnityContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns></returns>
        public TType Resolve()
        {
            return _container.Resolve<TType>();
        }

        /// <summary>
        /// Resolves the specified named instance.
        /// </summary>
        /// <param name="namedInstance">The named instance.</param>
        /// <returns></returns>
        public TType Resolve(string namedInstance)
        {
            return _container.Resolve<TType>(namedInstance);
        }
    }
}
