using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AppToolService.Proxy.AppTool;
using ASA.Web.Services.Common;
using ASA.Web.Services.AppToolService.Proxy;

namespace ASA.Web.Services.PersonService.Proxy
{
    public class MockInvokeAppToolService : IInvokeAppToolService
    {

        #region Get

        public GetAppToolResponse GetAppTool(GetAppToolRequest getRequest)
        {
            //Populate fake GetAppToolResponse object....
            GetAppToolResponse response = new GetAppToolResponse();
            AppToolCanonicalType[] appToolList = { new AppToolCanonicalType()};
            AppToolCanonicalType appToolCanonical = appToolList.First<AppToolCanonicalType>();

            appToolCanonical.AppToolTier1 = new AppToolTier1Type();
            appToolCanonical.AppToolTier1.PersonId = ((CriteriaAppTool_PersonAppToolType)getRequest.Criteria).CriterionPersonId.PersonId;
            appToolCanonical.AppToolTier1.CustomerId = 2;

            appToolCanonical.AppToolTier2 = new AppToolTier2Type();
            appToolCanonical.AppToolTier2.AppToolInfoType = new AppToolTier2Type.AppToolInfoTypeType();
            appToolCanonical.AppToolTier2.AppToolInfoType.AppToolType = ((CriteriaAppTool_PersonAppToolType)getRequest.Criteria).CriterionAppToolTypeId.ToolTypeId;

            appToolCanonical.AppToolTier2.AppToolInfoType.AppToolPersonId = 1;
            appToolCanonical.AppToolTier2.AppToolInfoType.BalanceAtStartDeferment = 20000;
            appToolCanonical.AppToolTier2.AppToolInfoType.ForbearancePaymentAmount = 500;
            appToolCanonical.AppToolTier2.AppToolInfoType.InterestRate = 6.200;
            appToolCanonical.AppToolTier2.AppToolInfoType.MonthlyIncome = 2500;
            appToolCanonical.AppToolTier2.AppToolInfoType.NumberOfForbearancePayments = 6;
            appToolCanonical.AppToolTier2.AppToolInfoType.NumberOfMonthsInDeferment = 4;
            appToolCanonical.AppToolTier2.AppToolInfoType.NumberOfMonthsInForbearance = 3;
            appToolCanonical.AppToolTier2.AppToolInfoType.NumberOfPayments = 120;
            appToolCanonical.AppToolTier2.AppToolInfoType.PrincipalAmount = 18000;

            response.AppToolCanonicalList = appToolList;
            return response;
        }

        #endregion

        #region Set
        public ResultCodeModel SaveAppTool(AppToolCanonicalType appTool)
        {
            ResultCodeModel rcm = new ResultCodeModel();
            rcm.ResultCode = 1;// Success.
            return rcm;
        }

        #endregion

    }
}
