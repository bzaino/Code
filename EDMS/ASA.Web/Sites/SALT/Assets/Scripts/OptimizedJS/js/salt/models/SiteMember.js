/*!
 * SALT / SiteMember Model
 */
//Declare a global variable outside of the require statement, so that it will stay in scope
//This variable is necessary for the SiteIntercept features to work
var SiteMemberInfo = {};

define([
    'jquery',
    'backbone',
    'underscore',
    'salt',
    'asa/ASAUtilities',
    'asa/ASAWebService',
    'jquery.cookie'
], function ($, Backbone, _, SALT, Utility) {

    var modelDeferred = $.Deferred();

    function getGoalRankResponses(responses, enabledGoals) {
        // lowest question ID must be first in this list
        var goalQuestionIDs = [17, 18, 19, 20];
        var changedEnabledGoals = false;
        //Filter the responses down to answers to the goal ranking questions
        var goalRankResponses = _.filter(responses, function (el) {
            return goalQuestionIDs.indexOf(el.QuestionExternalId) > -1;
        });


        _.each(goalRankResponses, function (gr) {
            gr.nameWithNoSpaces = gr.AnsName.replace(/[\W_]+/g, '');
        });

        //Loop through the enabled goals, if the goal rank responses dont match the enabled goals, they have changed and we'll want to re-onboard
        //Set the "changedEnabledGoals" flag to true in this case
        _.each(enabledGoals, function (isEnabled, goal, list) {
            var responseExists = _.findWhere(goalRankResponses, {nameWithNoSpaces: goal});
            if (isEnabled && !responseExists  || !isEnabled && responseExists) {
                changedEnabledGoals = true;
            }
        });

        if (changedEnabledGoals) {
            return [];
        }

        //We need to filter out responses for goals that are no longer enabled for this user.  The organization may have disabled a goal after the user already ranked it
        //The object that this function returns gets used for building sections of html, we dont want to include disabled goals
        goalRankResponses = _.filter(goalRankResponses, function (goal) {
            if (enabledGoals[goal.nameWithNoSpaces]) {
                return true;
            }
        });

        return goalRankResponses;
    }

    return new (Backbone.Model.extend({
        defaults: {
            ErrorList: [],
            ActiveDirectoryKey: '',
            Addresses: [
                {
                    ErrorList: [],
                    AddressKey: '',
                    AddressLine1: '',
                    AddressLine2: '',
                    City: '',
                    Country: '',
                    CountryID: null,
                    ForeignPostalCode: null,
                    IsPrimary: true,
                    IsValidated: true,
                    State: '',
                    StateID: null,
                    Type: null,
                    TypeID: null,
                    Zip: ''
                }
            ],
            ContactFrequency: false,
            ContactFrequencyKey: null,
            CustomerId: 0,
            DashboardEnabled: null,
            DOB: '',
            Emails: [
                {
                    ErrorList: [],
                    EmailAddress: '',
                    EmailKey: '',
                    IndividualId: null,
                    IsPrimary: true,
                    Type: '',
                    TypeID: null
                }
            ],
            EnrollmentStatus: '',
            EnrollmentStatusEffective: null,
            ExternalId: null,
            FirstName: '',
            GradeLevel: '',
            goalRankResponses: [],
            IndividualId: '',
            IsAuthenticated: 'false',
            LastName: '',
            LegalFirstName: '',
            MemberShipFlag: false,
            MemberToken: '',
            MembershipId: '',
            MembershipStartDate: '',
            MiddleInitial: '',
            PartTimeDate: null,
            PersonId: 0,
            Phones: [
                {
                    ErrorList: [],
                    IsPrimary: true,
                    PhoneKey: '',
                    PhoneNumber: '',
                    Type: '',
                    TypeID: null
                }
            ],
            PrimaryAddressKey: '',
            PrimaryEmailKey: '',
            PrimaryOrganizationKey: '',
            PrimaryPhoneKey: '',
            Products: [
                {
                    MemberID: null,
                    RefProductID: null,
                    MemberProductValue: '',
                    IsMemberProductActive: false
                }
            ],
            Organizations: [
                {
                    ErrorList: [],
                    BranchCode: '',
                    Brand: '',
                    ExpectedGraduationYear: null,
                    EffectiveStartDate: null,
                    OECode: '',
                    SchoolId: '',
                    OrganizationName: '',
                    SchoolType: null,
                    OrganizationId : ''
                }
            ],
            OrganizationProducts: [
                {
                    ProductID: null,
                    IsOrgProductActive: false
                }
            ],
            Source: '',
            WithdrawalDate: null
        },

        idAttribute: 'IndividualId',

        initialize: function () {
            modelDeferred.promise(this);

            this.on('sync', function (model) {
                var obj = model.toJSON();

                if (obj.DashboardEnabled) {
                    //Set some client side only properties (not in SAL model) used by dust templates and front end scripts to include certain features on a per-member basis
                    obj.OnboardingEnabled = Utility.isSubscribedToProduct(obj, 6);
                    obj.BrowseDisabled = !Utility.isSubscribedToProduct(obj, 12);
                    obj.SearchDisabled = !Utility.isSubscribedToProduct(obj, 13);
                }
                obj.ShowMM101 = Utility.isSubscribedToProduct(obj, 2);


                obj.OrgAdmin = Utility.isRoleActive(obj, '3');
                obj.ShowAdminPortal = Utility.isRoleActive(obj, '4');

                modelDeferred.resolve(obj);

                if (obj.FirstName) {
                    SiteMemberInfo = {
                        membershipId: obj.MembershipId,
                        school: obj.Organizations[0],
                        firstName: obj.FirstName,
                        lastName: obj.LastName,
                        email: obj.PrimaryEmailKey,
                        //Contact Frequency boolean needs to be inverted because of some back-end logic where false means contact
                        contactFrequency: !obj.ContactFrequency,
                        yearOfBirth: obj.YearOfBirth,
                    };
                }
                SALT.publish('siteMember:sync', obj);
            });

            this.on('error', function (model, resp) {
                var obj = {
                    message: resp
                };

                modelDeferred.reject(obj);
                console.error(obj.message);
            });

            this.fetch();
        },

        sync: function (method, model, options) {
            var me = this;

            if (method === 'read') {
                //Check for the IndiviualId cookie before trying to make call to back end
                //This cookie is set upon login, if they dont have it, they aren't logged in.
                if ($.cookie('IndividualId')) {
                    SALT.services.GetMemberByIndividual(function (data, textStatus, jqXHR) {
                        var errorMessages = [];

                        if (data.ErrorList.length === 0) {
                            data.IsAuthenticated = 'true';
                            data.enabledGoals = {};
                            data.enabledGoals.RepayStudentDebt = Utility.isSubscribedToProduct(data, 8);
                            data.enabledGoals.FindAJob = Utility.isSubscribedToProduct(data, 9);
                            data.enabledGoals.PayForSchool = Utility.isSubscribedToProduct(data, 10);
                            data.enabledGoals.MasterMoney = Utility.isSubscribedToProduct(data, 11);

                            //update goalRankResponses property
                            var rankResponses = _.last(data.ProfileQAs).Responses;
                            data.goalRankResponses = getGoalRankResponses(rankResponses, data.enabledGoals);
                            options.success(data, textStatus, jqXHR);
                        } else {
                            _.each(data.ErrorList, function (element) {
                                errorMessages.push(element.BusinessMessage);
                            }, this);

                            options.error('Unable to get site member (' + errorMessages.join(', ') + ')');
                        }
                    }, function (jqXHR, textStatus, errorThrown) {
                        options.success(model.defaults, '', {});
                    }, me);
                } else {
                    //We know we are logged out with no IndividualId cookie, set data to unauthenticated
                    options.success(model.defaults, '', {});
                }
            } else {
                options.error('Method unsupported (' + method + ')');
            }
        }
    }))();
});
