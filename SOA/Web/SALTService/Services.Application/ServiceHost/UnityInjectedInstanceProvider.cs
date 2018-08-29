﻿using Asa.Salt.Web.Services.Application.Implementation.Services;
using Microsoft.Practices.Unity;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    public class UnityInjectedInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer _container;
        /// <summary>
        /// The _contract type
        /// </summary>
        private readonly Type _contractType;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInjectedInstanceProvider"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="contractType">Type of the contract.</param>
        public UnityInjectedInstanceProvider(IUnityContainer container, Type contractType)
        {
            this._container = container;
            this._contractType = contractType;
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _container.Resolve(_contractType);
        }

        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext" /> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            _container.Teardown(instance);
        }



    }
}