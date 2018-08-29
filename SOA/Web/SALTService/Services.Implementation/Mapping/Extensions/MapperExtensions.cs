using System.Collections.Generic;
using AutoMapper;

namespace Asa.Salt.Web.Services.Application.Implementation.Mapping.Extensions
{
    public static class MapperExtensions
    {
        /// <summary>
        /// Converts an entity to its corresponding data contract.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="domainObject">The domain object.</param>
        /// <returns></returns>
        public static TContract ToDataContract<TType, TContract>(this TType domainObject) 
        {
            return Mapper.Map<TType, TContract>(domainObject);
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public static TType ToDomainObject<TContract, TType>(this TContract contract)
        {
            return Mapper.Map<TContract,TType>(contract);
        }

        /// <summary>
        /// Converts an entity to its corresponding data contract.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="domainObject">The domain object.</param>
        /// <returns></returns>
        public static List<TContract> ToDataContract<TType, TContract>(this IEnumerable<TType> domainObject)
        {
            return Mapper.Map<IEnumerable<TType>, List<TContract>>(domainObject);
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public static List<TType> ToDomainObject<TContract, TType>(this IEnumerable<TContract> contract)
        {
           return Mapper.Map<IEnumerable<TContract>, List<TType>>(contract);
        }

    }
}
