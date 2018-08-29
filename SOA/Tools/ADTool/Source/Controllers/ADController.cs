using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ADTool.Models;
//using ASA.MembershipProvider.extension;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
//using ADqueryClinet.PersonManagementService;



using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;

using System.Threading;
using System.Threading.Tasks;
using System.Collections;



namespace ADTool.Controllers
{
    public class ADController : Controller
    {
        //
        // GET: /AD/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /AD/Validate
        public ActionResult Validate()
        {
            return View();
        }




        // GET: /AD/Validate
        public ActionResult UpdateUser()
        {
            return View();
        }


        public ActionResult Deactivate()
        {

            return View("Deactivate");
        }

        // GET: /AD/Validate
        [HttpPost]
        public ActionResult ValidateUser(LogOnModel model)
        {
            string emailAdd = model.EmailAddress;
            string User = Membership.GetUserNameByEmail("testUser001@asa.org");
           
            if (string.IsNullOrEmpty(User))
            {
                ViewBag.Message = "User With email address was not found";
            }
            ViewBag.Message = "Welcome " + User;

            return View();
        }
        
		[HttpPost]
        public ActionResult Validate(LogOnModel model)
        {
            string emailAdd = model.EmailAddress;
            string User = Membership.GetUserNameByEmail(emailAdd);

            //AdMember adMember = new AdMember();

            ADUserDetails userModel = new ADUserDetails();

            userModel.UserDetails = new List<UserDetail>();
            UserDetail userDetail = new UserDetail();
            //DateTime CreationDate = adMember.GetUserCreationDate(emailAdd);

            ViewBag.Message = "Welcome " + User;


            //if (CreationDate.Equals(DateTime.MinValue))
            //{
            //    ViewBag.Message = "Welcome " + User
            //        + "<br>" + "Creation Date is Set To Minimum System Date: " + DateTime.MinValue
            //        + "<br>" + GetUserOU(emailAdd);

            //    userDetail.DateOfCreation = "Creation Date is Set To Minimum System Date: " + DateTime.MinValue;
            //    userDetail.EnvironmentName = GetUserOU(emailAdd);
            //    userDetail.UserName = User;
            //    userDetail.UserStatus = GetUserOU(emailAdd);
            //    userModel.UserDetails.Add(userDetail);

                
            //}
            //else
            //{
            //    ViewBag.Message = "Welcome " + User
            //        + " ===============================>  Email Add for Current User is  :  " + emailAdd + ": Was Created On Date: " + adMember.GetUserCreationDate(emailAdd).ToString()
            //        + ".............................>" + GetUserOU(emailAdd);


            //    userDetail.DateOfCreation = "Email Add for Current User is  :  " + emailAdd + ": Was Created On Date: " + adMember.GetUserCreationDate(emailAdd).ToString();
            //    userDetail.EnvironmentName = GetUserOU(emailAdd);
            //    userDetail.UserName = User;
            //    userDetail.UserStatus = GetUserOU(emailAdd);
            //    userModel.UserDetails.Add(userDetail);
            //}


            if (string.IsNullOrEmpty(User))
            {
                ViewBag.Message = "User With email address "+ emailAdd +" was not found";


            }


            return View("ValidateUser", GetUserOU(emailAdd));
        }

        private ADUserDetails GetUserOU(string emailAdd)
        {
			DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://app.extranet.local/ DC=app,DC=extranet,DC=local", "sv_NPedmsacctcre", "XeJpOWC1", AuthenticationTypes.Secure);

            DirectorySearcher search = new DirectorySearcher(myLdapConnection);

           // StringBuilder UserOus = new StringBuilder();


            ADUserDetails userDetails = new ADUserDetails();

            userDetails.EmailAddress = emailAdd;

            // search.Filter = "(objectCategory=Organizational-Unit)";

            // mail is the attribute used to validate user in salt as per jarvien Matthew
           // search.Filter = ("(&(objectclass=user)(objectcategory=person)(mail=" + emailAdd + "))");

            
            //Searching on CN cause CN can be duplicate
            search.Filter = ("(&(objectclass=user)(objectcategory=person)(CN=" + emailAdd + "))");


            try
            {
                SearchResultCollection collectedResult = search.FindAll();

                if (collectedResult.Count.Equals(0))
                {
                    search.Filter = ("(&(objectclass=user)(objectcategory=person)(mail=" + emailAdd + "))");
                    collectedResult = search.FindAll();
                }

                foreach (SearchResult temp in collectedResult)
                {
                    UserDetail userDetail = new UserDetail();
                    userDetail.DateOfCreation = DateTime.Parse(temp.Properties["WhenCreated"][0].ToString()).ToLocalTime().ToString();
                    userDetail.EnvironmentName = GetEnvironmentName(temp.Properties["distinguishedname"][0].ToString());
                    userDetail.UserName = temp.Properties["name"][0].ToString();
                    userDetail.UserPrincipalName = temp.Properties["userPrincipalName"][0].ToString();
                    userDetail.Mail = temp.Properties["mail"][0].ToString();
                    userDetail.CN = temp.Properties["CN"][0].ToString();
                    userDetails.UserDetails.Add(userDetail);
                    
                    //UserOus.Append(temp.Properties["name"][0].ToString());
                    //UserOus.Append(" Belongs to Following Organization Units  ");
                    //UserOus.Append(temp.Properties["distinguishedname"][0].ToString());

                    //foreach (DictionaryEntry de in temp.Properties)
                    //{
                    //    //Console.WriteLine("key " + de.Key);
                    //    //Console.WriteLine("Value " + de.Value);

                    //    //UserOus.Append("key " + de.Key.ToString());
                    //    //UserOus.Append("Value " + de.Value.ToString());

                    //    UserOus.Append(temp.Properties[de.Key.ToString()][0].ToString());
                    //}
                    
                   

                    DirectoryEntry ou = temp.GetDirectoryEntry();
                }
            }
            catch (Exception ex)
            {

                userDetails.ErrorMsg = ex.Message;
            }
            finally
            {

            }

            return userDetails;
        }

        private string GetEnvironmentName(string p)
        {
            
            if (p.Contains("EDMSNP"))
            {
                return "Non-Production";
            }
            else if(p.Contains("EDMS"))
            {
              return "Production";
            }

            else { return p; }
        }

        

        [HttpPost]
        public ActionResult Deactivate(LogOnModel model)
        {
            ViewBag.Title = "Deactivate User";
            string emailAdd = model.EmailAddress;
            
            //testUser001@asa.org
            string UserName = Membership.GetUserNameByEmail(emailAdd);


            if (!string.IsNullOrEmpty(UserName))
            {
                if (Membership.DeleteUser(UserName))
                {
                    ViewBag.Message = UserName + " Was Deleted Successfully";
                }
                else 
                {
                    ViewBag.Message = UserName + " Was Not Deleted";
                }
                
                
            }
            else
            {
                ViewBag.Message = UserName + " was not found to be deleted";
            }

            return View("ValidateUser");
        }


        [HttpPost]
        public ActionResult UpdateUser(LogOnModel model)

        {
            ViewBag.Title = "Update User";
            string emailAdd = model.EmailAddress;
            //testUser001@asa.org
            string UserName = Membership.GetUserNameByEmail(emailAdd);


            if (!string.IsNullOrEmpty(UserName))
            {
                //Update User by membership provider
                //MembershipUser user = Membership.GetUser(UserName);
                //user.Email = "test@asa.org";
                //Membership.UpdateUser(user);

                try
            {
				DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://app.extranet.local/OU=Users,OU=EDMSNP,DC=app,DC=extranet,DC=local", "sv_NPedmsacctcre", "XeJpOWC1", AuthenticationTypes.Secure); ;

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + UserName + ")";
                search.PropertiesToLoad.Add("title");

                SearchResult result = search.FindOne();

                if (result != null)
                {
                    // create new object from search result

                    DirectoryEntry entryToUpdate = result.GetDirectoryEntry();

                    // show existing title

                    Console.WriteLine("Current title   : " + 
                                      entryToUpdate.Properties["title"][0].ToString());

                    Console.Write("\n\nEnter new title : ");

                    // get new title and write to AD

                    String newTitle = Console.ReadLine();

                    entryToUpdate.Properties["title"].Value = newTitle;
                    entryToUpdate.CommitChanges();

                    Console.WriteLine("\n\n...new title saved");
                }

                else Console.WriteLine("User not found!");
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        


       }
            else
            {
                ViewBag.Message = "No user found to be Updated";
            }

            return View("UpdateUser");
        }

    }
}


 