using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common.xWeb;
using log4net;
using System.ServiceModel;
using System.Xml;
using System.Threading;



namespace ASA.Web.Services.Common
{
    public class xWebWrapper : ASA.Web.Services.Common.IxWebWrapper
    {
        private const string CLASSNAME = "ASA.Web.Services.Common.xWebWrapper";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);
        private static AuthorizationToken _authToken = null;
        //private IServiceLocator _serviceLocator = null;


        //private void SetDefaultLocator()
        //{

        //    //Spring.Context.IApplicationContext springContainer = Spring.Context.Support.ContextRegistry.GetContext();
        //    //_serviceLocator = new CommonServiceLocator.SpringAdapter.SpringServiceLocatorAdapter(springContainer);
        //    var simpleInjectorContainer = new SimpleInjector.Container();
        //    simpleInjectorContainer.Register<InetForumXMLSoapClient>(() => new netForumXMLSoapClient());
        //    _serviceLocator = new CommonServiceLocator.SimpleInjectorAdapter.SimpleInjectorServiceLocatorAdapter(simpleInjectorContainer);

        //}

        ///// <summary>
        ///// Test Constructor
        ///// </summary>
        ///// <param name="serviceLocator"></param>
        //public xWebWrapper(IServiceLocator serviceLocator)
            
        //{
        //    if (serviceLocator != null)
        //    {
        //        _serviceLocator = serviceLocator;
        //    }
        //    else
        //    {
        //        SetDefaultLocator();
        //    }

        //    String logMethodName = ".ctor(IServiceLocator serviceLocator) - ";
        //    _log.Debug(logMethodName + "Begin Method");

        //    GetAuthorizationToken();

        //    _log.Debug(logMethodName + "End Method");

        //}
       
        public xWebWrapper()
        {

           // SetDefaultLocator();

            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            GetAuthorizationToken();

            _log.Debug(logMethodName + "End Method");
        }

        #region Auth Token
        private static bool retryAuth(netForumXMLSoapClient xWebClient)
        {
            string strAuthResult;

            bool ret = false;

            for (int i = 0; i < Config.XWebRetryAttemps; i++)
            {
                _log.Error("Retrying authentication attempt#: " + i.ToString());
                try
                {
                    _authToken = xWebClient.Authenticate(Config.XWebLogin, Config.XWebPassword, out strAuthResult);
                }
                catch {}
                
                if (_authToken != null)
                {
                    return true;
                }
                Thread.Sleep(Config.XWebRetryWait);
            }

            return ret;
        }
        private void getNewAuthToken()
        {
            netForumXMLSoapClient xWebClient = null;
            string strAuthResult;
            String logMethodName = ".getNewAuthToken() - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {

                _log.Debug(logMethodName + "Creating xWebClient.");
                xWebClient = new netForumXMLSoapClient();

                _log.Debug(logMethodName + "successfully created xWebClient.");

                _log.Debug(logMethodName + "Authiencticating Client");
                _authToken = xWebClient.Authenticate(Config.XWebLogin, Config.XWebPassword, out strAuthResult);
                _log.Info(logMethodName + "The current xWeb AuthToken being used by this application is: " + _authToken.Token);
            }
            catch (TimeoutException te)
            {

                _log.Error(logMethodName + "TimeoutException on xWeb service call.  make sure endpoint is reachable and configured correctly.", te);
                if (!retryAuth(xWebClient))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw te;
                }


            }
            ////we do not want to retry these. These are errors that will not be fixed by a retry, i.e. invalid credentials
            //catch (FaultException fe)
            //{
            //    _log.Error(logMethodName + "There has been an error for an xWeb Auth operation: ", fe);
            //    throw fe;
            //}
            catch (CommunicationException ce)
            {
                _log.Error(logMethodName + "CommunicationException on xWeb service call.  make sure endpoint is reachable and configured correctly... this may be due to Token expiration", ce);
                if (!retryAuth(xWebClient))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw ce;
                }
            }
            finally
            {
                if (xWebClient.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(xWebClient);
                }
            }
            _log.Debug(logMethodName + "End Method");

        }

        public AuthorizationToken GetAuthorizationToken()
        {
            String logMethodName = ".GetAuthorizationToken() - ";
            _log.Debug(logMethodName + "Begin Method");

            // TODO: add "OR" for case when time elapsed is greater than the Timeout value of the Token. how do I find the Timeout value?
            // - This would be done with a configuration element, request time-outs to the backend tend to vary
            // by the service being implmented. I think for WCF calls it is configurable but it needs to be researched.
            if (_authToken == null)
            {
                getNewAuthToken();
            }

            _log.Debug(logMethodName + "End Method");

            return _authToken;
        }
        #endregion

        #region GetQuery
        private bool retryGet(netForumXMLSoapClient xWebClient, out XmlNode queryResults, string objectName, string columnList, string whereClause, string orderBy)
        {
            bool ret = false;
            queryResults = null;
            for (int i = 0; i < Config.XWebRetryAttemps; i++)
            {
                _log.Error("Retrying getQuery attempt#: " + i.ToString());
                try
                {
                    queryResults = xWebClient.GetQuery(ref _authToken, objectName, columnList, whereClause, orderBy);
                    if (queryResults != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    //ignore exception and retry
                }
                Thread.Sleep(Config.XWebRetryWait);
            }

            return ret;
        }
        public XmlNode GetQuery(string objectName, string columnList, string whereClause, string orderBy)
        {
            String logMethodName = ".GetQuery(string objectName, string columnList, string whereClause, string orderBy) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode queryResults = null;
            queryResults = getQuery(objectName, columnList, whereClause, orderBy, false);

            _log.Debug(logMethodName + "End Method");

            return queryResults;
        }

        private XmlNode getQuery(string objectName, string columnList, string whereClause, string orderBy, bool isRetry)
        {
            String logMethodName = ".getQuery(string objectName, string columnList, string whereClause, string orderBy, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode queryResults = null;
            netForumXMLSoapClient xWebClient = null;

            try
            {
                _log.Debug(logMethodName + "Creating netForumXMLSoapClient.");
                xWebClient = new netForumXMLSoapClient();
                _log.Debug(logMethodName + "netForumXMLSoapClient Created Successfully");

                logGetQuery(objectName, columnList, whereClause, orderBy, isRetry);
                _log.Debug(logMethodName + "Calling netForumXMLSoapClient.GetQuery(ref ASA.Web.Services.Common.xWeb.AuthorizationToken AuthorizationToken, string szObjectName, string szColumnList, string szWhereClause, string szOrderBy)");
                queryResults = xWebClient.GetQuery(ref _authToken, objectName, columnList, whereClause, orderBy);
                _log.Debug(logMethodName + "xWebClient.GetQuery(...) completed");
            }
            catch (TimeoutException te)
            {

                _log.Error(logMethodName + "getQuery: TimeoutException on xWeb service call.  make sure endpoint is reachable and configured correctly.", te);

                if (!retryGet(xWebClient, out queryResults, objectName, columnList, whereClause, orderBy))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw te;
                }


            }
            //we do not want to retry these. These are errors that will not be fixed by a retry, i.e. invalid credentials
            //catch (FaultException fe)
            //{
            //    _log.Error(logMethodName + "There has been an error for an xWeb GET operation: " + objectName, fe);
            //    throw fe;
            //}
            catch (CommunicationException ce)
            {

                _log.Error(logMethodName + "There has been an error attempting to communicate with XWeb, attempting retry (if enabled)", ce);


                // This will cause the AuthorizationToken to be refreshed, 
                // and it will perform ONE retry call to xWeb to GetQuery() following this CommunicationException
                if (isRetry == false && ce.Message.StartsWith("System.Web.Services.Protocols.SoapException: Failed"))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    _log.Info(logMethodName + "getQuery: Going to retry after getting a new Auth Token");
                    getNewAuthToken();

                    _log.Debug(logMethodName + "Calling netForumXMLSoapClient.GetQuery(ref ASA.Web.Services.Common.xWeb.AuthorizationToken AuthorizationToken, string szObjectName, string szColumnList, string szWhereClause, string szOrderBy)");
                    queryResults = getQuery(objectName, columnList, whereClause, orderBy, true);
                    _log.Debug(logMethodName + "xWebClient.GetQuery(...) completed");

                }
                else
                {
                    _log.Warn(logMethodName + "getQuery: CommunicationException on xWeb service call.  make sure endpoint is reachable and configured correctly... this may be due to Token expiration", ce);
                    if (!retryGet(xWebClient, out queryResults, objectName, columnList, whereClause, orderBy))
                    {
                        ProxyHelper.HandleServiceException(xWebClient);
                        throw ce;
                    }

                }

            }
            finally
            {
                if (xWebClient.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(xWebClient);
                }
            }

            _log.Debug(logMethodName + "End Method");
            return queryResults;
        }

        private void logGetQuery(string objectName, string columnList, string whereClause, string orderBy, bool isRetry)
        {
            StringBuilder sb = new StringBuilder();
            if (objectName != null)
            {
                sb.Append(",  objectName = ");
                sb.Append(objectName);
            }
            if (columnList != null)
            {
                sb.Append(",  columnList = ");
                sb.Append(columnList);
            }
            if (whereClause != null)
            {
                sb.Append(",  whereClause = ");
                sb.Append(whereClause);
            }
            if (orderBy != null)
            {
                sb.Append(",  orderBy = ");
                sb.Append(orderBy);
            }
            sb.Append(",  isRetry = ");
            sb.Append(isRetry.ToString());

            _log.Debug("GetQuery paramters ==> " + sb.ToString());
        }

        #endregion

        #region InsertFacadeObject
        private bool retryInsert(netForumXMLSoapClient xWebClient, out XmlNode queryResults, string objectName, XmlNode oNode)
        {
            bool ret = false;
            queryResults = null;
            for (int i = 0; i < Config.XWebRetryAttemps; i++)
            {
                _log.Error("Retrying insertFacadeObject attempt#: " + i.ToString());
                try
                {
                    queryResults = xWebClient.InsertFacadeObject(ref _authToken, objectName, oNode);
                    if (queryResults != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    //ignore exception and retry
                }

                Thread.Sleep(Config.XWebRetryWait);
            }

            return ret;
        }
        public XmlNode InsertFacadeObject(string objectName, XmlNode oNode)
        {
            String logMethodName = ".InsertFacadeObject(string objectName, XmlNode oNode) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode insertResults = null;
            insertResults = insertFacadeObject(objectName, oNode, false);

            _log.Debug(logMethodName + "End Method");

            return insertResults;
        }

        private XmlNode insertFacadeObject(string objectName, XmlNode oNode, bool isRetry)
        {
            String logMethodName = ".insertFacadeObject(string objectName, XmlNode oNode, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode queryResults = null;
            netForumXMLSoapClient xWebClient = null;

            try
            {
                _log.Debug(logMethodName + "Creating netForumXMLSoapClient.");
                xWebClient = new netForumXMLSoapClient();
                _log.Debug(logMethodName + "netForumXMLSoapClient created successfuly.");

                _log.Debug(logMethodName + "Calling netForumXMLSoapClient.InsertFacadeObject(ref ASA.Web.Services.Common.xWeb.AuthorizationToken AuthorizationToken, string szObjectName, System.Xml.XmlNode oNode)");
                logFacadeObject("INSERT", objectName, "", oNode, isRetry);
                queryResults = xWebClient.InsertFacadeObject(ref _authToken, objectName, oNode);
                _log.Debug(logMethodName + "xWebClient.InsertFacadeObject(...) completed");
            }
            catch (TimeoutException te)
            {
                ProxyHelper.HandleServiceException(xWebClient);
                _log.Error(logMethodName + "insertFacadeObject: TimeoutException on xWeb service call.  make sure endpoint is reachable and configured correctly.", te);
                if (!retryInsert(xWebClient, out queryResults, objectName, oNode))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw te;
                }


            }
            //we do not want to retry these. These are errors that will not be fixed by a retry, i.e. invalid credentials
            //catch (FaultException fe)
            //{
            //    _log.Error(logMethodName + "There has been an error for an xWeb INSERT operation: ", fe);
            //    throw fe;
            //}
            catch (CommunicationException ce)
            {

                _log.Error(logMethodName + "There has been an error attempting to communicate with XWeb, attempting retry (if enabled)", ce);


                // This will cause the AuthorizationToken to be refreshed, 
                // and it will perform ONE retry call to xWeb to insertFacadeObject() following this CommunicationException
                if (isRetry == false && ce.Message.StartsWith("System.Web.Services.Protocols.SoapException: Failed"))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    _log.Warn(logMethodName + "Failed to insert data, Communication Exception: Going to retry after getting a new Auth Token");
                    getNewAuthToken();

                    _log.Debug(logMethodName + "Calling netForumXMLSoapClient.InsertFacadeObject(ref ASA.Web.Services.Common.xWeb.AuthorizationToken AuthorizationToken, string szObjectName, System.Xml.XmlNode oNode)");
                    queryResults = insertFacadeObject(objectName, oNode, true);
                    _log.Debug(logMethodName + "xWebClient.InsertFacadeObject(...) completed");

                    //What happens if this throws an exception?
                }
                else
                {
                    _log.Warn(logMethodName + "insertFacadeObject: CommunicationException on xWeb service call.  make sure endpoint is reachable and configured correctly... this may be due to Token expiration", ce);
                    if (!retryInsert(xWebClient, out queryResults, objectName, oNode))
                    {
                        ProxyHelper.HandleServiceException(xWebClient);
                        throw ce;
                    }

                }
            }
            finally
            {
                if (xWebClient.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(xWebClient);
                }
            }

            _log.Debug(logMethodName + "End Method");
            return queryResults;
        }

        private void logFacadeObject(string insertOrUpdate, string objectName, string key, XmlNode oNode, bool isRetry)
        {
            String logMethodName = ".logFacadeObject(string insertOrUpdate, string objectName, string key, XmlNode oNode, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            StringBuilder sb = new StringBuilder();
            sb.Append(insertOrUpdate);

            if (objectName != null)
            {
                sb.Append(",  objectName = ");
                sb.Append(objectName);
            }
            if (key != null)
            {
                sb.Append(",  key = ");
                sb.Append(key);
            }
            if (oNode != null)
            {
                sb.Append(",  oNode = ");
                sb.Append(oNode.ToString());
            }
            sb.Append(",  isRetry = ");
            sb.Append(isRetry.ToString());

            _log.Debug(logMethodName + "FacadeObject paramters ==> " + sb.ToString());
            _log.Debug(logMethodName + "End Method");

        }
        #endregion

        #region UpdateFacadeObject
        private bool retryUpdate(netForumXMLSoapClient xWebClient, out XmlNode queryResults, string objectName, XmlNode oNode, string objectKey)
        {
            bool ret = false;
            queryResults = null;
            for (int i = 0; i < Config.XWebRetryAttemps; i++)
            {
                _log.Error("Retrying updateFacadeObject attempt#: " + i.ToString());
                try
                {
                    queryResults = xWebClient.UpdateFacadeObject(ref _authToken, objectName, objectKey, oNode);
                    if (queryResults != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    //ignore exception and retry
                }

                Thread.Sleep(Config.XWebRetryWait);
            }

            return ret;
        }
        public XmlNode UpdateFacadeObject(string objectName, string objectKey, XmlNode oNode)
        {
            String logMethodName = ".UpdateFacadeObject(string objectName, string objectKey, XmlNode oNode) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode updateResults = null;
            updateResults = updateFacadeObject(objectName, objectKey, oNode, false);

            _log.Debug(logMethodName + "End Method");

            return updateResults;
        }

        private XmlNode updateFacadeObject(string objectName, string objectKey, XmlNode oNode, bool isRetry)
        {
            String logMethodName = ".updateFacadeObject(string objectName, string objectKey, XmlNode oNode, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode queryResults = null;
            netForumXMLSoapClient xWebClient = null;

            try
            {
                //throw new CommunicationException();
                _log.Debug(logMethodName + "Creating netForumXMLSoapClient.");
                xWebClient = new netForumXMLSoapClient();
                _log.Debug(logMethodName + "netForumXMLSoapClient created successfuly.");

                _log.Debug(logMethodName + "Calling netForumXMLSoapClient.UpdateFacadeObjectResponse ASA.Web.Services.Common.xWeb.netForumXMLSoap.UpdateFacadeObject(ASA.Web.Services.Common.xWeb.UpdateFacadeObjectRequest request)");
                logFacadeObject("UPDATE", objectName, objectKey, oNode, isRetry);
                queryResults = xWebClient.UpdateFacadeObject(ref _authToken, objectName, objectKey, oNode);
                _log.Debug(logMethodName + "netForumXMLSoapClient.UpdateFacadeObject(...) completed");
            }
            catch (TimeoutException te)
            {

                _log.Error(logMethodName + "updateFacadeObject: TimeoutException on xWeb service call.  make sure endpoint is reachable and configured correctly.", te);
                if (!retryUpdate(xWebClient, out queryResults, objectName, oNode, objectKey))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw te;
                }


            }
            //we do not want to retry these. These are errors that will not be fixed by a retry, i.e. invalid credentials
            //catch (FaultException fe)
            //{
            //    _log.Error(logMethodName + "There has been an error for an xWeb UPDATE operation: ", fe);
            //    throw fe;
            //}
            catch (CommunicationException ce)
            {

                _log.Error(logMethodName + "There has been an error attempting to communicate with XWeb, attempting retry (if enabled)", ce);


                // This will cause the AuthorizationToken to be refreshed, 
                // and it will perform ONE retry call to xWeb to GetQuery() following this CommunicationException
                if (isRetry == false && ce.Message.StartsWith("System.Web.Services.Protocols.SoapException: Failed"))
                {

                    ProxyHelper.HandleServiceException(xWebClient);
                    _log.Info(logMethodName + "updateFacadeObject: Going to retry after getting a new Auth Token");
                    getNewAuthToken();

                    _log.Debug(logMethodName + "Calling netForumXMLSoapClient.UpdateFacadeObjectResponse ASA.Web.Services.Common.xWeb.netForumXMLSoap.UpdateFacadeObject(ASA.Web.Services.Common.xWeb.UpdateFacadeObjectRequest request)");
                    queryResults = updateFacadeObject(objectName, objectKey, oNode, true);
                    _log.Debug(logMethodName + "netForumXMLSoapClient.UpdateFacadeObject(...) completed");

                    //what happens if this throws an exception?

                }
                else
                {
                    _log.Warn(logMethodName + "updateFacadeObject: CommunicationException on xWeb service call.  make sure endpoint is reachable and configured correctly... this may be due to Token expiration", ce);
                    if (!retryUpdate(xWebClient, out queryResults, objectName, oNode, objectKey))
                    {
                        ProxyHelper.HandleServiceException(xWebClient);
                        throw ce;
                    }

                }
            }
            finally
            {
                if (xWebClient.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(xWebClient);
                }
            }

            _log.Debug(logMethodName + "End Method");
            return queryResults;
        }
        #endregion

        #region ExecuteMethod
        private bool retryExecute(netForumXMLSoapClient xWebClient, out XmlNode queryResults, string serviceName, string methodName, Parameter[] parameters)
        {
            bool ret = false;
            queryResults = null;
            for (int i = 0; i < Config.XWebRetryAttemps; i++)
            {
                _log.Error("Retrying ExecuteMethod attempt#: " + i.ToString());
                try
                {
                    queryResults = xWebClient.ExecuteMethod(ref _authToken, serviceName, methodName, parameters);
                    if (queryResults != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    //ignore exception and retry
                }

                Thread.Sleep(Config.XWebRetryWait);
            }

            return ret;
        }
        public XmlNode ExecuteMethod(string serviceName, string methodName, Parameter[] parameters)
        {
            String logMethodName = ".ExecuteMethod(string serviceName, string methodName, Parameter[] parameters) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode executeResults = null;
            executeResults = executeMethod(serviceName, methodName, parameters, false);

            _log.Debug(logMethodName + "End Method");
            return executeResults;
        }
        private XmlNode executeMethod(string serviceName, string methodName, Parameter[] parameters, bool isRetry)
        {
            String logMethodName = ".executeMethod(string serviceName, string methodName, Parameter[] parameters, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            XmlNode queryResults = null;
            netForumXMLSoapClient xWebClient = null;

            try
            {
                _log.Debug(logMethodName + "Creating netForumXMLSoapClient.");
                xWebClient = new netForumXMLSoapClient();
                _log.Debug(logMethodName + "netForumXMLSoapClient created successfuly.");

                _log.Debug(logMethodName + "Calling netForumXMLSoapClient.ExecuteMethod(ASA.Web.Services.Common.xWeb.ExecuteMethodRequest request)");
                logExecuteMethod(serviceName, methodName, parameters, isRetry);
                queryResults = xWebClient.ExecuteMethod(ref _authToken, serviceName, methodName, parameters);
                _log.Debug(logMethodName + "netForumXMLSoapClient.ExecuteMethod(...) completed");
            }
            catch (TimeoutException te)
            {

                _log.Error(logMethodName + "executeMethod: TimeoutException on xWeb service call.  make sure endpoint is reachable and configured correctly.", te);
                if (!retryExecute(xWebClient, out queryResults, serviceName, methodName, parameters))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    throw te;
                }
            }
            //we do not want to retry these. These are errors that will not be fixed by a retry, i.e. invalid credentials
            //catch (FaultException fe)
            //{
            //    _log.Error(logMethodName + "There has been an error for an xWeb Execute operation: ", fe);
            //    throw fe;
            //}
            catch (CommunicationException ce)
            {

                // This will cause the AuthorizationToken to be refreshed, 
                // and it will perform ONE retry call to xWeb to GetQuery() following this CommunicationException
                if (isRetry == false && ce.Message.StartsWith("System.Web.Services.Protocols.SoapException: Failed"))
                {
                    ProxyHelper.HandleServiceException(xWebClient);
                    _log.Info(logMethodName + "executeMethod: Going to retry after getting a new Auth Token");
                    getNewAuthToken();
                    queryResults = executeMethod(serviceName, methodName, parameters, true);
                }
                else
                {
                    _log.Error(logMethodName + "executeMethod: CommunicationException on xWeb service call.  make sure endpoint is reachable and configured correctly... this may be due to Token expiration", ce);
                    if (!retryExecute(xWebClient, out queryResults, serviceName, methodName, parameters))
                    {
                        ProxyHelper.HandleServiceException(xWebClient);
                        throw ce;
                    }

                }

            }
            finally
            {
                if (xWebClient.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(xWebClient);
                }
            }

            _log.Debug(logMethodName + "End Method");

            return queryResults;
        }

        private void logExecuteMethod(string serviceName, string methodName, Parameter[] parameters, bool isRetry)
        {
            String logMethodName = ".logExecuteMethod(string serviceName, string methodName, Parameter[] parameters, bool isRetry) - ";
            _log.Debug(logMethodName + "Begin Method");

            StringBuilder sb = new StringBuilder();

            if (serviceName != null)
            {
                sb.Append(",  serviceName = ");
                sb.Append(serviceName);
            }
            if (methodName != null)
            {
                sb.Append(",  methodName = ");
                sb.Append(methodName);
            }
            if (parameters != null)
            {
                foreach (Parameter p in parameters)
                {
                    sb.Append(",  ParamName = ");
                    sb.Append(p.Name);
                    sb.Append(",  ParamValue = ");
                    sb.Append(p.Value);
                }
            }
            sb.Append(",  isRetry = ");
            sb.Append(isRetry.ToString());

            _log.Debug(logMethodName + "FacadeObject paramters ==> " + sb.ToString());
            _log.Debug(logMethodName + "End Method");

        }
        #endregion
    }
}
