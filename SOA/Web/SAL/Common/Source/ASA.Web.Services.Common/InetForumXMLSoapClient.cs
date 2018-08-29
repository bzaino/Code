﻿using System;
using System.ServiceModel;
namespace ASA.Web.Services.Common.xWeb
{
    public interface InetForumXMLSoapClient : netForumXMLSoap, ICommunicationObject
    {
        AuthorizationToken Authenticate(string userName, string password, out string AuthenticateResult);
        void CreateAdvocacyData(ref AuthorizationToken AuthorizationToken, System.Xml.XmlNode oNode);
        System.Xml.XmlNode CreateInvoice(ref AuthorizationToken AuthorizationToken, System.Xml.XmlNode InvoiceNode);
        System.Xml.XmlNode CreatePayment(ref AuthorizationToken AuthorizationToken, System.Xml.XmlNode oNode);
        System.Xml.XmlNode ExecuteMethod(ref AuthorizationToken AuthorizationToken, string serviceName, string methodName, Parameter[] parameters);
        DateTime GetDateTime();
        System.Xml.XmlNode GetDynamicQuery(ref AuthorizationToken AuthorizationToken, string szObjectName, string szQueryName, string WithDescriptions, Parameter[] Parameters);
        QueryDefinition GetDynamicQueryDefinition(ref AuthorizationToken AuthorizationToken, string szObjectName, string szQueryName, string que_key);
        System.Xml.XmlNode GetFacadeObject(ref AuthorizationToken AuthorizationToken, string szObjectName, string szObjectKey);
        System.Xml.XmlNode GetFacadeObjectList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode GetFacadeXMLSchema(ref AuthorizationToken AuthorizationToken, string szObjectName);
        System.Xml.XmlNode GetIndividualInformation(ref AuthorizationToken AuthorizationToken, string IndividualKey);
        System.Xml.XmlNode GetOrganizationInformation(ref AuthorizationToken AuthorizationToken, string OrganizationKey);
        System.Xml.XmlNode GetQuery(ref AuthorizationToken AuthorizationToken, string szObjectName, string szColumnList, string szWhereClause, string szOrderBy);
        Object GetQueryDefinition(ref AuthorizationToken AuthorizationToken, string szObjectName);
        string GetVersion(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode InsertFacadeObject(ref AuthorizationToken AuthorizationToken, string szObjectName, System.Xml.XmlNode oNode);
        AVForm MetaDataGetForm(ref AuthorizationToken AuthorizationToken, Guid FormKey);
        AVForm MetaDataGetFormForFacadeObject(ref AuthorizationToken AuthorizationToken, Guid FormKey, string szObjectName, System.Xml.XmlNode oNode);
        AVForm MetaDataGetFormForIndividual(ref AuthorizationToken AuthorizationToken, Guid FormKey, IndividualType oFacadeObject);
        System.Xml.XmlNode NewIndividualInformation(ref AuthorizationToken AuthorizationToken, System.Xml.XmlNode oNode);
        System.Xml.XmlNode NewOrganizationInformation(ref AuthorizationToken AuthorizationToken, System.Xml.XmlNode oNode);
        System.Xml.XmlNode SetIndividualInformation(ref AuthorizationToken AuthorizationToken, string IndividualKey, System.Xml.XmlNode oUpdateNode);
        System.Xml.XmlNode SetOrganizationInformation(ref AuthorizationToken AuthorizationToken, string OrganizationKey, System.Xml.XmlNode oUpdateNode);
        string TestConnection();
        System.Xml.XmlNode UpdateFacadeObject(ref AuthorizationToken AuthorizationToken, string szObjectName, string szObjectKey, System.Xml.XmlNode oNode);
        bool WEBActivityAlreadyRegisteredForEvent(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid EventKey);
        System.Xml.XmlNode WEBActivityGetPurchasedChapterMembershipsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBActivityGetPurchasedDownoadableProductsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBActivityGetPurchasedEventsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBActivityGetPurchasedMembershipsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBActivityGetPurchasedProductsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBActivityGetRegistrantEvents(ref AuthorizationToken AuthorizationToken, Guid RegKey);
        System.Xml.XmlNode WEBActivityGetRegistrantGuests(ref AuthorizationToken AuthorizationToken, Guid RegKey);
        System.Xml.XmlNode WEBActivityGetRegistrantSessions(ref AuthorizationToken AuthorizationToken, Guid RegKey);
        System.Xml.XmlNode WEBActivityGetRegistrantTracks(ref AuthorizationToken AuthorizationToken, Guid RegKey);
        int WEBActivityNumberOfRegisteredGuests(ref AuthorizationToken AuthorizationToken, Guid RegistrationKey);
        CustomerAddressType WEBAddressGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBAddressGetAddressesByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBAddressGetCountries(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBAddressGetStates(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBAddressGetTypes(ref AuthorizationToken AuthorizationToken);
        CustomerAddressType WEBAddressInsert(ref AuthorizationToken AuthorizationToken, CustomerAddressType oFacadeObject);
        bool WEBAddressUpdate(ref AuthorizationToken AuthorizationToken, CustomerAddressType oFacadeObject);
        CentralizedOrderEntryType WEBCentralizedShoppingCartAddEventRegistrant(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, EventsRegistrantType oRegistration);
        CentralizedOrderEntryType WEBCentralizedShoppingCartAddEventRegistrantGroup(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, EventsRegistrantGroupType oGroupRegistration);
        EventsRegistrantGroupType WEBCentralizedShoppingCartAddEventRegistrantToGroup(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration, EventsRegistrantGroupType oGroupRegistration);
        CentralizedOrderEntryType WEBCentralizedShoppingCartAddLineItem(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, InvoiceDetailStaticType oLineItem);
        CentralizedOrderEntryType WEBCentralizedShoppingCartAddShippingItem(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, InvoiceDetailStaticType oShippingItem);
        CentralizedOrderEntryType WEBCentralizedShoppingCartApplyDiscountCode(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, string szDiscountCode);
        CentralizedOrderEntryType WEBCentralizedShoppingCartApplySourceCode(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, string szSourceCode);
        CentralizedOrderEntryType WEBCentralizedShoppingCartClearSourceCode(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry);
        EventsRegistrantType WEBCentralizedShoppingCartEventRegistrantGet(ref AuthorizationToken AuthorizationToken, Guid Key);
        EventsRegistrantType WEBCentralizedShoppingCartEventRegistrantGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid EventKey);
        EventsRegistrantGroupType WEBCentralizedShoppingCartEventRegistrantGroupGet(ref AuthorizationToken AuthorizationToken, Guid Key);
        EventsRegistrantGroupType WEBCentralizedShoppingCartEventRegistrantGroupGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid EventKey);
        EventsRegistrantGroupType WEBCentralizedShoppingCartEventRegistrantGroupRefresh(ref AuthorizationToken AuthorizationToken, EventsRegistrantGroupType oGroupRegistration);
        EventsRegistrantType WEBCentralizedShoppingCartEventRegistrantRefresh(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration);
        EventsRegistrantType WEBCentralizedShoppingCartEventRegistrantSetLineItems(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartEventRegistrantSetLineItemsWithCart(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid RegistrationKey, Fee[] oFeeCollection);
        EventsRegistrantGroupType WEBCentralizedShoppingCartEventRegistrantSetLineItemsWithGroupRegistration(ref AuthorizationToken AuthorizationToken, EventsRegistrantGroupType oGroupRegistration, Guid RegistrationKey, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartExhibitorAddExhibitor(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, ExhibitorNewType oExhibitor);
        ExhibitorNewType WEBCentralizedShoppingCartExhibitorGet(ref AuthorizationToken AuthorizationToken, Guid ExhibitorKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartExhibitorGetBoothCategoryList(ref AuthorizationToken AuthorizationToken, Guid ExhibitKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartExhibitorGetBoothList(ref AuthorizationToken AuthorizationToken, ExhibitorNewType oExhibitor, string BoothType, string BoothCategory, string ProductName);
        System.Xml.XmlNode WEBCentralizedShoppingCartExhibitorGetBoothTypeList(ref AuthorizationToken AuthorizationToken, Guid ExhibitKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartExhibitorGetExhibitList(ref AuthorizationToken AuthorizationToken);
        ExhibitorNewType WEBCentralizedShoppingCartExhibitorGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid ExhibitKey);
        ExhibitorNewType WEBCentralizedShoppingCartExhibitorSetLineItems(ref AuthorizationToken AuthorizationToken, ExhibitorNewType oExhibitor, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartExhibitorSetLineItemsWithCart(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid ExhibitorKey, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGet(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventByKey(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventFees(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration, CentralizedOrderEntryType oCOE);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventGuestRegistrantTypeListByEvent(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventListByName(ref AuthorizationToken AuthorizationToken, string EventName);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventListKeys(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventRegistrantGroupIndividualList(ref AuthorizationToken AuthorizationToken, Guid EventKey, Guid IndividualCustomerKey, Guid OrganizationCustomerKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventRegistrantRoomTypeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventRegistrantSourceCodeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventRegistrantTypeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventRegistrantTypeListByEvent(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventSessionFees(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration, CentralizedOrderEntryType oCOE);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetEventTrackFees(ref AuthorizationToken AuthorizationToken, EventsRegistrantType oRegistration, CentralizedOrderEntryType oCOE);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetFacultyListByEventKey(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetFacultyListBySessionKey(ref AuthorizationToken AuthorizationToken, Guid SessionKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetInstallmentFrequencyOptions(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetInstallmentTermsOptions(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetMerchandiseList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetMerchandiseList_Ignore_PC(ref AuthorizationToken AuthorizationToken);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetPaymentOptions(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentalizedOrderEntry);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductByKey(ref AuthorizationToken AuthorizationToken, Guid ProductKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductCategoryList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductComplementListByProductKey(ref AuthorizationToken AuthorizationToken, Guid ProductKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductComplements(ref AuthorizationToken AuthorizationToken);
        InvoiceDetailStaticType WEBCentralizedShoppingCartGetProductLineItem(ref AuthorizationToken AuthorizationToken, Guid ProductKey, Guid CustomerKey, Guid Customer_X_Address_Key);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductListAlsoPurchasedByProductKey(ref AuthorizationToken AuthorizationToken, Guid ProductKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductListByName(ref AuthorizationToken AuthorizationToken, string ProductName);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductListKeys(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductSubstituteListByProductKey(ref AuthorizationToken AuthorizationToken, Guid ProductKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductSubstitutes(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetProductTypeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetPublicationList_Ignore_PC(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetSessionListByEventKey(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetSessionListByTrackKey(ref AuthorizationToken AuthorizationToken, Guid TrackKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetShippingOptions(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentalizedOrderEntry);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetSponsorListByEventKey(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetSponsorListBySessionKey(ref AuthorizationToken AuthorizationToken, Guid SessionKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetSubscriptionList_Ignore_PC(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGetTrackListByEventKey(ref AuthorizationToken AuthorizationToken, Guid EventKey);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGetWithLineItem(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid ProductKey, Guid Customer_X_Address_Key);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGiftAddGiftFundraising(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, FundraisingGiftType oFundraisingGift);
        FundraisingGiftType WEBCentralizedShoppingCartGiftFundraisingGet(ref AuthorizationToken AuthorizationToken, Guid Key);
        FundraisingGiftType WEBCentralizedShoppingCartGiftFundraisingGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid GiftKey);
        FundraisingGiftType WEBCentralizedShoppingCartGiftFundraisingRefresh(ref AuthorizationToken AuthorizationToken, FundraisingGiftType oFundraisingGift);
        FundraisingGiftType WEBCentralizedShoppingCartGiftFundraisingSetLineItems(ref AuthorizationToken AuthorizationToken, FundraisingGiftType oFundraisingGift, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGiftFundraisingSetLineItemsWithCart(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid FundraisingGiftKey, Fee[] oFeeCollection);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetGiftProductByKey(ref AuthorizationToken AuthorizationToken, Guid GiftKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetGiftProductList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetGiftProductListByName(ref AuthorizationToken AuthorizationToken, string GiftName);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetGiftProductListKeys(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetGiftTypeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartGiftGetPremiumProductsListByGift(ref AuthorizationToken AuthorizationToken, Guid GiftKey, decimal GiftAmount);
        CentralizedOrderEntryType WEBCentralizedShoppingCartGiftRemoveFundraisingGift(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid FundraisingGiftKey);
        CentralizedOrderEntryType WEBCentralizedShoppingCartInsert(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry);
        InvoiceDetailStaticType WEBCentralizedShoppingCartLoadLineItem(ref AuthorizationToken AuthorizationToken, InvoiceDetailStaticType oLineItem);
        CentralizedOrderEntryType WEBCentralizedShoppingCartMebmershipRemoveMembership(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid MembershipKey);
        CentralizedOrderEntryType WEBCentralizedShoppingCartMembershipAddMembership(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, mb_membershipType oMembership);
        mb_membershipType WEBCentralizedShoppingCartMembershipGet(ref AuthorizationToken AuthorizationToken, Guid Key);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetMembershipTypeList(ref AuthorizationToken AuthorizationToken);
        mb_membershipType WEBCentralizedShoppingCartMembershipGetNew(ref AuthorizationToken AuthorizationToken, Guid CustomerKey, Guid PackageKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageByKey(ref AuthorizationToken AuthorizationToken, Guid PackageKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageComponentList(ref AuthorizationToken AuthorizationToken, Guid PackageKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageComponentListFromObject(ref AuthorizationToken AuthorizationToken, mb_membershipType oMembership);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageListByMembershipTypeKey(ref AuthorizationToken AuthorizationToken, Guid MembershipTypeKey);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageListByName(ref AuthorizationToken AuthorizationToken, string PackageName);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetPackageListKeys(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipGetRenewalPackageList(ref AuthorizationToken AuthorizationToken, mb_membershipType oMembership);
        CentralizedOrderEntryType WEBCentralizedShoppingCartMembershipOpenInvoiceAdd(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, InvoiceType oOpenInvoice);
        InvoiceType WEBCentralizedShoppingCartMembershipOpenInvoiceGet(ref AuthorizationToken AuthorizationToken, Guid Key);
        System.Xml.XmlNode WEBCentralizedShoppingCartMembershipOpenInvoiceGetList(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        mb_membershipType WEBCentralizedShoppingCartMembershipRefresh(ref AuthorizationToken AuthorizationToken, mb_membershipType oMembership);
        CentralizedOrderEntryType WEBCentralizedShoppingCartMembershipSetLineItemsWithCart(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid MembershipKey, Fee[] oFeeCollection);
        mb_membershipType WEBCentralizedShoppingCartMembesrhipSetLineItems(ref AuthorizationToken AuthorizationToken, mb_membershipType oMembership, Fee[] oFeeCollection);
        CentralizedOrderEntryType WEBCentralizedShoppingCartRefresh(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry);
        CentralizedOrderEntryType WEBCentralizedShoppingCartRemoveAllLineItems(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry);
        CentralizedOrderEntryType WEBCentralizedShoppingCartRemoveEventRegistrant(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid RegistrationKey);
        EventsRegistrantGroupType WEBCentralizedShoppingCartRemoveEventRegistrantFromGroup(ref AuthorizationToken AuthorizationToken, EventsRegistrantGroupType oGroupRegistration, Guid RegistrationKey);
        CentralizedOrderEntryType WEBCentralizedShoppingCartRemoveEventRegistrantGroup(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid GroupRegistrationKey);
        CentralizedOrderEntryType WEBCentralizedShoppingCartRemoveLineItem(ref AuthorizationToken AuthorizationToken, CentralizedOrderEntryType oCentralizedOrderEntry, Guid szLineItemKey);
        ChapterType WEBChapterGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBChaptersGetChapterByKey(ref AuthorizationToken AuthorizationToken, Guid ChapterKey);
        System.Xml.XmlNode WEBChaptersGetChapterList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBChaptersGetChapterListByName(ref AuthorizationToken AuthorizationToken, string ChapterName);
        System.Xml.XmlNode WEBChaptersGetChapterMembershipRoster(ref AuthorizationToken AuthorizationToken, Guid ChapterKey);
        System.Xml.XmlNode WEBChaptersGetChapterOfficers(ref AuthorizationToken AuthorizationToken, Guid ChapterKey);
        CommitteeType WEBCommitteeGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBCommitteeGetCommitteeByKey(ref AuthorizationToken AuthorizationToken, Guid CommitteeKey);
        System.Xml.XmlNode WEBCommitteeGetCommitteeList(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBCommitteeGetCommitteeListByName(ref AuthorizationToken AuthorizationToken, string CommitteeName);
        System.Xml.XmlNode WEBCommitteeGetCommitteesByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBCommitteeGetDocuments(ref AuthorizationToken AuthorizationToken, Guid CommitteeKey);
        System.Xml.XmlNode WEBCommitteeGetMembers(ref AuthorizationToken AuthorizationToken, Guid CommitteeKey);
        System.Xml.XmlNode WEBCommitteeGetPositionList(ref AuthorizationToken AuthorizationToken, Guid CommitteeKey);
        System.Xml.XmlNode WEBCommitteeGetSubCommitteeListByCommittee(ref AuthorizationToken AuthorizationToken, Guid CommitteeKey);
        CommitteeNominationsType WEBCommitteeNominationInsert(ref AuthorizationToken AuthorizationToken, CommitteeNominationsType oFacadeObject);
        System.Xml.XmlNode WEBContactRequestGetOriginations(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBContactRequestGetPriorities(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBContactRequestGetRequestTypeReasons(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBContactRequestGetRequestTypes(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBContactRequestGetStatuses(ref AuthorizationToken AuthorizationToken);
        bool WEBContactRequestInsert(ref AuthorizationToken AuthorizationToken, ContactRequestType oFacadeObject);
        CustomerEmailType WEBEmailGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBEmailGetEmailsByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        CustomerEmailType WEBEmailInsert(ref AuthorizationToken AuthorizationToken, CustomerEmailType oFacadeObject);
        bool WEBEmailUpdate(ref AuthorizationToken AuthorizationToken, CustomerEmailType oFacadeObject);
        CustomerFaxType WEBFaxGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBFaxGetFaxesByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBFaxGetTypes(ref AuthorizationToken AuthorizationToken);
        CustomerFaxType WEBFaxInsert(ref AuthorizationToken AuthorizationToken, CustomerFaxType oFacadeObject);
        bool WEBFaxUpdate(ref AuthorizationToken AuthorizationToken, CustomerFaxType oFacadeObject);
        System.Xml.XmlNode WEBFindUsersInRole(ref AuthorizationToken AuthorizationToken, string roleName, string usernameToMatch);
        System.Xml.XmlNode WEBGetAllRoles(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBGetRolesForUser(ref AuthorizationToken AuthorizationToken, string username);
        System.Xml.XmlNode WEBGetSystemOptions(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBGetUsersInRole(ref AuthorizationToken AuthorizationToken, string roleName);
        IndividualType WEBIndividualGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBIndividualGetPrefixes(ref AuthorizationToken AuthorizationToken);
        System.Xml.XmlNode WEBIndividualGetSuffixes(ref AuthorizationToken AuthorizationToken);
        IndividualType WEBIndividualInsert(ref AuthorizationToken AuthorizationToken, IndividualType oFacadeObject);
        bool WEBIndividualUpdate(ref AuthorizationToken AuthorizationToken, IndividualType oFacadeObject);
        bool WEBIsUserInRole(ref AuthorizationToken AuthorizationToken, string username, string roleName);
        string WebLogin(ref AuthorizationToken AuthorizationToken, string userLoginPlain, string passwordPlain, string keyOverride);
        void WebLogout(ref AuthorizationToken AuthorizationToken, string authenticationToken);
        System.Xml.XmlNode WEBMemberDirectoryOrganizationSearch(ref AuthorizationToken AuthorizationToken, string OrganizationAcronym, string OrganizationName, string OrganizationType, string City, string State, string Country);
        System.Xml.XmlNode WEBMemberDirectorySearch(ref AuthorizationToken AuthorizationToken, string FirstName, string LastName, string OrganizationName, string City, string State, string Country);
        OrganizationType WEBOrganizationGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBOrganizationGetTypes(ref AuthorizationToken AuthorizationToken);
        OrganizationType WEBOrganizationInsert(ref AuthorizationToken AuthorizationToken, OrganizationType oFacadeObject);
        bool WEBOrganizationUpdate(ref AuthorizationToken AuthorizationToken, OrganizationType oFacadeObject);
        CustomerPhoneType WEBPhoneGet(ref AuthorizationToken AuthorizationToken, Guid key);
        System.Xml.XmlNode WEBPhoneGetPhonesByCustomer(ref AuthorizationToken AuthorizationToken, Guid CustomerKey);
        System.Xml.XmlNode WEBPhoneGetTypes(ref AuthorizationToken AuthorizationToken);
        CustomerPhoneType WEBPhoneInsert(ref AuthorizationToken AuthorizationToken, CustomerPhoneType oFacadeObject);
        bool WEBPhoneUpdate(ref AuthorizationToken AuthorizationToken, CustomerPhoneType oFacadeObject);
        bool WEBRoleExists(ref AuthorizationToken AuthorizationToken, string roleName);
        bool WEBUpdateSystemOption(ref AuthorizationToken AuthorizationToken, string szOptionName, string szOptionValue);
        string WebValidate(ref AuthorizationToken AuthorizationToken, string authenticationToken);
        bool WEBWebUserChangePassword(ref AuthorizationToken AuthorizationToken, string recno, string oldPassword, string newPassword);
        WebUserType WEBWebUserCreate(ref AuthorizationToken AuthorizationToken, WebUserType oWebUser);
        System.Xml.XmlNode WEBWebUserFindOrganizationsByDomain(ref AuthorizationToken AuthorizationToken, string domainToMatch);
        System.Xml.XmlNode WEBWebUserFindUsersByDomain(ref AuthorizationToken AuthorizationToken, string domainToMatch);
        System.Xml.XmlNode WEBWebUserFindUsersByEmail(ref AuthorizationToken AuthorizationToken, string emailToMatch);
        System.Xml.XmlNode WEBWebUserFindUsersByName(ref AuthorizationToken AuthorizationToken, string usernameToMatch);
        System.Xml.XmlNode WEBWebUserFindUsersByUserNameFirstNameLastName(ref AuthorizationToken AuthorizationToken, string usernameToMatch, string firstnameToMatch, string lastnameToMatch);
        WebUserType WEBWebUserGet(ref AuthorizationToken AuthorizationToken, string cst_key);
        WebUserType WEBWebUserGetByRecno_Custom(ref AuthorizationToken AuthorizationToken, string cst_recno);
        bool WEBWebUserLock(ref AuthorizationToken AuthorizationToken, string cst_recno);
        WebUserType WEBWebUserLogin(ref AuthorizationToken AuthorizationToken, string LoginOrEmail, string password);
        string WEBWebUserLogin_Custom(ref AuthorizationToken AuthorizationToken, string LoginOrEmail, string password);
        string WEBWebUserLoginByRecno_Custom(ref AuthorizationToken AuthorizationToken, string cst_recno, string password);
        bool WEBWebUserUnlock(ref AuthorizationToken AuthorizationToken, string cst_recno);
        WebUserType WEBWebUserUpdate(ref AuthorizationToken AuthorizationToken, WebUserType oWebUser);
        WebUserType WEBWebUserValidateLogin(ref AuthorizationToken AuthorizationToken, string LoginOrEmail, string password);
        string WEBWebUserValidateLogin_Custom(ref AuthorizationToken AuthorizationToken, string LoginOrEmail, string password);
        WebUserType WEBWebUserValidateToken(ref AuthorizationToken AuthorizationToken, string authenticationToken);
        string WEBWebUserValidateToken_Custom(ref AuthorizationToken AuthorizationToken, string authenticationToken);
    }
}
