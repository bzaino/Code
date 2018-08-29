using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SALTCoursesWSClient;

namespace SALTCoursesWSClient.Tests
{
    [TestClass]
    public class MoodleUserTests
    {
        [TestMethod]
        public void BuildCoursesFromConfigTest_Should_Return_AllCourses()
        {
            //Arrange
            int memberId = 1940;
            var returnList = new List<CourseModel>();
            MoodleUser mu = new MoodleUser(memberId.ToString());

            //Act
            List<CourseModel> courses = mu.BuildCoursesFromConfig();

            //Assert
            Assert.IsNotNull(courses);
            Assert.AreEqual(12, courses.Count);
            Assert.AreEqual("Budgeting", courses[0].idnumber);
            Assert.AreEqual("Budgeting", courses[0].shortname);
            Assert.AreEqual(26, courses[0].id);
            Assert.AreEqual("101-25847", courses[0].contentid);

        }
    }
}
