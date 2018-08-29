using System;
using ASA.Web.Services.AppToolService.Proxy.AppTool;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.Proxy
{
    public static class TranslateAppToolModel
    {
        // TRANSLATION (SOAP --> AppToolModel)
        public static AppToolModel MapGetResponseToModel(GetAppToolResponse response)
        {
            AppToolModel appTool = new AppToolModel();

            if (response != null)
            {
                if (response.AppToolCanonicalList != null && 
                    response.AppToolCanonicalList.Length > 0
                    )
                {
                    if (response.AppToolCanonicalList[0].AppToolTier1 != null)
                    {
                        appTool.PersonId = response.AppToolCanonicalList[0].AppToolTier1.PersonId;
                    }

                    if(response.AppToolCanonicalList[0].AppToolTier2 != null &&
                    response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType != null)
                    {
                        appTool.AppToolPersonId = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.AppToolPersonId;
                        appTool.AppToolType = (int)response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.AppToolType;
                        appTool.BalanceAtStartDeferment = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.BalanceAtStartDeferment;
                        appTool.ForbearancePaymentAmount = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.ForbearancePaymentAmount;
                        appTool.InterestRate = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.InterestRate;
                        appTool.MonthlyIncome = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.MonthlyIncome;
                        appTool.NumberOfForbearancePayments = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.NumberOfForbearancePayments;
                        appTool.NumberOfMonthsInDeferment = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.NumberOfMonthsInDeferment;
                        appTool.NumberOfMonthsInForbearance = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.NumberOfMonthsInForbearance;
                        appTool.NumberOfPayments = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.NumberOfPayments;
                        appTool.PrincipalAmount = response.AppToolCanonicalList[0].AppToolTier2.AppToolInfoType.PrincipalAmount;
                    }

                }
                else if (response.ResponseMessageList != null && response.ResponseMessageList.Count > 0)
                {
                    for (int i = 0; i < response.ResponseMessageList.Count; i++)
                    {
                        ErrorModel error = new ErrorModel(response.ResponseMessageList[i].MessageDetails, "Web AppTool Service", response.ResponseMessageList[i].ResponseCode);
                        appTool.ErrorList.Add(error);
                    }
                }
                else
                {
                    ErrorModel error = new ErrorModel("An error occured when trying to retrieve AppTool information", "Web AppTool Service");
                    appTool.ErrorList.Add(error);
                }
            }
            else
            {
                ErrorModel error = new ErrorModel("No valid response was received from the service", "Web AppTool Service");
                appTool.ErrorList.Add(error);
            }
            // then map from response to AppToolModel...
            return appTool;
        }

        // TRANSLATION (AppToolPersonId --> SOAP Request)  
        public static GetAppToolRequest MapAppToolIdToGetRequest(int personId, int toolType)
        {
            GetAppToolRequest request = new GetAppToolRequest();

            // construct request to get a Calculator for this PersonId, and this AppToolType
            CriteriaAppTool_PersonAppToolType personAppToolTypeCriteria = new CriteriaAppTool_PersonAppToolType();
            personAppToolTypeCriteria.CriterionCustomerId = new CriterionCustomerIdType();
            personAppToolTypeCriteria.CriterionCustomerId.CustomerId = 2;
            personAppToolTypeCriteria.CriterionCustomerId.LogicalOperator = LogicalOperatorType.AND;
            personAppToolTypeCriteria.CriterionCustomerId.RelationalOperator = RelationalOperatorType.EQUALS;
            personAppToolTypeCriteria.CriterionPersonId = new CriterionPersonIdType();
            personAppToolTypeCriteria.CriterionPersonId.PersonId = personId;
            personAppToolTypeCriteria.CriterionPersonId.LogicalOperator = LogicalOperatorType.AND;
            personAppToolTypeCriteria.CriterionPersonId.RelationalOperator = RelationalOperatorType.EQUALS;
            personAppToolTypeCriteria.CriterionAppToolTypeId = new CriterionAppToolTypeIdType();
            personAppToolTypeCriteria.CriterionAppToolTypeId.ToolTypeId = (ApplicationToolType)Enum.ToObject(typeof(ApplicationToolType), toolType);
            personAppToolTypeCriteria.CriterionAppToolTypeId.LogicalOperator = LogicalOperatorType.AND;
            personAppToolTypeCriteria.CriterionAppToolTypeId.RelationalOperator = RelationalOperatorType.EQUALS;

            personAppToolTypeCriteria.ListReturnTypes = new ReturnListType();
            personAppToolTypeCriteria.ListReturnTypes.AppToolTier2Type = YNFlagType.Y;
            personAppToolTypeCriteria.MaxEntities = 50;

            request.Criteria = personAppToolTypeCriteria as AppToolServiceCriteriaType;
            
            return request;
        }


        public static AppToolCanonicalType MapModelToCanonical(AppToolModel appTool)
        {
            //TRANSLATION (AppToolModel --> Canonical)

            AppToolCanonicalType appToolCanonicalType = new AppToolCanonicalType();
            appToolCanonicalType.AppToolTier1 = new AppToolTier1Type();
            appToolCanonicalType.AppToolTier1.CustomerId = 2 ;
            appToolCanonicalType.AppToolTier1.PersonId = appTool.PersonId;
            appToolCanonicalType.AppToolTier2 = new AppToolTier2Type();
            appToolCanonicalType.AppToolTier2.AppToolInfoType = new AppToolTier2Type.AppToolInfoTypeType();
            appToolCanonicalType.AppToolTier2.AppToolInfoType.AppToolPersonId = appTool.AppToolPersonId;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.AppToolType = (ApplicationToolType)Enum.ToObject(typeof(ApplicationToolType), appTool.AppToolType);
            appToolCanonicalType.AppToolTier2.AppToolInfoType.BalanceAtStartDeferment = appTool.BalanceAtStartDeferment;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.ForbearancePaymentAmount = appTool.ForbearancePaymentAmount;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.InterestRate = appTool.InterestRate;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.MonthlyIncome = appTool.MonthlyIncome;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.NumberOfForbearancePayments = appTool.NumberOfForbearancePayments;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.NumberOfMonthsInDeferment = appTool.NumberOfMonthsInDeferment;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.NumberOfMonthsInForbearance = appTool.NumberOfMonthsInForbearance;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.NumberOfPayments = appTool.NumberOfPayments;
            appToolCanonicalType.AppToolTier2.AppToolInfoType.PrincipalAmount = appTool.PrincipalAmount;
            //appToolCanonicalType.AppToolTier2.AppToolInfoType.RecordVersion = appTool.RecordVersion;

            return appToolCanonicalType;
        }
    }
}