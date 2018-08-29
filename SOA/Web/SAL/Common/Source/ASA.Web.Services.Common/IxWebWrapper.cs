using System;
namespace ASA.Web.Services.Common
{
    public interface IxWebWrapper
    {
        System.Xml.XmlNode ExecuteMethod(string serviceName, string methodName, ASA.Web.Services.Common.xWeb.Parameter[] parameters);
        ASA.Web.Services.Common.xWeb.AuthorizationToken GetAuthorizationToken();
        System.Xml.XmlNode GetQuery(string objectName, string columnList, string whereClause, string orderBy);
        System.Xml.XmlNode InsertFacadeObject(string objectName, System.Xml.XmlNode oNode);
        System.Xml.XmlNode UpdateFacadeObject(string objectName, string objectKey, System.Xml.XmlNode oNode);
    }
}
