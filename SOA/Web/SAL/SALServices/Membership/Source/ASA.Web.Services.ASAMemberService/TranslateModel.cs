using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.ASAMemberService
{
    public static class MappingExtensions
    {
        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="memberContract">The member contract.</param>
        /// <returns></returns>
        public static ASAMemberModel ToDomainObject(this MemberContract memberContract)
        {
            if (memberContract == null)
                return null;

            return new ASAMemberModel()
            {
                LastName = memberContract.LastName,
                MembershipId = memberContract.MemberId.ToString(CultureInfo.InvariantCulture),
                FirstName = memberContract.FirstName,
                ActiveDirectoryKey = memberContract.ActiveDirectoryKey.ToString(),
                ActivationStatusFlag = memberContract.IsMemberActive,
                EnrollmentStatus = memberContract.EnrollmentStatus != null ? memberContract.EnrollmentStatus.EnrollmentStatusCode : string.Empty,
                GradeLevel = memberContract.GradeLevel != null ? memberContract.GradeLevel.GradeLevelCode : string.Empty,
                PrimaryEmailKey = memberContract.EmailAddress,
                PrimaryOrganizationKey = memberContract.MemberOrganizations != null && memberContract.MemberOrganizations.Count() > 0 ? memberContract.MemberOrganizations[0].OrganizationId.ToString() : null,
                InvitationToken = memberContract.InvitationToken.ToString(),
                Organizations = memberContract.MemberOrganizations.ToDomainObject(),
                OrganizationProducts = memberContract.OrganizationProducts.ToDomainObject(),
                Emails = new List<MemberEmailModel>() { new MemberEmailModel() { EmailAddress = memberContract.EmailAddress, IsPrimary = true } },
                Roles = memberContract.MemberRoles.ToDomainObject(),
                Products = memberContract.MemberProducts.ToDomainObject(),
                MemberShipFlag = memberContract.MemberOrganizations != null && memberContract.MemberOrganizations.Any(o => o.IsContracted == true),
                DisplayName = !string.IsNullOrWhiteSpace(memberContract.DisplayName) ? memberContract.DisplayName : memberContract.FirstName,
                //Used by loginmanager.js
                IndividualId = memberContract.ActiveDirectoryKey.ToString(),
                Source = memberContract.RegistrationSourceId.ToString(CultureInfo.InvariantCulture),
                ContactFrequency = !memberContract.IsContactAllowed,
                CommunityDisplayName = memberContract.CommunityDisplayName,
                IsCommunityActive = memberContract.IsCommunityActive,
                WelcomeEmailSent = memberContract.WelcomeEmailSent,
                USPostalCode = memberContract.USPostalCode,
                YearOfBirth = memberContract.YearOfBirth,
                OrganizationIdForCourses = memberContract.OrganizationIdForCourses > 0 ? memberContract.OrganizationIdForCourses.ToString() : null,
		        DashboardEnabled = memberContract.DashboardEnabled
            };
        }

        /// <summary> 
        /// To the domain object for member organization.
        /// </summary> 
        /// <param name="organizationContracts">member organization contracts.</param> 
        /// <returns></returns> 
        public static List<MemberOrganizationModel> ToDomainObject(this MemberOrganizationContract[] organizationContracts)
        {
            var toReturn = new List<MemberOrganizationModel>() { };
            
            if (organizationContracts != null)
            {
                foreach (var item in organizationContracts)
                {
                    var model = new MemberOrganizationModel();

                    //MemberOrganization Data
                    model.MemberId = item.MemberID;
                    model.OrganizationId = item.RefOrganizationID;
                    model.OrganizationExternalID = item.OrganizationExternalID;
                    model.ExpectedGraduationYear = item.ExpectedGraduationYear;
                    model.ReportingId = item.SchoolReportingID;
                    model.EffectiveStartDate = item.EffectiveStartDate;
                    model.EffectiveEndDate = item.EffectiveEndDate;

                    model.IsPrimary = true;
                    model.Brand = !string.IsNullOrEmpty(item.OrganizationLogoName) ? item.OrganizationLogoName : "nologo";
                    model.SchoolType = item.RefSALTSchoolTypeID;

                    //RefOrganization Data
                    model.OrganizationName = item.OrganizationName;
                    model.IsContracted = item.IsContracted;
                    model.OECode = item.OECode;
                    model.BranchCode = item.BranchCode;
                    model.OrganizationLogoName = item.OrganizationLogoName;
                    model.OrganizationAliases = item.OrganizationAliases;
                    model.RefSALTSchoolTypeID = item.RefSALTSchoolTypeID;
                    model.IsLookupAllowed = item.IsLookupAllowed;
                    model.RefOrganizationTypeId = item.RefOrganizationTypeID;
                    model.OrganizationTypeExternalId = item.OrganizationTypeExternalID;

                    toReturn.Add(model);
                }
            }

            return toReturn;
        }

        /// <summary> 
        /// To the domain object for member role.
        /// </summary> 
        /// <param name="roleContracts">member role contracts.</param> 
        /// <returns></returns> 
        public static List<MemberRoleModel> ToDomainObject(this MemberRoleContract[] roleContracts)
        {
            var toReturn = new List<MemberRoleModel>() { };            

            if (roleContracts != null)
            {
                foreach (var item in roleContracts)
                {
                    var model = new MemberRoleModel() { };
                    model.RoleId = item.RefMemberRole.RefMemberRoleID.ToString();
                    model.RoleName = item.RefMemberRole.RoleName;
                    model.RoleDescription = item.RefMemberRole.RoleDescription;
                    model.IsMemberRoleActive = item.IsMemberRoleActive;
                    toReturn.Add(model);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="refOrganizationContract">The refOrganization contract.</param>
        /// <returns></returns>
        public static BasicOrgInfoModel ToDomainObject(this RefOrganizationContract refOrganizationContract)
        {
            return new BasicOrgInfoModel()
            {
                OeCode = refOrganizationContract.OPECode,
                OeBranch = refOrganizationContract.BranchCode,
                OrgName = refOrganizationContract.OrganizationName,
                OrgLogo = string.IsNullOrWhiteSpace(refOrganizationContract.OrganizationLogoName) ? "nologo" : refOrganizationContract.OrganizationLogoName,
                ExtOrgId = refOrganizationContract.OrganizationExternalID,
                OrgId = refOrganizationContract.RefOrganizationID,
                IsBranded = !string.IsNullOrEmpty(refOrganizationContract.OrganizationLogoName),
                IsSchool = refOrganizationContract.RefOrganizationTypeID == 4 ? true : false
            };
        }

        /// <summary>
        /// To the domain object. Map SALT Service RefOrganizationProductContract list to SAL OrganizationProductModel list
        /// </summary>
        /// <param name="refOrgProductContractList">The List of RefOrganizationProductContract items.</param>
        /// <returns>List<OrganizationProductModel></returns>
        public static List<OrganizationProductModel> ToDomainObject(this RefOrganizationProductContract[] refOrgProductContractList)
        {
            var toReturn = new List<OrganizationProductModel>() { };

            if (refOrgProductContractList != null)
            {
                foreach (var item in refOrgProductContractList)
                {
                    var model = new OrganizationProductModel();
                    model.OrganizationId = item.RefOrganizationID;
                    model.ProductID = item.RefProductID;
                    model.IsOrgProductActive = item.IsRefOrganizationProductActive;
                    model.ProductName = item.RefProduct.ProductName;
                    model.ProductTypeID = item.RefProduct.RefProductTypeID;
                    toReturn.Add(model);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// To the domain object for member profile questions' response
        /// </summary>
        /// <param name="profileAnswerContract"></param>
        /// <returns></returns>
        public static MemberProfileResponseModel ToDomainObject(this MemberProfileAnswerContract profileAnswerContract)
        {
            return new MemberProfileResponseModel
            {
                MemberID = profileAnswerContract.MemberID,
                ProfileAnswerExternalID = profileAnswerContract.RefProfileAnswerID,
                CustomValue = profileAnswerContract.CustomValue
            };
        }

        public static List<MemberProfileResponseModel> ToDomainObject(this MemberProfileAnswerContract[] profAnsContList)
        {
            var toReturn = new List<MemberProfileResponseModel>() { };
            foreach (var ans in profAnsContList)
            {
                var profRespModel = new MemberProfileResponseModel() { };
                profRespModel.MemberID = ans.MemberID;
                profRespModel.ProfileAnswerExternalID = ans.RefProfileAnswerID;
                profRespModel.CustomValue = ans.CustomValue;
                toReturn.Add(profRespModel);
            }

            return toReturn;
        }

        public static IList<MemberOrganizationContract> ToDataContract(this IList<MemberOrganizationModel> memberOrganizations)
        {

            var toReturn = new List<MemberOrganizationContract>() { };

            foreach (var organization in memberOrganizations)
            {
                var memberOrganizationContract = new MemberOrganizationContract() { };
                //MemberOrganization Data
                memberOrganizationContract.MemberID = organization.MemberId;
                memberOrganizationContract.RefOrganizationID = organization.OrganizationId;
                memberOrganizationContract.OECode = organization.OECode;
                memberOrganizationContract.BranchCode = organization.BranchCode;
                memberOrganizationContract.ExpectedGraduationYear = organization.ExpectedGraduationYear.HasValue && organization.ExpectedGraduationYear > 1899 ? organization.ExpectedGraduationYear : 1900;
                memberOrganizationContract.SchoolReportingID = organization.ReportingId;
                memberOrganizationContract.EffectiveStartDate = organization.EffectiveStartDate;
                memberOrganizationContract.EffectiveEndDate = organization.EffectiveEndDate;
                memberOrganizationContract.IsOrganizationDeleted = organization.IsOrganizationDeleted;

                toReturn.Add(memberOrganizationContract);
            }

            return toReturn;
        }

        public static IList<MemberProfileQAContract> ToDataContract(this IList<MemberProfileQAModel> profileResponses)
        {

            var toReturn = new List<MemberProfileQAContract>() { };

            foreach (var response in profileResponses)
            {
                var profRespContract = new MemberProfileQAContract() { };
                profRespContract.ProfileQuestionExternalID = response.QuestionExternalId;
                profRespContract.ProfileAnswerExternalID = response.AnsExternalId;
                profRespContract.CustomValue = !string.IsNullOrWhiteSpace(response.CustomValue) ? response.CustomValue : null;
                toReturn.Add(profRespContract);
            }

            return toReturn;
        }

        /// <summary>
        /// To the member data contract.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static MemberContract ToDataContract(this ASAMemberModel member)
        {
            return new MemberContract()
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                EmailAddress = member.Emails.First().EmailAddress,
                MemberId = new AsaMemberAdapter().GetMemberIdFromContext(),
                ActiveDirectoryKey = new Guid(member.ActiveDirectoryKey),
                IsContactAllowed = !member.ContactFrequency,
                EnrollmentStatusCode = member.EnrollmentStatus,
                GradeLevelCode = member.GradeLevel,
                MemberOrganizations = member.Organizations.Any() ? member.Organizations.ToDataContract().ToArray() : null,
                DisplayName = member.DisplayName,
                CommunityDisplayName = member.CommunityDisplayName,
                IsCommunityActive = member.IsCommunityActive,
                WelcomeEmailSent = member.WelcomeEmailSent,
                USPostalCode = member.USPostalCode,
                YearOfBirth = member.YearOfBirth.HasValue && member.YearOfBirth > 0 ? member.YearOfBirth : null as short?
            };
        }

        /// <summary>
        /// To the user registration data contract.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static UserRegistrationContract ToUserRegistrationDataContract(this  ASAMemberModel member)
        {
            IList<MemberOrganizationContract> organizationList = new List<MemberOrganizationContract>();
            if (member.Organizations != null && member.Organizations.Count > 0)
            {
                foreach (var organization in member.Organizations)
                {
                    organizationList.Add(new MemberOrganizationContract()
                    {
                        OrganizationId = organization.OrganizationId,
                        OECode = organization.OECode,
                        BranchCode = organization.BranchCode,
                        ExpectedGraduationYear = organization.ExpectedGraduationYear.HasValue && organization.ExpectedGraduationYear > 1899 ? organization.ExpectedGraduationYear : 1900,
                        SchoolReportingID = organization.ReportingId
                    });
                }
            }
            else
            {
                organizationList.Add(new MemberOrganizationContract() { OECode = "000000", BranchCode = "00", ExpectedGraduationYear = 1899 });
            }

            return new UserRegistrationContract()
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    ActiveDirectoryKey = member.MembershipId,
                    InvitationToken = member.InvitationToken,
                    EmailAddress = member.Emails[0].EmailAddress,
                    IsContactAllowed = !member.ContactFrequency,
                    EnrollmentStatus = member.EnrollmentStatus,
                    GradeLevel = member.GradeLevel,
                    YearOfBirth = member.YearOfBirth,
                    Password = member.Password,
                    RegistrationSourceId = Convert.ToInt32(member.Source),
                    MemberOrganizations = organizationList.ToArray()
                };
        }

        /// <summary>
        /// Map SALTService VLCMemberProfile Contract to SAL Model
        /// </summary>
        /// <param name="vlcMemberProfileContract"></param>
        /// <returns></returns>
        public static VLCMemberProfileModel ToDomainObject(this VLCUserProfileContract vlcProfileContract)
        {
            if (vlcProfileContract == null)
                return null;

            return new VLCMemberProfileModel()
            {
                MemberID = vlcProfileContract.MemberID,
                EnrollmentStatus = vlcProfileContract.EnrollmentStatus,
                AdjustedGrossIncome = vlcProfileContract.AdjustedGrossIncome,
                GraduationDate = vlcProfileContract.GraduationDate,
                RepaymentStatus = vlcProfileContract.RepaymentStatus,
                FamilySize = vlcProfileContract.FamilySize,
                StateOfResidence = vlcProfileContract.StateOfResidence,
                TaxFilingStatus = vlcProfileContract.TaxFilingStatus
            };

        }

        public static VLCUserProfileContract ToDataContract(this VLCMemberProfileModel profile)
        {
            return new VLCUserProfileContract()
            {
                MemberID = profile.MemberID,
                EnrollmentStatus = profile.EnrollmentStatus,
                GraduationDate = profile.GraduationDate,
                AdjustedGrossIncome = profile.AdjustedGrossIncome,
                FamilySize = Convert.ToByte(profile.FamilySize),
                RepaymentStatus = profile.RepaymentStatus,
                StateOfResidence = profile.StateOfResidence,
                TaxFilingStatus = profile.TaxFilingStatus
            };
        }

        public static MemberOrganizationModel ToMemberOrganizationModel(this MemberOrganizationContract memberOrg)
        {
            return new MemberOrganizationModel
            {
                MemberId = memberOrg.MemberID,
                OrganizationId = memberOrg.OrganizationId,
                OrganizationName = memberOrg.OrganizationName,
                OrganizationAliases = memberOrg.OrganizationAliases,
                OrganizationLogoName = memberOrg.OrganizationLogoName,
                OECode = memberOrg.OECode,
                BranchCode = memberOrg.BranchCode,
                EffectiveEndDate = memberOrg.EffectiveEndDate,
                EffectiveStartDate = memberOrg.EffectiveStartDate,
                IsContracted = memberOrg.IsContracted,
                RefSALTSchoolTypeID = memberOrg.RefSALTSchoolTypeID
            };
        }

        public static MemberCourseModel ToMemberCourseModel(this SALTCoursesWSClient.CourseModel saltCourseModel)
        {
            return new MemberCourseModel
            {
                CourseID = saltCourseModel.id,
                ClassID = saltCourseModel.classid,
                CompleteStatus = saltCourseModel.completestatusid == 2 ? true: false,
                Credits = saltCourseModel.credits,
                Grade = saltCourseModel.grade,
                FullName = saltCourseModel.fullname,
                ShortName = saltCourseModel.shortname,
                IdNumber = saltCourseModel.idnumber,
                UserID = saltCourseModel.userid,
                ContentID = saltCourseModel.contentid
            };
        }

        /// <summary>
        /// Map SALT Service MemberProductContract to SAL MemberProductModel
        /// </summary>
        /// <param name="memProduct"></param>
        /// <returns></returns>
        public static MemberProductModel ToMemberProductModel(this MemberProductContract memProduct)
        {
            return new MemberProductModel
            {
                MemberID = memProduct.MemberID,
                RefProductID = memProduct.RefProductID,
                IsMemberProductActive = memProduct.IsMemberProductActive,
                MemberProductValue = memProduct.MemberProductValue
            };
        }

        public static MemberProductContract ToMemberProductContract(this MemberProductModel memProductModel)
        {
            return new MemberProductContract
            {
                MemberID = memProductModel.MemberID,
                RefProductID = memProductModel.RefProductID,
                IsMemberProductActive = memProductModel.IsMemberProductActive,
                MemberProductValue = memProductModel.MemberProductValue
            };
        }

        /// <summary> 
        /// To the domain object for member products.
        /// </summary> 
        /// <param name="memProdContracts">member product contracts.</param> 
        /// <returns></returns> 
        public static List<MemberProductModel> ToDomainObject(this MemberProductContract[] memProdContracts)
        {
            var toReturn = new List<MemberProductModel>() { };

            if (memProdContracts != null)
            {
                foreach (var item in memProdContracts)
                {
                    toReturn.Add(new MemberProductModel()
                    {
                        RefProductID = item.RefProductID,
                        MemberID = item.MemberID,
                        MemberProductValue = item.MemberProductValue,
                        IsMemberProductActive = item.IsMemberProductActive
                    });
                }
            }

            return toReturn;
        }

        /// <summary> 
        /// To the domain object for member profile questions and answers.
        /// </summary> 
        /// <param name="memberProfileQAContracts">member profile question and answer contracts.</param> 
        /// <returns>List<MemberProfileQAModel></returns> 
        public static List<MemberProfileQAModel> ToDomainObject(this MemberProfileQAContract[] memberProfileQAContracts)
        {
            var toReturn = new List<MemberProfileQAModel>() { };

            if (memberProfileQAContracts != null)
            {
                foreach (var response in memberProfileQAContracts)
                {
                    var model = new MemberProfileQAModel() { };
                    model.QuestionName = response.ProfileQuestionName;
                    model.QuestionExternalId = response.ProfileQuestionExternalID;
                    model.AnsName = response.ProfileAnswerName;
                    model.AnsDescription = response.ProfileAnswerDescription;
                    model.AnsExternalId = response.ProfileAnswerExternalID;
                    model.CustomValue = response.CustomValue;
                    toReturn.Add(model);
                }
            }
            return toReturn;
        }


        /// <summary> 
        /// To the domain object for MemberQAModel questions and answers.
        /// </summary> 
        /// <param name="MemberQAContract">member profile question and answer contracts.</param> 
        /// <returns>List<MemberQAModel></returns> 
        public static IList<MemberQAModel> ToDomainObject(this IList<MemberQAContract> Responses)
        {
            List<MemberQAModel> toReturn = new List<MemberQAModel>() { };

            foreach (MemberQAContract response in Responses)
            {
                var memberQAModel = new MemberQAModel();
                memberQAModel = response.ToDomainObject();
                toReturn.Add(memberQAModel);
            }

            return toReturn;
        }

        /// <summary> 
        /// To the ToDataContract object for MemberQAContract questions and answers.
        /// </summary> 
        /// <param name="MemberQAModel">member profile question and answer contracts.</param> 
        /// <returns>List<MemberQAContract></returns> 
        public static IList<MemberQAContract> ToDataContract(this IList<MemberQAModel> Responses)
        {
            List<MemberQAContract> toReturn = new List<MemberQAContract>() { };

            foreach (var response in Responses)
            {
                var memberQAContract = new MemberQAContract();
                memberQAContract = response.ToDataContract();
                toReturn.Add(memberQAContract);
            }

            return toReturn;
        }

        /// <summary> 
        /// To the ToDataContract object for MemberQAContract.
        /// </summary> 
        /// <param name="MemberQAModel">member product contracts.</param> 
        /// <returns>MemberQAContract</returns> 
        public static MemberQAContract ToDataContract(this MemberQAModel Responses)
        {
            MemberQAContract toReturn = new MemberQAContract()
            {
                AnswerText = Responses.choiceText,
                ExternalSourceAnswerID = int.Parse(Responses.choiceId),
                ExternalSourceQuestionID = int.Parse(Responses.questionId),
                RefSourceID = int.Parse(Responses.sourceId),
                MemberQuestionAnswerID = Responses.MemberQuestionAnswerID,
                FreeformAnswerText = Responses.FreeformAnswerText
            };
            return toReturn;
        }
        /// <summary> 
        /// To the ToDataContract object for MemberQAModel.
        /// </summary> 
        /// <param name="MemberQAContract">member product contracts.</param> 
        /// <returns>MemberQAModel</returns> 
        public static MemberQAModel ToDomainObject(this MemberQAContract Responses)
        {
            MemberQAModel toReturn = new MemberQAModel()
            {
                choiceText = Responses.AnswerText,
                choiceId = Responses.ExternalSourceAnswerID.ToString(),
                questionId = Responses.ExternalSourceQuestionID.ToString(),
                sourceId = Responses.RefSourceID.ToString(),
                MemberQuestionAnswerID = Responses.MemberQuestionAnswerID,
                FreeformAnswerText = Responses.FreeformAnswerText
            };
            return toReturn;
        }
        /// <summary> 
        /// To the view domain object for member questions and answers.
        /// </summary> 
        /// <param name="vmemberQuestionAnswerContract">member question and answer contracts.</param> 
        /// <returns>vMemberQuestionAnswerModel</returns> 
        public static vMemberQuestionAnswerModel ToDomainObject(this vMemberQuestionAnswerContract memberQAObject)
        {
            return new vMemberQuestionAnswerModel()
            {
                MemberID = memberQAObject.MemberID,
                FirstName = memberQAObject.FirstName,
                LastName = memberQAObject.LastName,
                choiceId = memberQAObject.ExternalSourceQuestionAnswerID,
                questionId = memberQAObject.ExternalSourceQuestionID,
                choiceText = memberQAObject.AnswerText,
                StandardAnswerText = memberQAObject.StandardAnswerText,
                QuestionText = memberQAObject.QuestionText,
                StandardQuestionText = memberQAObject.StandardQuestionText,
                MemberQuestionAnswerID = memberQAObject.MemberQuestionAnswerID,
                RefAnswerID = memberQAObject.RefAnswerID,
                RefQuestionID = memberQAObject.RefQuestionID,
                RefSourceID = memberQAObject.RefSourceID,
                SourceName = memberQAObject.SourceName,
                SourceDescription = memberQAObject.SourceDescription,
                RefSourceQuestionID = memberQAObject.RefSourceQuestionID,
                SourceQuestionDescription = memberQAObject.SourceQuestionDescription,
                AnswerDisplayOrder = memberQAObject.AnswerDisplayOrder,
                QuestionDisplayOrder = memberQAObject.QuestionDisplayOrder,
                RefQuestions_refQuestionID = memberQAObject.RefQuestions_refQuestionID,
                RefAnswer_refAnswerID = memberQAObject.RefAnswer_refAnswerID,
                RefSourceQuestionAnswer_refAnswerID = memberQAObject.RefSourceQuestionAnswer_refAnswerID,
                RefSourceQuestionAnswerID = memberQAObject.RefSourceQuestionAnswerID,
                CreatedDate = memberQAObject.CreatedDate,
                FreeformAnswerText = memberQAObject.FreeformAnswerText
            };
        }

        /// <summary> 
        /// To the view domain object for member questions and answers.
        /// </summary> 
        /// <param name="vMemberQuestionAnswerModel">member question and answer contracts.</param> 
        /// <returns>vMemberQuestionAnswerContract</returns> 
        public static vMemberQuestionAnswerContract ToDataContract(this vMemberQuestionAnswerModel memberQAObject)
        {
            var model = new vMemberQuestionAnswerContract()
            {
                MemberID = memberQAObject.MemberID,
                FirstName = memberQAObject.FirstName,
                LastName = memberQAObject.LastName,
                ExternalSourceQuestionAnswerID = memberQAObject.choiceId,
                ExternalSourceQuestionID = memberQAObject.questionId,
                AnswerText = memberQAObject.choiceText,
                StandardAnswerText = memberQAObject.StandardAnswerText,
                QuestionText = memberQAObject.QuestionText,
                StandardQuestionText = memberQAObject.StandardQuestionText,
                MemberQuestionAnswerID = memberQAObject.MemberQuestionAnswerID,
                RefAnswerID = memberQAObject.RefAnswerID,
                RefQuestionID = memberQAObject.RefQuestionID,
                RefSourceID = memberQAObject.RefSourceID,
                SourceName = memberQAObject.SourceName,
                SourceDescription = memberQAObject.SourceDescription,
                RefSourceQuestionID = memberQAObject.RefSourceQuestionID,
                SourceQuestionDescription = memberQAObject.SourceQuestionDescription,
                AnswerDisplayOrder = memberQAObject.AnswerDisplayOrder,
                QuestionDisplayOrder = memberQAObject.QuestionDisplayOrder,
                RefQuestions_refQuestionID = memberQAObject.RefQuestions_refQuestionID,
                RefAnswer_refAnswerID = memberQAObject.RefAnswer_refAnswerID,
                RefSourceQuestionAnswer_refAnswerID = memberQAObject.RefSourceQuestionAnswer_refAnswerID,
                RefSourceQuestionAnswerID = memberQAObject.RefSourceQuestionAnswerID,
                CreatedDate = memberQAObject.CreatedDate,
                FreeformAnswerText = memberQAObject.FreeformAnswerText
            };
            return model;
        }

        public static MemberToDoModel ToDomainObject(this MemberToDoListContract todoContract)
        {
            return new MemberToDoModel()
            {
                MemberID = todoContract.MemberID,
                MemberToDoListID = todoContract.MemberToDoListID,
                ContentID = todoContract.ContentID,
                RefToDoStatusID = todoContract.RefToDoStatusID,
                RefToDoTypeID = todoContract.RefToDoTypeID,
                CreatedBy = todoContract.CreatedBy,
                CreatedDate = todoContract.CreatedDate,
                ModifiedDate = todoContract.ModifiedDate,
                ModifiedBy = todoContract.ModifiedBy
            };
        }

        public static MemberToDoListContract ToDataContract(this MemberToDoModel todo)
        {
            return new MemberToDoListContract()
            {
                MemberToDoListID = todo.MemberToDoListID,
                MemberID = todo.MemberID,
                ContentID = todo.ContentID,
                RefToDoStatusID = todo.RefToDoStatusID,
                RefToDoTypeID = todo.RefToDoTypeID,
                CreatedBy = todo.CreatedBy,
                CreatedDate = todo.CreatedDate,
                ModifiedDate = todo.ModifiedDate,
                ModifiedBy = todo.ModifiedBy
            };
        }

        /// <summary>
        /// Map SALTService RefProfileQuestionContract list to SAL ProfileQuestionModel list
        /// </summary>
        /// <param name="profQueContracts"></param>
        /// <returns></returns>
        public static IList<ProfileQuestionModel> ToDomainObject(this IList<RefProfileQuestionContract> profQueContracts)
        {
            var toReturn = new List<ProfileQuestionModel>() { };

            if (profQueContracts != null)
            {
                foreach (var ques in profQueContracts)
                {
                    var profQuesModel = new ProfileQuestionModel() { };
                    profQuesModel.QuestionID = ques.ProfileQuestionExternalID;
                    profQuesModel.QuestionName = ques.ProfileQuestionName;
                    profQuesModel.QuestionDescription = ques.ProfileQuestionDescription;
                    profQuesModel.ProfileQuestionTypeName = ques.RefProfileQuestionType.ProfileQuestionTypeName;
                    profQuesModel.ProfileQuestionPriority = ques.ProfileQuestionPriority;
                    profQuesModel.IsProfileQuestionActive = ques.IsProfileQuestionActive;
                    profQuesModel.IsInLineProfileQuestion = ques.IsInLineProfileQuestion;

                    toReturn.Add(profQuesModel);
                }
            }

            return toReturn;
        }

        public static IList<ProfileAnswerModel> ToDomainObject(this IList<RefProfileAnswerContract> profAnsContracts)
        {
            var toReturn = new List<ProfileAnswerModel>() { };

            if (profAnsContracts != null)
            {
                foreach (var ans in profAnsContracts)
                {
                    var profAnsModel = new ProfileAnswerModel() { };
                    profAnsModel.AnsID = ans.ProfileAnswerExternalID;
                    profAnsModel.AnsName = ans.ProfileAnswerName;
                    profAnsModel.AnsDescription = ans.ProfileAnswerDescription;
                    profAnsModel.QuestionID = ans.RefProfileQuestionID;
                    profAnsModel.IsProfileAnswerActive = ans.IsProfileAnswerActive;
                    profAnsModel.ProfileAnswerDisplayOrder = ans.ProfileAnswerDisplayOrder;

                    toReturn.Add(profAnsModel);
                }
            }

            return toReturn;
        }

        public static List<ProfileResponseModel> ToDomainObject(this List<MemberProfileQAContract> profileResponses)
        {
            var toReturn = new List<ProfileResponseModel>() { };

            if (profileResponses != null)
            {
                foreach (var response in profileResponses)
                {
                    var proResponseModel = new ProfileResponseModel() { };
                    proResponseModel.QuestionExternalId = response.ProfileQuestionExternalID;
                    proResponseModel.AnsExternalId = response.ProfileAnswerExternalID;
                    proResponseModel.AnsName = response.ProfileAnswerName;
                    proResponseModel.AnsDescription = response.ProfileAnswerDescription;
                    proResponseModel.CustomValue = response.CustomValue;

                    toReturn.Add(proResponseModel);
                }
            }

            return toReturn;
        }

    }
}
