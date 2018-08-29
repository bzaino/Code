using System;

namespace Asa.Salt.Web.Services.Domain
{
    public class JellyVisionQuizResponse
    {
        public int MemberId { get; set; }
        public string QuizName { get; set; }
        public string QuizTakenSite { get; set; }
        public string QuizResult { get; set; }
        public string QuizResponse { get; set; }
    }
}