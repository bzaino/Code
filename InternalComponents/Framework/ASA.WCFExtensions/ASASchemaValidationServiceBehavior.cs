using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Schema;
using ASA.Log.ServiceLogger;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace ASA.WCFExtensions
{
    public class ASASchemaValidationServiceBehavior : IServiceBehavior
    {
        static readonly IASALog _Log = ASALogManager.GetLogger(typeof(ASASchemaValidationServiceBehavior));

        XmlSchemaSet schemaSet;
        bool validateRequest;
        bool validateReply;
        

        public ASASchemaValidationServiceBehavior(String messageContractSchemasPath, bool inspectRequest, bool inspectReply)
        {
            this.schemaSet = new XmlSchemaSet();

            Uri baseSchema = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            string location = new Uri(baseSchema, "." + messageContractSchemasPath).ToString();
            using (XmlTextReader xmlTextReader = new XmlTextReader(location))
            {
                XmlSchema schema = XmlSchema.Read(
                    xmlTextReader, null
                    );
                this.schemaSet.Add(schema);
            }
            
            this.validateReply = inspectReply;
            this.validateRequest = inspectRequest;
        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            _Log.Info("Apply Schema Validation DispatchBehavior");
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    _Log.InfoFormat("Adding to {0}", endpointDispatcher.EndpointAddress.Uri);


                    SchemaValidationMessageInspector svmi = new SchemaValidationMessageInspector(schemaSet, validateRequest, validateReply,false);

                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(svmi);


                }

            } 
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
