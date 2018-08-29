define([
    'modules/Profile/PersonalInformation',
    'modules/Profile/FinancialInformation',
    'modules/Profile/OrganizationInformation',
    'modules/Profile/AccountDetails'
], function (PersonalInformation, FinancialInformation, OrganizationInformation, AccountDetails) {

    /* Create facade object to hold all the different views so that the application can pull in this file,
        rather than all of the views individually */
    return {
        PersonalInformation : PersonalInformation,
        FinancialInformation : FinancialInformation,
        OrganizationInformation : OrganizationInformation,
        AccountDetails : AccountDetails
    };

});