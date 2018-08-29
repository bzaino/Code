define([
    'jquery',
    'salt',
    'configuration'
], function ($, SALT, configuration) {
    // Make sure SALT.services is only defined once.
    if (!SALT.services) {
        SALT.services = {
            //////////////////////////////////////////////////////////////////////////
            // BASE FUNCTIONALITY
            //////////////////////////////////////////////////////////////////////////
            asaConfig: null,

            ASAServiceCall: function (callType, endpointBase, endptOperation, successMethod, errorMethod, jsonData, ctx, contenttype, datatype, cacheValue, asyncValue) {

                if (!ctx) {
                    ctx = this;
                }

                if (!cacheValue) {
                    cacheValue = true;
                }

                if (!asyncValue) {
                    asyncValue = true;
                }

                jQuery.support.cors = true;

                var endpoint = endpointBase + endptOperation;

                if (!contenttype) {
                    contenttype = 'application/json; charset=utf-8';
                }
                if (!datatype) {
                    datatype = 'JSON';
                }

                $.ajax({
                    type: callType,
                    contentType: contenttype,
                    context: ctx,
                    url: endpoint,
                    cache: cacheValue,
                    data: jsonData,
                    dataType: datatype,
                    async: asyncValue,
                    success: function (data, textStatus, jqXHR) {
                        var self = this,
                            args = arguments;

                        require(['salt/global'], function () {
                            //check that they exist first before trying to clear/reset.
                            if (successMethod) {
                                successMethod.apply(self, args); //where 'this' refers to the 'ctx' object passed-in
                            }
                        });
                    },
                    error: function (data, textStatus, jqXHR) {
                        var self = this,
                            args = arguments;

                        require(['salt/global'], function () {
                            //check that they exist first before trying to clear/reset.
                            errorMethod.apply(self, args); //where 'this' refers to the 'ctx' object passed-in
                        });
                    },
                    jsonp: null,
                    jsonpCallback: null
                });
            },

            GetMemberByIndividual: function (successCallback, errorCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/GetMember/Individual', successCallback, errorCallback, '{}', ctx, null, null, false, true);
            },

            // PUT RegisterMember
            RegisterMember: function (formData, successCallback, errorCallback, ctx, cache, async) {
                formData.Emails = [{
                    EmailAddress : formData.EmailAddress,
                    Type : 'Primary',
                    TypeID : 'P'
                }];

                var sessionString = '?Session=true';
                if (formData.SpecialEvent) {
                    sessionString = '?Session=false';
                }

                formData = JSON.stringify(formData);

                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/Member/' + sessionString, successCallback, errorCallback, formData, ctx, null, null, cache, async);
            },

            DeleteMember: function (memberToDelete, successCbk, errCbk, ctx) {
                SALT.services.ASAServiceCall('DELETE', configuration.apiEndpointBases.MemberService, '/Member/' + memberToDelete.get('MembershipId'), successCbk, function () {}, null, ctx, null, null, false, true);
            },

            LogOff: function (successCbk, errCbk, ctx) {
                SALT.services.ASAServiceCall('POST', '/Account/LogOff', '', '', successCbk, function () {}, '{}', ctx, null, null, false, true);
            },

            // POST AskMe
            AskMe: function (firstName, lastName, subject, emailAddress, yourQuestion, membershipId, successCallback, errorCallback, ctx, cache, async) {

                var askMeRequest = JSON.stringify({
                    MembershipId: membershipId,
                    YourQuestion: yourQuestion,
                    FromEmailAddress: emailAddress,
                    Subject: subject,
                    LastName: lastName,
                    FirstName: firstName
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.AlertServiceEndpoint, '/JustAsk', successCallback, errorCallback, askMeRequest, ctx, null, null, cache, async);

            },
            UpdateMemberOrganizations: function (memberId, organizations, successCallback, errorCallback, ctx) {
                var organizationsJSON = JSON.stringify(organizations);
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService + '/Member/' + memberId, '/OrganizationsAffiliation', successCallback, errorCallback, organizationsJSON, ctx, null, null, false, false);
            },
            GetUserSegment: function (userSegmentCookie, successCallback, errorCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.CustomLanding, '/courses?Endeca_user_segments=' + userSegmentCookie, successCallback, errorCallback, null, ctx, null, null, false, false);
            },
            ManageAccount: function (member, successCallback, errorCallback, ctx) {
                var memberJSON = JSON.stringify(member);
                SALT.services.ASAServiceCall('POST', '/Account/ManageAccount/', null, successCallback, errorCallback, memberJSON, ctx, null, null, false, false);
            },
            // POST content feedback Email Us
            contentFeedbackEmail: function (formObject, successCallback, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.AlertServiceEndpoint, '/ContentFeedbackEmail', successCallback, function () {}, formObject, ctx, null, null, false, true);
            },
            GetMemberCourses: function (successCallback, ctx, fromMoodle) {
                fromMoodle = fromMoodle || false; //default to false if not specified
                var sourceString = '?source=' + fromMoodle;
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/Member/Individual/Courses' + sourceString, successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            SyncCoursesCompletion: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.MemberService, '/Member/Individual/SyncCourses', successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            GetMemberQuestionAnswer: function (memberId, sourceId, successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/GetMemberQuestionAnswer/' + memberId + '/' + sourceId, successCallback, function () {}, '', ctx, null, null, false, true);
            },
            UpsertQuestionAnswer: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/QuestionAnswerResponse', successCallback, function () {}, data, ctx, null, null, false, true);
            },
            GetScholarshipDetail: function (scholarshipId, successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.SearchServiceEndpoint + '/Scholarships/', scholarshipId, successCallback, function () {}, '', ctx, 'application/text', null, false, true);
            },
            GetUnigoAnswers: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', '/Scholarships/', 'answers', successCallback, function () {}, '', ctx, 'application/text', null, false, true);
            },
            GetScholarships: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SearchServiceEndpoint + '/submitChoicesToUnigo', '', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            SchoolLookup: function (oe, schoolname, successCallback, errorCallback, ctx) {
                var schoolInfoStr = JSON.stringify({
                    oeCode: oe,
                    school: schoolname
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SearchServiceEndpoint, '', successCallback, errorCallback, schoolInfoStr, ctx, null, null, true, true);
            },
            SetLocation: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.SearchServiceEndpoint, '/SetLocation', successCallback, function () {}, data, ctx, null, null, false, true);
            },
            PostSurvey: function (surveyID, surveyQuestionID, individualID, response, successCallback, errorCallback, ctx) {
                var surveyInfoStr = JSON.stringify({
                    SurveyId: surveyID,
                    SurveyQuestionId: surveyQuestionID,
                    IndividualId: individualID,
                    Response: response
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SurveyServiceEndpoint, '/Survey', successCallback, errorCallback, surveyInfoStr, ctx, null, null, true, true);
            },
            getTodos: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            upsertTodo: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            deleteTodo: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('DELETE', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            upsertProfileResponse: function (memberID, data, successCallback, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/Member/' + memberID + '/ProfileResponses', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            }
        };
    }
});
