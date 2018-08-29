using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SALTCoursesWSClient
{
    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string auth { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string department { get; set; }
        public string idnumber { get; set; }
        public int firstaccess { get; set; }
        public int lastaccess { get; set; }
        public string description { get; set; }
        public int descriptionformat { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string profileimageurlsmall { get; set; }
        public string profileimageurl { get; set; }
        public List<CustomfieldModel> customfields { get; set; }
    }

    public class CustomfieldModel
    {
        public string type { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public string shortname { get; set; }
    }

    public class MoodleGetUsersResponseModel
    {
        public List<UserModel> users { get; set; }
        public List<MoodleWarningModel> warnings { get; set; }
    }

    public class MoodleWarningModel
    {
        public string item { get; set; }
        public int itemid { get; set; }
        public string warningcode { get; set; }
        public string message { get; set; }
    }

    public class MoodleCreateUserResponseModel
    {
        public int id { get; set; }
        public string username { get; set; }
    }

    public class MoodleExceptionModel
    {
        public string exception { get; set; }
        public string errorcode { get; set; }
        public string message { get; set; }
        public string debuginfo { get; set; }
    }

    public class ElisUsersetEnrollmentCreateResponseModel
    {
        public string messagecode { get; set; }
        public string message { get; set; }
        public ElisUsersetRecord record { get; set; } 
    }

    public class ElisUsersetRecord
    {
        public string clusterid { get; set; }
        public string userid { get; set; }
        public string plugin { get; set; }
        public bool leader { get; set; }
    }

    public class ElisClassEnrolmentUpdateResponseModel
    {
        public string messagecode { get; set; }
        public string message { get; set; }
        public CourseModel record { get; set; }
    }

    public class CourseModel
    {
        public int id { get; set; } //moodle course id
        public string shortname { get; set; } //Moodle course short name
        public string fullname { get; set; }
        public string idnumber { get; set; } //Moodle course identifier by name, similar to shortname
        public int classid { get; set; }
        public int userid { get; set; }
        public int completestatusid { get; set; } //moodle course completion status by id
        public double grade { get; set; }
        public int credits { get; set; }
        public string contentid { get; set; } //salt content tile id for this course
    }
}