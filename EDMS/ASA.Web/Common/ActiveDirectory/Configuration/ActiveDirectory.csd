<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="379a5def-5e81-4de5-9a57-f0a9e4418d85" namespace="ASA.Web.Utility.ActiveDirectory.Configuration" xmlSchemaNamespace="urn:ASA.Web.Utility.ActiveDirectory.Configuration" assemblyName="ASA.Web.Utility.ActiveDirectory.Configuration" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ActiveDirectoryConnectorSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="activeDirectoryConnectorSection">
      <elementProperties>
        <elementProperty name="UserID" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="userID" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/UserID" />
          </type>
        </elementProperty>
        <elementProperty name="Password" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/Password" />
          </type>
        </elementProperty>
        <elementProperty name="UserContainer" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="userContainer" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/UserContainer" />
          </type>
        </elementProperty>
        <elementProperty name="Domain" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="domain" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/Domain" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="UserContainer">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="UserID">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="Password">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="Domain">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/379a5def-5e81-4de5-9a57-f0a9e4418d85/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>