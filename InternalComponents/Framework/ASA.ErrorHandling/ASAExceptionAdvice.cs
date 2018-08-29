using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Spring.Aop;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ASA.ErrorHandling
{
    class ASAExceptionAdvice : IThrowsAdvice
    {
        private ILog Logger;

        public ASAExceptionAdvice()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(this.GetType().Name);
            if (Logger.IsInfoEnabled)
            {
                GlobalContext.Properties["stack"] = "";
            }
        }

        public void AfterThrowing(MethodInfo method, Object[] args, Object target, Exception exception)//where T : ASAException
        {
            ASAException tempException;

            if (exception is ASAException)
            {
                tempException = (ASAException)exception;
                if (tempException.ExceptionError_id == null)
                {
                    //set ExceptionError_id if not already set
                    tempException.ExceptionError_id = "GEN0000001";
                }
            }
            else
            {
                try
                {
                    ASAExceptionTranslator afterThrowingTranslator = new ASAExceptionTranslator();
                    tempException = afterThrowingTranslator.Translate(exception);
                }
                catch (ASAException e)
                {
                    tempException = e;
                }
            }

            if (Logger.IsInfoEnabled)
            {
                GlobalContext.Properties["stack"] = Environment.StackTrace;
                Logger.Info(exception.Source + exception.GetType().ToString());
            }

            tempException.Error_call_stack = exception.StackTrace + "/////////" + Environment.StackTrace;

            throw tempException;
        }
    }


}
