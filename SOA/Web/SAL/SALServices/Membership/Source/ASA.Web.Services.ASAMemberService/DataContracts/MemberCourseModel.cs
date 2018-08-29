using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberCourseModel
    {
        [Required]
        public int CourseID { get; set; }
        public string ShortName { get; set; } //Moodle course short name
        public string FullName { get; set; }
        public string IdNumber { get; set; } //Moodle course identifier by name, similar to shortname
        public int UserID { get; set; } //Moodle userid
        public int ClassID { get; set; } //Elis calssid
        public bool CompleteStatus { get; set; } //moodle course completion status
        public double Grade { get; set; }
        public int Credits { get; set; }
        public string ContentID { get; set; } //salt content tile id for this course
    }
}
