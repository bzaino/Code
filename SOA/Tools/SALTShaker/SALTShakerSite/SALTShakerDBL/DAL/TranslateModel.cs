using System;
using System.Collections.Generic;
using log4net;
using SALTShaker.DAL.DataContracts;
using SALTShaker.Proxies.SALTService;

namespace SALTShaker.DAL
{
    public static class MappingExtensions
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MappingExtensions));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="membersVC"></param>
        /// <returns></returns>

        public static MemberModel ToDomainObject(this MemberContract memberContract)
        {
            if (memberContract == null)
                return null;

            return new MemberModel()
            {
                MemberId = memberContract.MemberId,
                FirstName = memberContract.FirstName,
                LastName = memberContract.LastName,
                EmailAddress = memberContract.EmailAddress,
                IsContactAllowed = memberContract.IsContactAllowed,
                RegistrationSourceId = memberContract.RegistrationSourceId,
                ActiveDirectoryKey = memberContract.ActiveDirectoryKey,
                MemberStartDate = memberContract.MemberStartDate,
                IsMemberActive = memberContract.IsMemberActive,
                LastLoginDate = memberContract.LastLoginDate,
                CreatedBy = memberContract.CreatedBy,
                CreatedDate = memberContract.CreatedDate,
                DisplayName = memberContract.DisplayName,
                InvitationToken = memberContract.InvitationToken

            };
        }

        public static MemberContract ToDataContract(this MemberModel member)
        {
            return new MemberContract()
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                EmailAddress = member.EmailAddress,
                MemberId = member.MemberId,
                ActiveDirectoryKey = member.ActiveDirectoryKey,
                IsContactAllowed = member.IsContactAllowed,
                EnrollmentStatusCode = member.EnrollmentStatusId.ToString(),
                GradeLevelId = member.GradeLevelId,
                DisplayName = member.DisplayName
            };
        }

        /// <summary>
        /// Translates the MemberActivityHistoryContract list to the MemberActivityHistoryModel list.
        /// </summary>
        /// <param name="activities">The List<MemberActivityHistoryContract.</param>
        /// <returns></returns>
        public static IEnumerable<MemberActivityHistoryModel> ToMemberActivityHistoryModelList(this List<MemberActivityHistoryContract> activities)
        {
            var list = new List<MemberActivityHistoryModel>();
            foreach (var activity in activities)
            {
                list.Add(activity.ToMemberActivityHistoryModel());
            }

            return list;
        }

        /// <summary>
        /// Translates the MemberActivityHistoryContract to the MemberActivityHistoryModel.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static MemberActivityHistoryModel ToMemberActivityHistoryModel(this MemberActivityHistoryContract activity)
        {
            return new MemberActivityHistoryModel
            {
                ActivityDate = activity.ActivityDate,
                MemberActivityHistoryID = activity.MemberActivityHistoryID,
                MemberID = activity.MemberID,
                RefActivityTypeID = activity.RefActivityTypeID,
                AdditionalDetails = activity.AdditionalDetails,
                CreatedBy = activity.CreatedBy,
                CreatedDate = activity.CreatedDate,
                ModifiedBy = activity.ModifiedBy,
                ModifiedDate = activity.ModifiedDate,
                //RefActivityType = activity.RefActivityType.ToActivityTypeModel(),
                RefActivityTypeName = activity.RefActivityType.ActivityTypeName
            };

        }

        /// <summary>
        /// Translates ActivityTypeContract list to ActivityTypeModel list.
        /// </summary>
        /// <param name="activityTypes"></param>
        /// <returns></returns>
        public static IEnumerable<ActivityTypeModel> ToActivityTypeModelList(this List<ActivityTypeContract> activityTypes)
        {
            var list = new List<ActivityTypeModel>();
            foreach (var actType in activityTypes)
            {
                list.Add(actType.ToActivityTypeModel());
            }

            return list;
        }

        /// <summary>
        /// Translates ActivityTypeContract to ActivityTypeModel.
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public static ActivityTypeModel ToActivityTypeModel(this ActivityTypeContract activityType)
        {
            return new ActivityTypeModel
            {
                ActivityTypeId = activityType.ActivityTypeId,
                ActivityTypeName = activityType.ActivityTypeName,
                ActivityTypeDescription = activityType.ActivityTypeDescription,
                CreatedBy = activityType.CreatedBy,
                CreatedDate = activityType.CreatedDate,
                ModifiedBy = activityType.ModifiedBy,
                ModifiedDate = activityType.ModifiedDate

            };
        }

        /// <summary>
        /// Translates MemberActivityHistoryModel list to MemberActivityHistoryContract list.
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        public static IList<MemberActivityHistoryContract> ToMemberActivityHistoryContractList(this List<MemberActivityHistoryModel> activities)
        {
            var list = new List<MemberActivityHistoryContract>();

            foreach (var activity in activities)
            {
                list.Add(activity.ToMemberActivityHistoryContract());
            }

            return list;
        }

        /// <summary>
        /// Translates MemberActivityHistoryModel to MemberActivityHistoryContract.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static MemberActivityHistoryContract ToMemberActivityHistoryContract(this MemberActivityHistoryModel activity)
        {
            return new MemberActivityHistoryContract
            {
                ActivityDate = activity.ActivityDate,
                MemberActivityHistoryID = activity.MemberActivityHistoryID,
                MemberID = activity.MemberID,
                RefActivityTypeID = activity.RefActivityTypeID,
                AdditionalDetails = activity.AdditionalDetails,
                CreatedBy = activity.CreatedBy,
                CreatedDate = activity.CreatedDate,
                ModifiedBy = activity.ModifiedBy,
                ModifiedDate = activity.ModifiedDate

            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="membersVC"></param>
        /// <returns></returns>
        public static IEnumerable<SaltMemberModel> ToDomainObject(this List<vMemberAcademicInfoContract> membersVC)
        {
            var list = new List<SaltMemberModel>();
            foreach (var memViewCon in membersVC)
            {
                list.Add(memViewCon.ToSaltMemberModel());
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberViewContract"></param>
        /// <returns></returns>
        public static SaltMemberModel ToSaltMemberModel(this vMemberAcademicInfoContract memberViewContract)
        {
            if (memberViewContract == null)
            {
                return null;
            }

            return new SaltMemberModel()
            {
                MemberID = memberViewContract.MemberID,
                FirstName = memberViewContract.FirstName,
                LastName = memberViewContract.LastName,
                EmailAddress = memberViewContract.EmailAddress,
                IsContactAllowed = memberViewContract.IsContactAllowed,
                IsContracted = memberViewContract.IsContracted,
                RefRegistrationSourceID = memberViewContract.RefRegistrationSourceID,
                ActiveDirectoryKey = memberViewContract.ActiveDirectoryKey,
                IsMemberActive = memberViewContract.IsMemberActive,
                MemberFirstActivationDate = memberViewContract.MemberFirstActivationDate,
                MemberActivationHistoryID = memberViewContract.MemberActivationHistoryID,
                ActivationDate = memberViewContract.ActivationDate,
                DeactivationDate = memberViewContract.DeactivationDate,
                OrganizationName = memberViewContract.OrganizationName,
                OrganizationDescription = memberViewContract.OrganizationDescription,
                OrganizationExternalID = memberViewContract.OrganizationExternalID,
                OrganizationLogoName = memberViewContract.OrganizationLogoName,
                OrganizationEffectiveStartDate = memberViewContract.EffectiveStartDate,
                OrganizationEffectiveEndDate = memberViewContract.EffectiveEndDate,
                RefOrganizationID = memberViewContract.RefOrganizationID,
                InvitationToken = memberViewContract.InvitationToken,
                DisplayName = memberViewContract.DisplayName,
                BranchCode = memberViewContract.BranchCode,
                OPECode = memberViewContract.OPECode,
                EnrollmentStatusCode = memberViewContract.EnrollmentStatusCode,
                EnrollmentStatusDescription = memberViewContract.EnrollmentStatusDescription,
                RefEnrollmentStatusID = memberViewContract.RefEnrollmentStatusID,
                RefGradeLevelID = memberViewContract.RefGradeLevelID,
                GradeLevelCode = memberViewContract.GradeLevelCode,
                GradeLevelDescription = memberViewContract.GradeLevelDescription,
                WelcomeEmailSent = memberViewContract.WelcomeEmailSent
            };
        }



        public static SaltMemberModel ToSaltMemberModel(this MemberContract memberContract)
        {
            if (memberContract == null)
                return null;

            return new SaltMemberModel()
            {
                MemberID = memberContract.MemberId,
                FirstName = memberContract.FirstName,
                LastName = memberContract.LastName,
                EmailAddress = memberContract.EmailAddress,
                IsContactAllowed = memberContract.IsContactAllowed,
                RefRegistrationSourceID = memberContract.RegistrationSourceId,
                ActiveDirectoryKey = memberContract.ActiveDirectoryKey,
                MemberFirstActivationDate = memberContract.MemberStartDate,
                IsMemberActive = memberContract.IsMemberActive,
                LastLoginDate = memberContract.LastLoginDate,
                DisplayName = memberContract.DisplayName,
                InvitationToken = memberContract.InvitationToken

            };
        }

        public static IEnumerable<MemberOrganizationModel> ToDomainObject(this List<MemberOrganizationContract> memberOrgsCon)
        {
            var translatedList = new List<MemberOrganizationModel>();
            foreach (var memOrgC in memberOrgsCon)
            {
                translatedList.Add(memOrgC.ToMemberOrganizationModel());
            }

            return translatedList;
        }

        public static MemberOrganizationModel ToMemberOrganizationModel(this MemberOrganizationContract memberOrgCon)
        {
            return new MemberOrganizationModel()
            {
                MemberID = memberOrgCon.MemberID,
                OrganizationID = memberOrgCon.RefOrganizationID,
                OrganizationExternalID = memberOrgCon.OrganizationExternalID,
                ExpectedGraduationYear = memberOrgCon.ExpectedGraduationYear,
                ReportingID = memberOrgCon.SchoolReportingID,
                EffectiveStartDate = memberOrgCon.EffectiveStartDate,
                EffectiveEndDate = memberOrgCon.EffectiveEndDate,
                OrganizationName = memberOrgCon.OrganizationName,
                IsContracted = memberOrgCon.IsContracted,
                OrganizationAliases = memberOrgCon.OrganizationAliases,
                OrganizationLogoName = memberOrgCon.OrganizationLogoName,
                OECode = memberOrgCon.OECode,
                BranchCode = memberOrgCon.BranchCode,
                RefSALTSchoolTypeID = memberOrgCon.RefSALTSchoolTypeID
            };
        }

        public static IEnumerable<vMemberAcademicInfoModel> ToVMemberAcademicInfoList(this List<SaltMemberModel> spModelList)
        {
            var list = new List<vMemberAcademicInfoModel>();
            foreach (var model in spModelList)
            {
                list.Add(model.ToVMemberAcademicInfo());
            }

            return list;
        }

        public static vMemberAcademicInfoModel ToVMemberAcademicInfo(this SaltMemberModel member)
        {
            return new vMemberAcademicInfoModel()
            {
                ActivationDate = member.ActivationDate,
                ActiveDirectoryKey = member.ActiveDirectoryKey,
                BranchCode = member.BranchCode,
                DeactivationDate = member.DeactivationDate,
                DisplayName = member.DisplayName,
                EmailAddress = member.EmailAddress,
                EnrollmentStatusCode = member.EnrollmentStatusCode,
                EnrollmentStatusDescription = member.EnrollmentStatusDescription,
                FirstName = member.FirstName,
                GradeLevelCode = member.GradeLevelCode,
                GradeLevelDescription = member.GradeLevelDescription,
                InvitationToken = member.InvitationToken,
                IsContactAllowed = member.IsContactAllowed,
                IsContracted = member.IsContracted,
                IsMemberActive = member.IsMemberActive,
                LastName = member.LastName,
                MemberActivationHistoryID = member.MemberActivationHistoryID,
                MemberFirstActivationDate = member.MemberFirstActivationDate,
                MemberID = member.MemberID.HasValue ? (int)member.MemberID : (int)0,
                OPECode = member.OPECode,
                RefEnrollmentStatusID = member.RefEnrollmentStatusID.HasValue ? (int)member.RefEnrollmentStatusID : (int)0,
                RefGradeLevelID = member.RefGradeLevelID.HasValue ? (int)member.RefGradeLevelID : (int)0,
                RefRegistrationSourceID = member.RefRegistrationSourceID,
                RefOrganizationID = member.RefOrganizationID.HasValue ? (int)member.RefOrganizationID : (int)0,
                OrganizationDescription = member.OrganizationDescription == null ? string.Empty : member.OrganizationDescription,
                OrganizationExternalID = member.OrganizationExternalID,
                OrganizationLogoName = member.OrganizationLogoName == null ? string.Empty : member.OrganizationLogoName,
                OrganizationName = member.OrganizationName == null ? string.Empty : member.OrganizationName,
                WelcomeEmailSent = member.WelcomeEmailSent
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="membersVC"></param>
        /// <returns></returns>
        public static IEnumerable<vMemberAcademicInfoModel> ToVMemberAcademicInfoDomainObject(this List<vMemberAcademicInfoContract> membersVC)
        {
            var list = new List<vMemberAcademicInfoModel>();
            foreach (var memViewCon in membersVC)
            {
                list.Add(memViewCon.ToVMemberAcademicInfo());
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberViewContract"></param>
        /// <returns></returns>
        public static vMemberAcademicInfoModel ToVMemberAcademicInfo(this vMemberAcademicInfoContract memberViewContract)
        {
            if (memberViewContract == null)
            {
                return null;
            }

            return new vMemberAcademicInfoModel()
            {
                ActivationDate = memberViewContract.ActivationDate,
                ActiveDirectoryKey = memberViewContract.ActiveDirectoryKey,
                BranchCode = memberViewContract.BranchCode,
                DeactivationDate = memberViewContract.DeactivationDate,
                DisplayName = memberViewContract.DisplayName,
                EmailAddress = memberViewContract.EmailAddress,
                EnrollmentStatusCode = memberViewContract.EnrollmentStatusCode,
                EnrollmentStatusDescription = memberViewContract.EnrollmentStatusDescription,
                FirstName = memberViewContract.FirstName,
                GradeLevelCode = memberViewContract.GradeLevelCode,
                GradeLevelDescription = memberViewContract.GradeLevelDescription,
                InvitationToken = memberViewContract.InvitationToken,
                IsContactAllowed = memberViewContract.IsContactAllowed,
                IsContracted = memberViewContract.IsContracted,
                IsMemberActive = memberViewContract.IsMemberActive,
                LastName = memberViewContract.LastName,
                MemberActivationHistoryID = memberViewContract.MemberActivationHistoryID,
                MemberFirstActivationDate = memberViewContract.MemberFirstActivationDate,
                MemberID = memberViewContract.MemberID,
                MemberOrganizationID = memberViewContract.MemberOrganizationID.HasValue ? (int)memberViewContract.MemberOrganizationID : (int)0,
                OPECode = memberViewContract.OPECode,
                RefEnrollmentStatusID = memberViewContract.RefEnrollmentStatusID.HasValue ? (int)memberViewContract.RefEnrollmentStatusID : (int)0,
                RefGradeLevelID = memberViewContract.RefGradeLevelID.HasValue ? (int)memberViewContract.RefGradeLevelID : (int)0,
                RefRegistrationSourceID = memberViewContract.RefRegistrationSourceID,
                RefOrganizationID = memberViewContract.RefOrganizationID.HasValue ? (int)memberViewContract.RefOrganizationID : (int)0,
                OrganizationDescription = memberViewContract.OrganizationDescription,
                OrganizationExternalID = memberViewContract.OrganizationExternalID,
                OrganizationLogoName = memberViewContract.OrganizationLogoName,
                OrganizationName = memberViewContract.OrganizationName,
                WelcomeEmailSent = memberViewContract.WelcomeEmailSent
            };
        }

        public static vMemberAcademicInfoContract ToDataContract(this SaltMemberModel members)
        {
            return new vMemberAcademicInfoContract()
            {

            };
        }

        public static IEnumerable<OrganizationModel> ToDomainObject(this List<RefOrganizationContract> organizations)
        {
            var list = new List<OrganizationModel>();
            foreach (var org in organizations)
            {
                list.Add(org.ToOrganizationDomainObject());
            }

            return list;
        }

        public static OrganizationModel ToOrganizationDomainObject(this RefOrganizationContract organizationContract)
        {
            if (organizationContract == null)
                return null;
            return new OrganizationModel
            {
                RefOrganizationID = organizationContract.RefOrganizationID,
                OPECode = organizationContract.OPECode,
                BranchCode = organizationContract.BranchCode,
                OrganizationName = organizationContract.OrganizationName,
                OrganizationLogoName = organizationContract.OrganizationLogoName,
                OrganizationAliases = organizationContract.OrganizationAliases,
                OrganizationDescription = organizationContract.OrganizationDescription,
                OrganizationExternalID = organizationContract.OrganizationExternalID,
                IsContracted = organizationContract.IsContracted,
                IsLookupAllowed = organizationContract.IsLookupAllowed,
                RefStateID = organizationContract.RefStateID,
                RefSALTSchoolTypeID = organizationContract.RefSALTSchoolTypeID,
                CreatedBy = organizationContract.CreatedBy,
                CreatedDate = organizationContract.CreatedDate,
                ModifiedBy = organizationContract.ModifiedBy,
                ModifiedDate = organizationContract.ModifiedDate,
                OrganizationToDoItems = organizationContract.OrganizationToDoLists.ToDomainObject()                
            };
        }

        public static IEnumerable<OrganizationProductModel> ToDomainObject(this List<RefOrganizationProductContract> organizationProducts)
        {
            var list = new List<OrganizationProductModel>();
            foreach (var orgProdCont in organizationProducts)
            {
                list.Add(orgProdCont.toOrgProductDomainObject());
            }

            return list;
        }

        public static OrganizationProductModel toOrgProductDomainObject(this RefOrganizationProductContract orgProdC)
        {
            if (orgProdC == null)
                return null;

            return new OrganizationProductModel
            {
                RefOrganizationID = orgProdC.RefOrganizationID,
                RefProductID = orgProdC.RefProductID,
                IsRefOrganizationProductActive = orgProdC.IsRefOrganizationProductActive,
                CreatedBy = orgProdC.CreatedBy,
                CreatedDate = orgProdC.CreatedDate,                
                ModifiedBy = orgProdC.ModifiedBy,
                ModifiedDate = orgProdC.ModifiedDate
            };
        }

        public static IEnumerable<ProductModel> ToDomaiObject(this List<RefProductContract> products)
        {
            var list = new List<ProductModel>();
            foreach (var product in products)
            {
                list.Add(product.toProductDomainObject());
            }
            return list;
        }

        public static ProductModel toProductDomainObject(this RefProductContract productC)
        {
            if (productC == null)
                return null;

            return new ProductModel
            {
                RefProductID = productC.RefProductID,
                ProductName = productC.ProductName,
                ProductDescription = productC.ProductDescription,
                IsProductActive = productC.IsProductActive,
                CreatedBy = productC.CreatedBy,
                CreatedDate = productC.CreatedDate,
                ModifiedBy = productC.ModifiedBy,
                ModifiedDate = productC.ModifiedDate,
                RefProductTypeID = productC.RefProductTypeID
            };
        }

        public static IEnumerable<RefRegistrationSourceModel> ToDomainObject(this IEnumerable<RefRegistrationSourceContract> regSources)
        {
            var list = new List<RefRegistrationSourceModel>();
            foreach (var regSourceCont in regSources)
            {
                list.Add(regSourceCont.toRefRegistrationSourceDomainObject());
            }

            return list;
        }

        public static RefRegistrationSourceModel toRefRegistrationSourceDomainObject(this RefRegistrationSourceContract regSource)
        {
            if (regSource == null)
                return null;

            return new RefRegistrationSourceModel
            {
                RegistrationSourceId = regSource.RefRegistrationSourceId,
                RegistrationSourceName = regSource.RegistrationSourceName,
                RegistrationDetail = regSource.RegistrationDetail,
                CreatedBy = regSource.CreatedBy,
                CreatedDate = regSource.CreatedDate,
                ModifiedBy = regSource.ModifiedBy,
                ModifiedDate = regSource.ModifiedDate,
                RefRegistrationSourceTypeId = regSource.RefRegistrationSourceTypeId,
                RefCampaignId = regSource.RefCampaignId,
                CampaignName = regSource.RefCampaign.CampaignName,
                RefChannelId = regSource.RefChannelId,
                ChannelName = regSource.RefChannel.ChannelName
            };
        }

        public static IEnumerable<RefRegistrationSourceTypeModel> ToDomainObject(this IEnumerable<RefRegistrationSourceTypeContract> regSourceTypes)
        {
            var list = new List<RefRegistrationSourceTypeModel>();
            foreach (var regSourceTypeCont in regSourceTypes)
            {
                list.Add(regSourceTypeCont.toRefRegistrationSourceTypeDomainObject());
            }

            return list;
        }

        public static RefRegistrationSourceTypeModel toRefRegistrationSourceTypeDomainObject(this RefRegistrationSourceTypeContract regSourceType)
        {
            if (regSourceType == null)
                return null;

            return new RefRegistrationSourceTypeModel
            {
                RefRegistrationSourceTypeId = regSourceType.RefRegistrationSourceTypeID,
                RegistrationSourceTypeName = regSourceType.RegistrationSourceTypeName,
                RegistrationSourceTypeDescription = regSourceType.RegistrationSourceTypeDescription,
                CreatedBy = regSourceType.CreatedBy,
                CreatedDate = regSourceType.CreatedDate,
                ModifiedBy = regSourceType.ModifiedBy,
                ModifiedDate = regSourceType.ModifiedDate
            };
        }

        public static IEnumerable<RefCampaignModel> ToDomainObject(this IEnumerable<RefCampaignContract> campaigns)
        {
            var list = new List<RefCampaignModel>();
            foreach (var campaign in campaigns)
            {
                list.Add(campaign.toRefCampaignDomainObject());
            }

            return list;
        }

        public static RefCampaignModel toRefCampaignDomainObject(this RefCampaignContract campaign)
        {
            if (campaign == null)
                return null;

            return new RefCampaignModel
            {
                RefCampaignId = campaign.RefCampaignID,
                CampaignName = campaign.CampaignName,
                CreatedBy = campaign.CreatedBy,
                CreatedDate = campaign.CreatedDate,
                ModifiedBy = campaign.ModifiedBy,
                ModifiedDate = campaign.ModifiedDate
            };
        }

        public static IEnumerable<RefChannelModel> ToDomainObject(this IEnumerable<RefChannelContract> channels)
        {
            var list = new List<RefChannelModel>();
            foreach (var channel in channels)
            {
                list.Add(channel.toRefChannelDomainObject());
            }

            return list;
        }

        public static RefChannelModel toRefChannelDomainObject(this RefChannelContract channel)
        {
            if (channel == null)
                return null;

            return new RefChannelModel
            {
                RefChannelId = channel.RefChannelID,
                ChannelName = channel.ChannelName,
                CreatedBy = channel.CreatedBy,
                CreatedDate = channel.CreatedDate,
                ModifiedBy = channel.ModifiedBy,
                ModifiedDate = channel.ModifiedDate
            };
        }

        public static IEnumerable<vMemberRoleModel> ToDomainObject(this IEnumerable<MemberRoleContract> memberRoles)
        {
            var list = new List<vMemberRoleModel>();
            foreach (var role in memberRoles)
            {
                list.Add(role.toMemberRoleDomainObject());
            }

            return list;
        }

        public static vMemberRoleModel toMemberRoleDomainObject(this MemberRoleContract memberRoleC)
        {
            if (memberRoleC == null)
                return null;

            return new vMemberRoleModel
            {
                RefMemberRoleID = memberRoleC.RefMemberRoleID,
                IsMemberRoleActive = memberRoleC.IsMemberRoleActive,
                RoleName = memberRoleC.RefMemberRole.RoleName,
                RoleDescription = memberRoleC.RefMemberRole.RoleDescription,
                CreatedBy = memberRoleC.CreatedBy,
                CreatedDate = memberRoleC.CreatedDate,
                ModifiedBy = memberRoleC.ModifiedBy,
                ModifiedDate = memberRoleC.ModifiedDate
            };
        }

        public static IEnumerable<vMemberRoleModel> ToDomainObject(this IEnumerable<RefMemberRoleContract> refMemberRoles)
        {
            var list = new List<vMemberRoleModel>();
            foreach (var refMemberRole in refMemberRoles)
            {
                list.Add(refMemberRole.toRefMemberRoleDomainObject());
            }

            return list;
        }

        public static vMemberRoleModel toRefMemberRoleDomainObject(this RefMemberRoleContract refMemberRoleC)
        {
            if (refMemberRoleC == null)
                return null;

            return new vMemberRoleModel
            {
                RefMemberRoleID = refMemberRoleC.RefMemberRoleID,
                RoleName = refMemberRoleC.RoleName,
                RoleDescription = refMemberRoleC.RoleDescription
            };
        }

        public static IList<MemberRoleContract> ToMemberRolesDataContractList(this List<vMemberRoleModel> memberRoles)
        {
            var list = new List<MemberRoleContract>();

            foreach (var role in memberRoles)
            {
                list.Add(role.ToMemberRoleContract());
            }

            return list;
        }

        /// <summary>
        /// To the member reported loan contract.
        /// </summary>
        /// <param name="loan">The loan.</param>
        /// <returns></returns>
        public static MemberRoleContract ToMemberRoleContract(this vMemberRoleModel memberRole)
        {
            return new MemberRoleContract
            {
                RefMemberRoleID = memberRole.RefMemberRoleID,
                IsMemberRoleActive = memberRole.IsMemberRoleActive,
                ModifiedBy = memberRole.ModifiedBy
                
            };

        }
        /// <summary>
        /// Converts the OrganizationToDoListContract list object into a List of OrganizationToDoModel objects.
        /// </summary>
        /// <param name="organizationToDoList">OrganizationToDoList</param>
        /// <returns>List of OrganizationToDoModel objects</returns>
        public static List<OrganizationToDoModel> ToDomainObject(this IEnumerable<OrganizationToDoListContract> organizationToDoList)
        {
            var list = new List<OrganizationToDoModel>();
            foreach (var organizationToDo in organizationToDoList)
            {
                list.Add(ToDomainObject(organizationToDo));
            }

            return list;
        }

        /// <summary>
        /// To the organization to do contract.
        /// </summary>
        /// <param name="orgToDo">The to do item.</param>
        /// <returns></returns>
        public static OrganizationToDoListContract ToOrgToDoContract(this OrganizationToDoModel orgToDo)
        {
            return new OrganizationToDoListContract
            {
                ContentID = orgToDo.ContentId,
                Headline = orgToDo.Headline,
                RefToDoStatusID = orgToDo.StatusId,
                RefToDoTypeID = orgToDo.TypeId,
                CreatedBy = orgToDo.CreatedBy,
                CreatedDate = orgToDo.CreatedDate,
                RefOrganizationID = orgToDo.OrganizationId,
                ModifiedBy = orgToDo.ModifiedBy,
                ModifiedDate = orgToDo.ModifiedDate,
                DueDate = orgToDo.DueDate
            };

        }

        /// <summary>
        /// Converts the OrganizationToDoListContract object into a OrganizationToDoModel object.
        /// </summary>
        /// <param name="organizationToDoListItem">OrganizationToDoListContract object</param>
        /// <returns>OrganizationToDoModel object</returns>
        public static OrganizationToDoModel ToDomainObject(OrganizationToDoListContract organizationToDoListItem)
        {
            return new OrganizationToDoModel()
            {
                ContentId = organizationToDoListItem.ContentID,
                Headline = organizationToDoListItem.Headline,
                StatusId = organizationToDoListItem.RefToDoStatusID,
                TypeId = organizationToDoListItem.RefToDoTypeID,
                CreatedBy = organizationToDoListItem.CreatedBy,
                CreatedDate = organizationToDoListItem.CreatedDate,
                ModifiedBy = organizationToDoListItem.ModifiedBy,
                ModifiedDate = organizationToDoListItem.ModifiedDate,
                DueDate = organizationToDoListItem.DueDate
            };
        }
        /// <summary>
        /// Converts the Endeca response object into a ContentModel object.
        /// </summary>
        /// <param name="endecaContentModel">EndecaContentModel</param>
        /// <returns>ContentModel object</returns>
        public static ContentModel ToContentModel(EndecaContentModel endecaContentModel)
        {
            return new ContentModel()
            {
                ContentId = endecaContentModel.PrimaryKey,
                PageTitle = endecaContentModel.PageTitle,
                ContentType = endecaContentModel.ContentType
            };
        }
    }
}
