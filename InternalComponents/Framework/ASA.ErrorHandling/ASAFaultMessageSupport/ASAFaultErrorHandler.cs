using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using ASA.DataAccess.SEL.Interfaces;
using ASA.Common;
using ASA.Log.ServiceLogger;
using ASA.BusinessEntities.SEL;
using ASA.TID;

namespace ASA.ErrorHandling
{
    public class ASAFaultErrorHandler : IErrorHandler, IServiceBehavior
    {
        public static ISELDao _mSELDao = null;
        public IASALog Log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IErrorHandler Members

        // Provide a fault. The Message fault parameter can be replaced, or set to
        // null to suppress reporting a fault.
        public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
        {
            _mSELDao = (ISELDao)ContextHelper.GetContextObject("SELDAO");

            ASAException translatedException = new ASAException();

            //catch all, in case error comes into EHF without being translated
            if (error is ASAException)
            {
                translatedException = (ASAException)error;
            }
            else
            {
                ASAExceptionTranslator afterThrowingTranslator = new ASAExceptionTranslator();
                translatedException = afterThrowingTranslator.Translate(error);
            }

            string tidCorrelationID = ASATIDHelper.GetTIDCorrelationID();

            if (error != null && error is NoMatchingObjectException)
            {
                msg = BuildErrorMessage<ASAFaultDetail>(version, "Server", tidCorrelationID, translatedException.Error_FaultString, translatedException.Error_DetailMessage);
            }
            else if (error != null && error is ServiceRequestValidationException)
            {
                msg = BuildErrorMessage<ASAFaultDetail>(version, "Server", tidCorrelationID, translatedException.Error_FaultString + ": " + translatedException.Error_DetailMessage, translatedException.Error_DetailMessage);
            }
            else if (error != null && error is ServiceReplyValidationException)
            {
                msg = BuildErrorMessage<ASAFaultDetail>(version, "Server", tidCorrelationID, translatedException.Error_FaultString, translatedException.Error_DetailMessage);
            }
            else if (error != null && error is ASADemogBusinessException)
            {
                //QC 1690-1693 handle new exception types
                msg = BuildErrorMessage<ASADemogFaultDetail>(version, "Server", tidCorrelationID, translatedException.Error_FaultString + ": " + translatedException.Error_DetailMessage, translatedException.Error_DetailMessage);
            }
            else if (error != null && error is ASABusinessException)
            {
                //QC 1690-1693 handle new exception types
                msg = BuildErrorMessage<ASABusinessFaultDetail>(version, "Server", tidCorrelationID, translatedException.Error_FaultString + ": " + translatedException.Error_DetailMessage, translatedException.Error_DetailMessage);
            }
            else if (error != null && error is Exception)
            {
                switch (translatedException.ExceptionType)
                {
                    case "ASADataAccessException":
                    case "ASAUnknownException":
                    case "ASA.ExcErrCodeUnavail":
                        {
                            msg = BuildErrorMessage<ASAFaultDetail>(version, "Server", tidCorrelationID,
                                  translatedException.BusinessDescription, translatedException.Original_Message);
                            break;
                        }
                    default:
                        {
                            msg = BuildErrorMessage<ASAFaultDetail>(version, "Server", tidCorrelationID,
                                  translatedException.Error_FaultString, translatedException.Error_DetailMessage);
                            break;
                        }
                }
            }

            Log.Error(msg);

            #region add message to the LogException tables

            string payload = string.Empty;

            if (Payload.ContainsMessagePayLoad(tidCorrelationID))
            {
                payload = Payload.GetMessagePayLoad(tidCorrelationID);
            }

            //LogEvent logEventRec = new LogEvent();
            LogException logExceptionRec = new LogException();

            logExceptionRec.CreatedBy = (ASATIDHelper.GetTIDUsername() != "") ? ASATIDHelper.GetTIDUsername() : "ASA_USER";
            logExceptionRec.CreatedDate = DateTime.Now;
            logExceptionRec.Payload = payload.ToString();
            logExceptionRec.ExceptionStack = error.StackTrace;
            logExceptionRec.Correlationid = new Guid(tidCorrelationID);
            logExceptionRec.ExceptionErrorid = translatedException.ExceptionError_id;

            long eventID;

            try
            {
                //_mSELDao.AddLogExceptionRecord(logExceptionRec, out eventID);
                Log.Error(payload);
                Log.Error(logExceptionRec);
            }

            catch (Exception ex)
            {
                //if there is an error logging the record to the DB, write payload to log file
                Log.Error(payload);
            }
            #endregion

        }

        /// <summary>
        /// Build error message using exception info
        /// </summary>
        /// <typeparam name="T">Fault Contract Type</typeparam>
        /// <returns></returns>

        public Message BuildErrorMessage<T>(MessageVersion version, string faultCode, string tidCorrelationID, string message, string detailMessage)
            where T : new()
        {
            T faultDetail = new T();

            FieldInfo[] baseClassFields = faultDetail.GetType().GetFields(
                BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo fi in baseClassFields)
            {
                switch (fi.Name)
                {
                    case "_IdField":
                        fi.SetValue(faultDetail, new Guid(tidCorrelationID));
                        break;

                    case "_MessageField":
                        fi.SetValue(faultDetail, detailMessage);
                        break;
                }
            }

            FaultException<T> fex = new FaultException<T>(faultDetail, message, new FaultCode(faultCode));
            MessageFault fault = fex.CreateMessageFault();

            return Message.CreateMessage(version, fault, fex.Action);
        }

        // HandleError. Log an error, then allow the error to be handled as usual.
        // Return true if the error is considered as already handled

        public bool HandleError(Exception error)
        {
            return true;
        }

        #endregion

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channDisp in serviceHostBase.ChannelDispatchers)
            {
                channDisp.ErrorHandlers.Add(this);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //do nothing for now

            //foreach (var svcEndpoint in serviceDescription.Endpoints)
            //{
            //    if (svcEndpoint.Contract.Name != "IMetadataExchange")
            //    {
            //        foreach (var opDesc in svcEndpoint.Contract.Operations)
            //        {
            //            if (opDesc.Faults.Count == 0)
            //            {
            //                string msg =
            //                    string.Format("ServiceErrorHandlerBehavior requires a FaultContract(typeof(ApplicationFault)) on each operation contract. The {0} contains no FaultContracts.", opDesc.Name);
            //                throw new InvalidOperationException(msg);
            //            }

            //            var fcExists = from fc in opDesc.Faults
            //                           where fc.DetailType == typeof (ApplicationFault)
            //                           select fc;

            //            if (fcExists.Count() == 0)
            //            {
            //                string msg = string.Format("ServiceErrorHandlerBehavior requires a FaultContract(typeof(ApplicationFault)) on each operation contract.");
            //                throw new InvalidOperationException(msg);
            //            }
            //        }
            //    }
            //}
        }

        #endregion
    }

    //This allows the error handler to be added to a service via configuration
    public class ASAFaultErrorHandlerBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ASAFaultErrorHandler); }
        }

        protected override object CreateBehavior()
        {
            return new ASAFaultErrorHandler();
        }

    }

    // This attribute can be used to install a custom error handler for a service
    public sealed class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        Type errorHandlerType;

        public ErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }

        public Type ErrorHandlerType
        {
            get { return this.errorHandlerType; }
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
            {
                errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
            }
            catch (MissingMethodException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must have a public empty constructor.", e);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must implement System.ServiceModel.Dispatcher.IErrorHandler.", e);
            }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }
    }


}
