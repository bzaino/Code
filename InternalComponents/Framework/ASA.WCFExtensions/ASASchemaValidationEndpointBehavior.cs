
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Schema;
using System.Xml;

namespace ASA.WCFExtensions
{
    public class SchemaValidationEndpointBehavior : IEndpointBehavior
    {
        XmlSchemaSet schemaSet;
        bool validateRequest;
        bool validateReply;
        
        public SchemaValidationEndpointBehavior(String messageContractSchemasPath, bool inspectRequest, bool inspectReply)
        {
            this.schemaSet = new XmlSchemaSet();

            Uri baseSchema = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            string location = new Uri(baseSchema, messageContractSchemasPath).ToString();
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


        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            SchemaValidationMessageInspector inspector = new SchemaValidationMessageInspector(schemaSet, validateRequest, validateReply, false);
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            SchemaValidationMessageInspector inspector = new SchemaValidationMessageInspector(schemaSet, validateRequest, validateReply, false);
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}