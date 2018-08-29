<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="62476eda-1f0d-4fb4-ad0e-6954e2e404a9" namespace="ASA.Web.WTF.Integration.MVC3" xmlSchemaNamespace="urn:ASA.Web.WTF.Integration.MVC3" assemblyName="ASA.Web.WTF.Integration.MVC3" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="ASAIntegration" namespace="ASA.Web.WTF.Integration.MVC3" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="asaIntegration">
      <elementProperties>
        <elementProperty name="Startup" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="startup" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Startup" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="Startup" namespace="ASA.Web.WTF.Integration.MVC3">
      <attributeProperties>
        <attributeProperty name="EnablePreload" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="enablePreload" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="PreloadStoreAuth" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="preloadStoreAuth" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="ContentPreload" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="contentPreload" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/ContentPreload" />
          </type>
        </elementProperty>
        <elementProperty name="MemberContextPreload" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="memberContextPreload" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/MemberContextPreload" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="ContentPreload" namespace="ASA.Web.WTF.Integration.MVC3" xmlItemName="contentItem" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="enabled" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/ContentItem" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ContentItem" namespace="ASA.Web.WTF.Integration.MVC3">
      <attributeProperties>
        <attributeProperty name="RequestPath" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="requestPath" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="MemberContextPreload" namespace="ASA.Web.WTF.Integration.MVC3" xmlItemName="add" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="enabled" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="TestUserLoad" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="testUserLoad" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="TestUserId" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="testUserId" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/ExtendedOption" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ExtendedOption" namespace="ASA.Web.WTF.Integration.MVC3">
      <attributeProperties>
        <attributeProperty name="Key" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="key" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/62476eda-1f0d-4fb4-ad0e-6954e2e404a9/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>