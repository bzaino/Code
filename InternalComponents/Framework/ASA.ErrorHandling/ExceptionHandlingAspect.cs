using System;
using System.Collections.Generic;
using System.Text;
using Spring.Context;

using Spring.Aop.Support;
using Spring.Aop.Framework;
using Spring.Aop;

namespace ASA.ErrorHandling
{
    public class ExceptionHandlingAspect
    {
        private static ASAExceptionAdvice aASAExceptionAdvice = new ASAExceptionAdvice();
        //just in case ASAExceptionAdvice was not implemrnted
        internal ASAExceptionAdvice ASAExceptionAdvice
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the proxy by applying throw advices. The methods identified by the
        /// <code>methodRE</code> regular expression will be intercepted by the throw advice.
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="methodRE"></param>
        /// <returns></returns>
        public static object GetProxy(object target, string methodRE)
        {
            try
            {
                SdkRegularExpressionMethodPointcut reMethodPointcut = new SdkRegularExpressionMethodPointcut(methodRE);

                ProxyFactory proxyFactory = new ProxyFactory(target);
                DefaultPointcutAdvisor exceptionHandlingAdvisor =
                    new DefaultPointcutAdvisor(reMethodPointcut, aASAExceptionAdvice);
                proxyFactory.AddAdvisor(exceptionHandlingAdvisor);
                return proxyFactory.GetProxy();
            }
            catch (Exception ex)
            {
                return new ASAException("ASAExceptionAdvice failed  " + ex.Message);
            }
        }
        /// <summary>
        /// Gets the proxy by applying throw advices. 
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object GetProxy(object target)
        {
            try
            {
                ProxyFactory proxyFactory = new ProxyFactory(target);
                DefaultPointcutAdvisor exceptionHandlingAdvisor =
                    new DefaultPointcutAdvisor(aASAExceptionAdvice);
                proxyFactory.AddAdvisor(exceptionHandlingAdvisor);
                return proxyFactory.GetProxy();
            }
            catch (Exception ex)
            {
                return new ASAException("ASAExceptionAdvice failed  " + ex.Message);
            }
        }
    }
}
