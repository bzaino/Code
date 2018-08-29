
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ASA.Web.Services.LessonsService
{
        [ServiceContract]
    public class Lessons
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson1User/{id}", Method = "POST")]
        public object Lesson1User(Guid id)
        {
            return null;
        }

       [OperationContract]
       [WebInvoke(UriTemplate = "Lesson1User/{redirectUrl}", Method = "GET")]
        public void AssociateRegisteredUser(string redirectUrl)
        {
           // AsaMemberAdapter memberAdapter = new AsaMemberAdapter();
           //int memberId = memberAdapter.GetMemberIdFromContext();

           //if (memberId>0)
           // {
           //     User currentUser = db.Users.Find(userGuid);
           //     if (currentUser == null)
           //     {
           //         // We couldn't find the currently logged in lesson user; this shouldn't happen, something went terribly wrong
           //         return Request.CreateResponse(HttpStatusCode.NotFound);
           //     }

           //     User individualUser = db.Users.FirstOrDefault(u => u.IndividualId == individualId);
           //     if (individualUser == null)
           //     {
           //         // We couldn't find a lesson user for that IndividualId, this is the members first time through Lessons
           //         currentUser.IndividualId = individualId;

           //         db.Entry(currentUser).State = EntityState.Modified;

           //         try
           //         {
           //             db.SaveChanges();
           //         }
           //         catch (DbUpdateConcurrencyException)
           //         {
           //             return Request.CreateResponse(HttpStatusCode.NotFound);
           //         }
           //     }
           //     else if (currentUser.IndividualId != individualId)
           //     {
           //         // We have both an anonymous lesson user and an existing user; we need to merge them.

           //         try
           //         {
           //             var lessonsDAL = new LessonsDAL(db);

           //             lessonsDAL.MergeUserData(currentUser, individualUser);

           //             var userGuidCookie = new CookieHeaderValue("UserGuid", individualUser.UserId.ToString());
           //             userGuidCookie.Path = "/";
           //             newCookies.Add(userGuidCookie);
           //         }
           //         catch (DbUpdateConcurrencyException)
           //         {
           //             return Request.CreateResponse(HttpStatusCode.NotFound);
           //         }

           //     }

           //     redirectUrl = redirectUrl.Replace('!', '/');
           //     if (Uri.IsWellFormedUriString(redirectUrl, UriKind.RelativeOrAbsolute))
           //     {
           //         if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
           //         {
           //             redirectUrl = string.Format("{0}://{1}{2}", Request.RequestUri.Scheme, Request.RequestUri.Authority, redirectUrl);
           //         }

           //         var redirectResponse = Request.CreateResponse(HttpStatusCode.Found);
           //         redirectResponse.Headers.Location = new Uri(redirectUrl);
           //         redirectResponse.Headers.AddCookies(newCookies);
           //         return redirectResponse;
           //     }

           //     var response = Request.CreateResponse(HttpStatusCode.OK);
           //     response.Headers.AddCookies(newCookies);
           //     return response;
           // }

           //return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}