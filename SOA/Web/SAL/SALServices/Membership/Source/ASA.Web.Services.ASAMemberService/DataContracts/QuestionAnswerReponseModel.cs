using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class QuestionAnswerReponseModel
    {
        [Required]
        public string memberId { get; set; }
        [Required]
        public IList<MemberQAModel> choicesList { get; set; }
    }
}
