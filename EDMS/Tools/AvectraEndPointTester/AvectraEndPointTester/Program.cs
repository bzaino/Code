using System;
using System.ServiceModel; 
using xWebProxy;
using System.Xml;


/* SUCCESSFUL RUN
 
 C:\>avectraendpointtester64 xwebsalt 9502C8FA http://adevweb022/xWeb/secure/netForumXML.asmx "Rivier College" 4813de79-3b23-4b67-a9b3-2a9ccc4442e8
  Format is {application name}.exe {avectra user id} {avectra password} {web service endpoint} {customer name} {customer key}

AVECTRA-SALT ENPOINT TESTER


Authenticating.....
Authenticated ... token = 5fb2d637-7bd5-4846-ba32-e9bc077554e9
Inserting new test user ....
Inserted indiviual successfully .....
Inserted individual id: 9230a62e-de4a-483c-9011-b13c3f20d49a last name: AvectraT
estUser11061194755
Get Query.....
Get Query.....Success
UpdateFacadeObject.....
UpdateFacadeObject.....Success
InsertFacadeObject.....
InsertFacadeObject.....Success
Set Token and Access Code.....
Token: D6FBEBFE-50D4-4EB8-B7CA-AE15BF77DA85 Access Code: AE71FE
Set Token and Access Code.....Success


ALL TESTS PASSED!  

*/
namespace AvectraEndPointTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\nAVECTRA-SALT ENPOINT TESTER\n\n");
            if (args.Length == 0 || args.Length < 4)
            {
                Console.WriteLine("Invalid call.\nFormat is {application name}.exe {avectra user id} {avectra password} {web service endpoint} {customer name} {customer key}");
                return;
            }
            

            xWebProxy.netForumXMLSoapClient client = GetClient(args[2]);

            AuthorizationToken token = new AuthorizationToken();
            string authKey = string.Empty;
            IndividualType savedIndividual;

            #region Authenticate
            try
            {
                Console.WriteLine("Authenticating.....");
                token = client.Authenticate(args[0], args[1], out authKey);
                Console.WriteLine("Authenticated ... token = {0} ", token.Token);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Authentication failed check user id and password. \n\n{0} ", ex.Message);
                return;
            }
            #endregion

            #region Web Insert
            try
            {
                Console.WriteLine("Inserting new test user ....");
                IndividualType individual = new IndividualType()
                {
                    Individual = new Individual_Individual_DataObjectType
                    {
                        ind_first_name = "AvectraPingTestUser",
                        ind_last_name = "AvectraTestUser" + DateTime.Now.ToString("MMddyyHmmss"),
                        ind_int_code = "student",
                        ind_ods_flag_ext = 1,
                        ind_token_ext = Guid.NewGuid().ToString(),
                        ind_token_expiration_date_ext = DateTime.Today.AddYears(1).ToString("d"),
                        ind_access_code_ext = Guid.NewGuid().ToString().Substring(0, 6)

                    }
                     ,
                    Customer = new Individual_Customer_DataObjectType
                    {
                        cst_org_name_dn = args[3]
                        
                    }
                      ,
                    Organization_XRef = new Individual_Organization_XRef_DataObjectType
                    {

                    },
                    Organization = new Individual_Organization_DataObjectType
                    {
                        org_cst_key = args[4]
                    }
                };
                savedIndividual = client.WEBIndividualInsert(ref token, individual);
                Console.WriteLine("Inserted indiviual successfully .....");
                Console.WriteLine(string.Format("Inserted individual id: {0} last name: {1}", savedIndividual.Individual.ind_cst_key, savedIndividual.Individual.ind_last_name));
            }
            catch (Exception ex)
            {
                Console.WriteLine("WebInsert failed: \n\n{0} ", ex.Message);
                return;
            }

            #endregion

            #region Get Query
            try
            {
                Console.WriteLine("Get Query.....");
                XmlNode results;
                results = client.GetQuery(ref token, "Individual @TOP 1", "ind_add_date,cst_cxa_key,ad2__adr_line1,ad2__adr_city,ad2__adr_state,ad2__adr_zip,eml_address,co_address.adr_line_1,mbr_installment_frequency,mbr_installment_frequency_for_renewal,ixo_enrollment_status_a03_code_ext,ixo_grade_level_a04_code_ext,mbr_join_date,ind_cst_key, ind_token_ext, ind_token_ext,ind_token_expiration_date_ext,ind_first_name,ind_first_name,ind_last_name,ind_dob,ind_mid_name,cst_eml_address", string.Format("ind_cst_key='{0}'", savedIndividual.Individual.ind_cst_key), "");
                if (results.SelectSingleNode("./@recordReturn").Value == "1")
                {
                    Console.WriteLine("Get Query.....Success");
                }
                else
                {
                    Console.WriteLine("Get Query.....Failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Query failed: \n\n{0} ", ex.Message);
                return;
            }
            #endregion

            #region UpdateFacadeObject
            try
            {
                Console.WriteLine("UpdateFacadeObject.....");
                string xmlUpdate = @"<IndividualObjects>
	            <IndividualObject>
		            <ind_mid_name>Test</ind_mid_name>
	            </IndividualObject>
            </IndividualObjects>";
                XmlNode results;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlUpdate);

                results = client.UpdateFacadeObject(ref token, "Individual", savedIndividual.Individual.ind_cst_key, doc.SelectSingleNode("/*"));
                if (results.SelectSingleNode("./@recordReturn").Value == "1")
                {
                    Console.WriteLine("UpdateFacadeObject.....Success");
                }
                else
                {
                    Console.WriteLine("UpdateFacadeObject.....Failed");
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("UpdateFacadeObject failed: \n\n{0} ", ex.Message);
                return;
            }
            #endregion

            #region InsertFacadeObject
            try
            {
                Console.WriteLine("InsertFacadeObject.....");
                string xmlUpdate = string.Format(@"<ASAWebActivityObjects>
	            <ASAWebActivityObject>
                    <a13_ind_cst_key>{0}</a13_ind_cst_key>
                    <a13_a12_code>Welcome Email</a13_a12_code>                                                 
                    <a13_send_email_flag>0</a13_send_email_flag>
	            </ASAWebActivityObject>
            </ASAWebActivityObjects>", savedIndividual.Individual.ind_cst_key);

                XmlNode results;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlUpdate);

                results = client.InsertFacadeObject(ref token, "ASAWebActivity", doc.SelectSingleNode("/*"));

                if (results.SelectSingleNode("./@recordReturn").Value == "1")
                {
                    Console.WriteLine("InsertFacadeObject.....Success");
                }
                else
                {
                    Console.WriteLine("InsertFacadeObject.....Failed");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("InsertFacadeObject failed: \n\n{0} ", ex.Message);
                return;
            }
            #endregion

            #region SetTokenAndAccessCode
            try
            {
                Parameter[] parameters = new Parameter[1]{ 
            new Parameter(){ Name="ind_cst_key", Value=savedIndividual.Individual.ind_cst_key }
            };
                XmlNode results;
                Console.WriteLine("Set Token and Access Code.....");
                results = client.ExecuteMethod(ref token, "netForumASA", "SetTokenAndAccessCode", parameters);
                if (results.SelectSingleNode(".").Name == "NewUserTokenValid")
                {


                    //We've got a valid token. XML may have changed. Ignore schema change errors 
                    try
                    {
                        Console.WriteLine("Token: {0} Access Code: {1}", results.SelectSingleNode("./Individual/@WebUserToken").Value, results.SelectSingleNode("./Individual/@WebUserAccessCode").Value);
                    }
                    catch (Exception ex)
                    {


                    }

                    Console.WriteLine("Set Token and Access Code.....Success");
                }
                else
                {
                    Console.WriteLine("Set Token and Access Code.....Failed");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("SetTokenAndAccessCode failed: \n\n{0} ", ex.Message);
                return;
            }
            #endregion


            Console.WriteLine("\n\nALL TESTS PASSED! ");
        }
        static netForumXMLSoapClient GetClient(string endpointStr)
        {
            //<basicHttpBinding>
            //    <binding name="netForumXMLSoap" closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard" maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536" messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered" useDefaultWebProxy="true">
            //        <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="65536000" />
            //        <security mode="Transport">
            //            <transport clientCredentialType="None" proxyCredentialType="None" realm="" />
            //            <message clientCredentialType="UserName" algorithmSuite="Default" />
            //        </security>
            //    </binding>
            //</basicHttpBinding>

            //Set up the binding element to match the app.config settings '
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "netForumXMLSoap";
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(1);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferSize = 65536;
            binding.MaxBufferPoolSize = 524288;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;

            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 5242880;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 5242880;

            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Transport.Realm = "";
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;



            EndpointAddress endpoint = new EndpointAddress(endpointStr);


            return new netForumXMLSoapClient(binding, endpoint);


        }
    }
}
