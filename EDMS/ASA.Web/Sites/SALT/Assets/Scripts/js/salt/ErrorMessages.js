/*!
 * SALT: Error Messages
 */

define(function (require, exports, module) {
    var SALT = require('salt');

    SALT.ErrorMessages = {
        InvalidAge: "Sorry, you are ineligible for Salt based on your age.",
        InvalidEmailAddress: "Please provide a valid email address.",
        InvalidPhone: "Please specify a valid phone number.",
        RequiredSecurityAnswer: "Security Answer is required.",
        RequiredLastName: "Last name is required.",
        RequiredNewPassword: "New password is required.",
        RequiredDate: "Please select a valid date.",
        RequiredConfirmPassword: "Confirmed password is required.",
        RequiredSchool: "Please select a school.",
        PasswordDoNotMatch: "Passwords do not match.",
        BasicRequired: "This field is required.",
        NameRequirement: "Please enter a valid name.",
        NumbersNotAllowed: "Numbers are not allowed.",
        NumbersOnly: "Numbers only please.",
        EmailsDoNotMatch: "Email addresses do not match.",
        QASServiceInterruption: "We can not confirm your address, please try entering it again. If this problem persists, click on the Contact Us link and let us know.",
        SchoolLookupError: "An error occurred. Please try again.",
        SaveLoanStatus: "Saving Loans",
        RetrieveLoanStatus: "Retrieving Loans",
        CheckingForRewardsStatus: "Checking for rewards"
    };

    return SALT.ErrorMessages;
});