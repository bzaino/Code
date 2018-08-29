var curriculum = curriculum || {};

// pagination vars
var pages   = [];
var pagePass = false;
var animationPass = true;
var expenseGraph = 0;
var goalCalculated = 0;
var pageTransitionDuration = 360;

var currentExpensePagePosition = 0;
var expenseSubPagesCount = 0;
var cashflow;
var cashflowStatus = '';
var returnToReview = false;
var goalPageLoaded = false;

var expensePages   = [];
var totalExpenses = 0;
var totalNonCreditExpenses = 0;
var expensesCutGoal= 0;
var ccTotal = 0;

var save;
var userData = {};
var EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

var cboxElement = '.cbox';
var cboxCheckedSelector = '.checked';

//used for WebTrends logging
var userInfo = {
  state: '00',
  siteMemberId: ''
};

//*********** Begin curriculum global var ****************/
/* most of the work for lesson 1 is done here
/* tracking:
//*******************************************************/
require([
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'jquery.colorbox',
    'jquery.client',
    'jquery.waypoints'
], function ($, SALT, SiteMember) {
  curriculum.global = {
    tracking: {
      //paramters
      preloaded: false,
      startTime: null,
      endTime: null,
      _ready: $.Deferred(),
      //default object
      defaults: {
        name: 'how-does-your-cash-flow'
      },
      init: function () {
        var self = this;

        this.startTime = new Date();

        // Lesson 1 Analytics Subscriptions
        require(['salt/analytics/webtrends'], function(WT) {
          WT.subscribe('salt/analytics/webtrends/maps/lessons.json').always(function() {
            self._ready.resolve();
          });
        });
      },
      //
      trigger: function (eventName, message) {
        var additionalDetails = {};

        if (eventName.search("step0") == -1) {
          // For every call, add the guid and the time
          additionalDetails = _.extend(this.defaults, {
            user: userData.individualId,
            time: new Date(),
            step: {
              number: save.global.step,
              preloaded: this.preloaded
            }
          });

          //added userInfo for usage  in WebTrends Lesson tagging
          additionalDetails = $.extend(true, additionalDetails, userInfo);

          if (message) {
            additionalDetails = $.extend(true, additionalDetails, message);
          }

          // For the last call, extend to contain totalTime
          if (eventName === "lesson:overall:complete") {
            this.endTime = (new Date()).getTime();
            var totalTime = this.endTime - this.startTime.getTime();

            additionalDetails = _.extend(additionalDetails, {
              totalTime: (totalTime / 60000)
            });
          }
          // For the curriculum.global.saveAndExit  bond to header save and exit button
          if (eventName === 'siteMember:signin:complete'){
            this.endTime = (new Date()).getTime();
            var totalTime = this.endTime - this.startTime.getTime();

            additionalDetails = _.extend(additionalDetails, {
                totalTime: (totalTime / 60000)
            });
            //Wait for authentication then save user data
            this._ready.done(function() {
              SALT.subscribe('siteMember:sync', function() {
               curriculum.global.SaveAfterAuthenticate.UpdateUserGuidAndSave();
              });
            });
          }

          this._ready.done(function() {
              SALT.global.WT.publish(eventName, {
              lesson: additionalDetails
            });
          });
        }
      }
    },
    //End tracking

    utils: {
      initSharing: function () {
        $.getScript("//s7.addthis.com/js/300/addthis_widget.js#pubid=xa-50f5e9a91f8c0bfc&async=1", function () {
          if (typeof addthis !== "undefined") {
            var baseUrl = document.location.protocol + "//" + document.location.host,
                lessonLandingUrl = baseUrl + "/content/media/lesson/how-does-your-cash-flow/_/R-101-2702";

            $addthisWrapper = $(".addthis_toolbox");
            $addthisWrapper.attr("addthis:url", lessonLandingUrl);

            addthis.init();
          }
        });
      },

      userAlreadySawStep1: false,
      showLoadDataPrompt: function () {
        if (curriculum.global.utils.userAlreadySawStep1 == false) {
          this.userAlreadySawStep1 = true;
          return (userData.goal && userData.goal.name != null) ? true : false;
          // return false;
        } else {
          return false;
        }
      },

      server: {
        useStub: false,
        // TODO: serverUrl will be webroot relative for now,
        // but if we want a configurable path it needs to be setup in the build
        serverUrl: '/api/LessonsService',
        frequencyTypes: {},
        expenseTypes: {},
        topLevelExpenses: {
          'credit': null,
          'eating': null,
          'entertainment': null,
          'groceries': null,
          'health': null,
          'housing': null,
          'other': null,
          'savings': null,
          'school': null,
          'technology': null,
          'transportation': null,
          'utilities': null
        },
        incomeTypes: {},
        retrievedServerData: null,
        getIndividualId: function (cbf) {
          $.ajax({
            url: '/api/ASAMemberService/GetMember/Individual',
            type: 'GET',
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
              cbf(EMPTY_GUID);
            },
            success: function (msg) {
              if (typeof msg === "object" && msg.IndividualId !== undefined) {
                //If msg.IndividualId is populated, the user is recognized as an ASA User
                cbf(msg.IndividualId);
              } else {
                //If msg.IndividualId isn't populated or can't be found, the user isn't logged in or an error occured
                cbf(EMPTY_GUID);
              }
            }
          });
        },
        getUserIdByIndiviualId: function (individualId, cbf) {
          $.ajax({
            url: this.serverUrl + '/User/IndividualId/' + individualId,
            type: 'GET',
            async: false,
            cache: false,
            timeout: 30000,
            error: function (xhr, ajaxOptions, thrownError) {
              cbf(null);
            },
            success: function (data) {
              cbf(data.UserId, data.IndividualId);
            }
          });
        },
        startOver: function()
        {
          $.ajax({
            url: this.serverUrl + '/User/1',
            type: 'delete',
            async: false
          });
        },
        createUser: function (individualId, cbf) {
          var userData = new Object();
          userData.individualId = individualId;
          var serverData = this.convertToServer(userData);

          var retrievedData;
          $.ajax({
            url: this.serverUrl + '/Lesson1User',
            type: 'POST',
            data: JSON.stringify(serverData),
            async: false,
            cache: false,
            timeout: 30000,
            contentType: 'application/json',
            error: function () {
              //TODO:Implement save error state;
            },
            success: function (data) {
              retrievedData = data;
            }
          });
          this.retrievedServerData = retrievedData;
          this.convertToFrontEnd(userData, this.retrievedServerData);
          cbf(userData.userGuid);

        },
        existingUser: function (userId) {
          var exists;
          $.ajax({
            url: this.serverUrl + '/User/' + userId + '/1',
            type: 'GET',
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
              exists = false;
            },
            success: function (msg) {
              exists = true;
            }
          });
          return exists;
        },
        saveToServer: function (userData) {
          var serverData = this.convertToServer(userData);

          $.ajax({
            url: this.serverUrl + '/Lesson1User',
            type: 'PUT',
            data: JSON.stringify(serverData),
            async: false,
            cache: false,
            timeout: 30000,
            contentType: 'application/json',
            error: function () {
              // TODO: Implement save error state if lesson save fails
              // throw new Error("There was a problem saving");
            },
            success: function (data) {
              curriculum.global.tracking.trigger("lesson:overall:saveToServer", {
                step: {
                  number: save.global.step
                }
              });
            }
          });
        },
        retrieveFromServer: function (userId, userData) {

          var retrievedData;
          $.ajax({
            url: this.serverUrl + '/Lesson1User/',
            type: 'GET',
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
              exists = false;
            },
            success: function (data) {
              //console.log(data);
              retrievedData = data;
            }
          });
          this.retrievedServerData = retrievedData;
          this.convertToFrontEnd(userData, this.retrievedServerData);
        },
        convertToServer: function (userData) {
          var serverData = new Object();
          serverData.UserId = userData.userGuid;
          serverData.IndividualId = userData.individualId;
          this.convertServerUser(userData, serverData);
          this.convertServerGoal(userData, serverData);
          this.convertServerIncomes(userData, serverData);
          this.convertServerExpenses(userData, serverData);
          this.convertServerCreditExpenses(userData, serverData);

          return serverData;
        },
        convertToFrontEnd: function (userData, serverData) {

          if (userData == null) {
            userData = new Object();
          }
          this.loadFrequencyTypes();

          if (typeof serverData !== "undefined") {
            userData.userGuid = serverData.UserId;
            userData.individualId = serverData.IndividualId;
            this.convertFEUser(userData, serverData);
            this.convertFEGoal(userData, serverData);
            this.convertFEIncomes(userData, serverData);
            this.convertFEExpenses(userData, serverData);
            this.createExpenseList(userData);
            this.createNonCreditExpenseList(userData);
          }
        },
        convertServerUser: function (userData, serverData) {
          if (userData.User != null) {
            var user = new Object();
            user.UserId = userData.userGuid;
            user.IndividualId = userData.individualId;
            user.Lesson1Step = userData.User.Lesson1Step;
            serverData.User = user;
          }
        },
        convertFEUser: function (userData, serverData) {
          if (serverData.User != null) {
            var user = new Object();
            user.UserId = serverData.User.UserId;
            user.IndividualId = serverData.User.IndividualId;
            user.Lesson1Step = serverData.User.Lesson1Step;
            userData.User = user;
          }
        },
        convertServerGoal: function (userData, serverData) {
          if (userData.goal != null) {
            var Goal = new Object();
            Goal.GoalId = userData.goal.id;
            Goal.Name = userData.goal.name;

            Goal.Months = userData.goal.time;
            Goal.Value = userData.goal.value;
            Goal.UserId = userData.userGuid;
            serverData.Goal = Goal;
          }
        },
        convertFEGoal: function (userData, serverData) {
          if (serverData.Goal != null) {
            var goal = new Object();
            goal.name = serverData.Goal.Name;
            goal.value = serverData.Goal.Value;
            goal.time = serverData.Goal.Months;
            goal.userId = serverData.Goal.UserId;
            goal.id = serverData.Goal.GoalId;
            userData.goal = goal;
          }
        },
        convertServerIncomes: function (userData, serverData) {
          if (userData.income != null) {
            var incomes = [];
            for (var incomeTypeName in userData.income) {
              var inc = userData.income[incomeTypeName];
              inc.Value = inc.value;
              inc.FrequencyId = this.frequencyTypes[inc.time].FrequencyId;
              inc.IncomeTypeName = incomeTypeName;
              incomes.push(inc);
            }

            serverData.Incomes = incomes;
          }
        },
        convertFEIncomes: function (userData, serverData) {
          if (serverData.Incomes != null && serverData.Incomes.length > 0) {
            var income = new Object();
            for (var i = serverData.Incomes.length - 1; i >= 0; i--) {
              var inc = serverData.Incomes[i];
              inc.value = inc.Value;
              inc.time = this.frequencyTypes[inc.FrequencyId].Name;
              income[inc.IncomeTypeName] = inc;
            }
            userData.income = income;
          }
        },
        isComplexExpense: function (expense) {
          if (expense.Complex) {
            return true;
          }

          if (expense.Name == 'credit' || expense.Name == 'entertainment' || expense.Name == 'savings' || expense.Name == 'groceries' || expense.Name == 'eating' || expense.Name == 'technology') {
            return false;
          } else {
            return true;
          }
        },
        addCreditExpense: function (userData, exp, parentName) {
          if (typeof (userData.creditExpenses) == 'undefined' || userData.creditExpenses == null) {
            userData.creditExpenses = [];
          }

          var name = exp.Name;
          if (parentName) {
            if (parentName == 'transportation') {
              name = name.split('_')[0];
            }
            name = parentName + '-' + name;
          }

          userData.creditExpenses.push(name);
        },
        convertServerCreditExpenses: function (userData, serverData) {
          if (userData.creditExpenses != null) {
            var creditExpenses = [];
            $.extend(true, creditExpenses, userData.creditExpenses);
            $.each(creditExpenses, function (k, v) { creditExpenses[k] = v.replace('transportation-', ''); });

            $.each(serverData.Expenses, function (k, v) {
              if( $.inArray(v.Name, creditExpenses) !== -1 ) {
                v.CreditExpense = true;
              }
            });

          }
        },
        convertServerExpenses: function (userData, serverData) {
          if (userData.expenses != null) {
            var expenses = [];
            for (var topLevelExpense in userData.expenses) {
              var tle = userData.expenses[topLevelExpense];
              tle.Name = tle.Name || topLevelExpense;
              if (!this.isComplexExpense(tle)) {
                tle.Value = tle.value;
                tle.DisplayName = tle.displayName;

                tle.FrequencyId = this.frequencyTypes[tle.time].FrequencyId;
                expenses.push(tle);
              } else {
                this.processComplexExpense(tle, expenses);
              }
            }
            serverData.Expenses = expenses;
          }
        },
        processComplexExpense: function (tle, expenses) {
          var parentObject = tle;
          if (this.isCostExpense(tle.Name)) {
            parentObject = tle.cost;
          }
          var parentExpense = {};

          parentExpense.ExpenseId = tle.ExpenseId;
          parentExpense.Name = tle.Name;
          if (parentExpense.Name == 'housing') {
            if (tle.selected == 'rent') {
              parentExpense.Name = parentExpense.Name + '-rent';
            } else {
              parentExpense.Name = parentExpense.Name + '-own';
            }
          }

          parentExpense.DisplayName = tle.displayName;
          parentExpense.ExpenseTypeId = tle.ExpenseTypeId;
          parentExpense.Complex = true;
          parentExpense.Recurring = true;
          parentExpense.UserId = tle.UserId;
          parentExpense.ParentExpenseId = tle.ParentExpenseId;
          parentExpense.ParentName = tle.ParentName;
          // if(!tle.Complex) {
          //   parentExpense.FrequencyId = this.frequencyTypes[tle.time].FrequencyId;
          //   parentExpense.Value = tle.value;
          // }
          expenses.push(parentExpense);

          for (exp in parentObject) {
            var childExpense = parentObject[exp];
            if (this.isExpense(childExpense)) {
              if (parentExpense.ExpenseId == null || parentExpense.ExpenseId == '') {
                childExpense.ParentName = parentExpense.Name;
              } else {
                childExpense.ParentExpenseId = parentExpense.ExpenseId;
              }

              if (childExpense.Complex) {
                childExpense.Name = childExpense.Name || exp;
                childExpense.UserId = tle.UserId;
                this.processComplexExpense(childExpense, expenses);
              } else {
                // console.log('childExpense');
                // console.log(parentObject);
                // console.log(childExpense);
                childExpense.Value = childExpense.value;
                childExpense.FrequencyId = this.frequencyTypes[childExpense.time].FrequencyId;
                childExpense.Name = childExpense.Name || exp;
                expenses.push(childExpense);
              }
            }
          }
        },
        convertFEExpenses: function (userData, serverData) {
          if (serverData.Expenses != null && serverData.Expenses.length > 0) {
            var expenseList = new Object();
            var expenses = $.extend(true, {}, this.topLevelExpenses);
            var childExpenses = [];
            var toplevelexpenseMap = {};
            for (var i = serverData.Expenses.length - 1; i >= 0; i--) {

              if (serverData.Expenses[i].Name == 'housing-rent') {//Housing expense has extra data
                serverData.Expenses[i].Name = 'housing';
                serverData.Expenses[i].selected = 'rent';
              } else if (serverData.Expenses[i].Name == 'housing-own') {
                serverData.Expenses[i].Name = 'housing';
                serverData.Expenses[i].selected = 'own';
              }
              var exp = serverData.Expenses[i];

              if (exp.CreditExpense) {
                var parentName = undefined;
                if (exp.ParentExpenseId != 0) {
                  var parent = _.find(serverData.Expenses, function (expense) { return expense.ExpenseId == exp.ParentExpenseId; });
                  if (parent.ParentExpenseId != 0) {
                    parent = _.find(serverData.Expenses, function (expense) { return expense.ExpenseId == parent.ParentExpenseId; });
                  }
                  parentName = parent.Name;
                  if (parentName == 'housing-rent' || parentName == 'housing-own') {
                    parentName = 'housing';
                  }
                }
                this.addCreditExpense(userData, exp, parentName);
              }

              if (this.topLevelExpenses.hasOwnProperty(exp.Name) && exp.ParentExpenseId == 0) {
                //Add Expenses to map  of exisiting top level expenses for a user for reference when
                //associating child expenses
                this.createTopLevelExpense(toplevelexpenseMap, expenses, exp);
              } else {
                if (exp.Complex) {
                  toplevelexpenseMap[exp.ExpenseId] = exp;
                  exp.displayName = exp.DisplayName;
                  exp.value = exp.Value;
                }
                childExpenses.push(exp);
              }
            }

            for (var i = childExpenses.length - 1; i >= 0; i--) {
              var exp = childExpenses[i];
              exp.value = exp.Value;
              exp.displayName = exp.DisplayName;

              if (!exp.Complex) {
                exp.time = this.frequencyTypes[exp.FrequencyId].Name;
              }
              if (this.isCostExpense(toplevelexpenseMap[exp.ParentExpenseId].Name)) {
                expenses[toplevelexpenseMap[exp.ParentExpenseId].Name]['cost'][exp.Name] = exp;
              } else {
                if (toplevelexpenseMap[exp.ParentExpenseId].Name == "transportation") {
                  var name = exp.Name.split('_')[0];
                  toplevelexpenseMap[exp.ParentExpenseId][name] = exp;
                } else {
                  toplevelexpenseMap[exp.ParentExpenseId][exp.Name] = exp;
                }
              }
            }

            for (topLevelExpense in expenses) {
              if (expenses[topLevelExpense] == null) {//Remove empty top level expenses
                delete expenses[topLevelExpense];
              }
            }

            userData.expenses = expenses;
          }
        },
        totalExpenseValue: function (expense, excludeCreditExpenses) {
          var val = 0;
          if (expense.Complex) {
            var parent = expense;
            if (this.isCostExpense(expense.Name)) {
              parent = expense.cost;
            }
            for (key in parent) {
              if (this.isExpense(parent[key]) && (!parent[key].CreditExpense || !excludeCreditExpenses)) {
                val += this.totalExpenseValue(parent[key], excludeCreditExpenses);
              }
            }
          } else if (!expense.CreditExpense || !excludeCreditExpenses) {
            val = curriculum.global.utils.determineRate.init(expense.value, expense.time);
          }
          return val;
        },
        createExpenseList: function (userData) {
          if (userData.expenses != null) {
            var expenseList = new Object();
            for (topLevelExpense in userData.expenses) {
              var val = this.totalExpenseValue(userData.expenses[topLevelExpense], false);
              expenseList[topLevelExpense] = val;

              userData.expenseList = expenseList;
            }
          }
        },
        createNonCreditExpenseList: function (userData) {
          if (userData.expenses != null) {
            var nonCreditExpenseList = new Object();
            for (topLevelExpense in userData.expenses) {
              var val = this.totalExpenseValue(userData.expenses[topLevelExpense], true);
              nonCreditExpenseList[topLevelExpense] = val;

              userData.nonCreditExpenseList = nonCreditExpenseList;
            }
          }
        },
        createTopLevelExpense: function (toplevelexpenseMap, expenses, expense) {
          toplevelexpenseMap[expense.ExpenseId] = expense;
          expense.displayName = expense.DisplayName;
          expense.value = expense.Value;

          if (!expense.Complex) {
            expense.time = this.frequencyTypes[expense.FrequencyId].Name;
          }
          if (this.isCostExpense(expense.Name)) {
            expense.cost = {};
          }
          expenses[expense.Name] = expense;
        },
        isExpense: function (expense) {
          if (expense != null && (expense.hasOwnProperty('displayName') || expense.hasOwnProperty('value'))) {
            return true;
          }

          return false;
        },
        isCostExpense: function (expenseName) {
          if (expenseName == 'housing' || expenseName == 'housing-rent' || expenseName == 'housing-own' || expenseName == 'school' || expenseName == 'utilities') {
            return true;
          }
          return false;
        },
        loadFrequencyTypes: function (refreshData) {
          if ($.isEmptyObject(this.frequencyTypes)) {
            var retrievedData;

            $.ajax({
              url: this.getFrequencyUrl(),
              type: 'GET',
              async: false,
              cache: false,
              timeout: 30000,
              error: function () {
                exists = false;
              },
              success: function (data) {
                retrievedData = data;
              }
            });

            for (var i = retrievedData.length - 1; i >= 0; i--) {
              this.frequencyTypes[retrievedData[i].Name] = retrievedData[i];
              this.frequencyTypes[retrievedData[i].FrequencyId] = retrievedData[i];
            }
          }

          return this.frequencyTypes;
        },
        getFrequencyUrl: function () {
          if (this.useStub) {
            return this.serverUrl + '/json/frequencyStub.json';
          }

          return this.serverUrl + '/Frequency';
        },
        loadExpenseTypes: function () {
        },
        loadIncomeTypes: function () {
        }
      },
      setCookie: function (name, value, days) {

        if (name === undefined || value === undefined) {
          return;
        }

        var expire = new Date();
        // if days is not set, cookie expires at end of session
        if (days !== undefined && days !== null) {
          expire.setTime(expire.getTime() + (1000 * 60 * 60 * 24 * days)); // ms * sec * min * hours * days
        }

        document.cookie =
            name + "=" + escape(value)
            + ((days !== undefined) ? "; expires=" + expire.toGMTString() : "")
            + "; path=/";
      },
      getCookie: function (name) {
        var cookies = document.cookie;
        if (cookies.length > 0) {
          var cookieIndex = cookies.indexOf(name + "=");
          if (cookieIndex != -1) {
            var valueStart = cookieIndex + name.length + 1;
            var valueEnd = cookies.indexOf(";", valueStart);
            if (valueEnd == -1) {
              valueEnd = cookies.length;
            }
            return unescape(cookies.substring(valueStart, valueEnd));
          }
        }
        return null;
      },
      init: function () {
        var $html = $("html");

        //determine if using windows
        userOS = $.client.os;
        if (userOS === 'Windows') {
          $html.addClass("windows");
        }

        if ($.browser.webkit === true) {
          $html.addClass("webkit");
        }

        //load the 1st page
        curriculum.global.utils.paginate.init();
        curriculum.global.viewport.init();
        curriculum.global.social.init();
        curriculum.global.saveAndExit.init();

        if ($.browser.msie === true) {
          cboxElement = 'input[type=checkbox]';
          cboxCheckedSelector = ':checked';
        }

        // curriculum.global.common.init();
        setTimeout(curriculum.global.common.init, 600);
      },
      determineRate: {
        init: function (value, time) {
          //takes a value and returns a calculated value
          //based on the rate selected

          newValue = value;
          switch (time) {
            case 'week':
              //divide by week
              newValue = value * 4;
              break;
            case 'month':
              //don't divide
              newValue = value;
              break;
            case 'semester':
              // divide by
              newValue = value / 4;
              break;
            case 'year':
              //divide by 12
              newValue = value / 12;
              break;
          }

          return newValue;
        }
      }, //END determineRate
      paginate: {
        forwardNavLocked: true,
        init: function () {
          var curriculumUtils = curriculum.global.utils;

          curriculumUtils.paginate.currentPagePosition = 0;

          //pull in navigation from .JSON file
          //set those values to global arrays
          $.getJSON('js/json/lesson1.json', function (data, textStatus) {
            pages = data.pages;
            if (textStatus === 'success') {
              curriculumUtils.paginate.nextURL = pages[0].path;
              curriculumUtils.paginate.navigate();
              curriculumUtils.paginate.initNavigation();
              setTimeout(function () {
                $("#header-center").addClass("visible");
              }, 400);
            }
          });

          //handle forward and back buttons
          $('#footer .right-button').click(function (e) {
            e.preventDefault();

            // TODO: decouple these from the next button
            var $totalVal = $('#total .value');
            $totalVal.removeClass('positive');
            $totalVal.removeClass('negative');

            if (pages[curriculum.global.utils.paginate.currentPagePosition].lessonComplete == true){
              curriculum.global.tracking.trigger("lesson:overall:complete", {
                step: {
                  number: save.global.step
                }
              });
            };

            if (pages[curriculum.global.utils.paginate.currentPagePosition].step === 5) {
                SALT.trigger('content:todo:completed');
            }

            if (curriculumUtils.paginate.currentPagePosition < pages.length - 1) {
              if (save.global.step != null) {
                if (pagePass === true) {
                  $(".left-button").removeClass("invisible");
                }
              }

              curriculumUtils.paginate.next();
            } else {
              userData.User.Lesson1Step = pages[curriculum.global.utils.paginate.currentPagePosition].step;
              save.global.utils.saveData();

              if( userData.individualId == EMPTY_GUID ) {
                curriculum.global.utils.showLoginPrompt($(this).attr("href"));
              } else {
                curriculum.global.utils.linkTo($(this).attr("href"));
              }
            }
          });

          $('#footer .left-button').click(function (e) {
            e.preventDefault();

            if (curriculumUtils.paginate.currentPagePosition > 0) {
              //clear out any possible error
              $('#footer .error-msg').fadeOut();

              //fix this - this is lesson specific
              var $totalVal = $('#total .value');
              $totalVal.removeClass('positive');
              $totalVal.removeClass('negative');

              curriculumUtils.paginate.prev();
            }
          });
        }, // END paginate.init()
        initNavigation: function () {
          var navOptions = '';
          var i = 1;
          //loop through the elements in the pages and insert them into the DOM
          $(pages).each(function (key, value) {
            if (pages[key].main) {
              navOptions += '<li data-path="' + pages[key].path + '"><a href="#"><span class="number">' + i + '</span><span>' + pages[key].title + '</span></a></li>';
              i++;
            }
          });
          $('#header-dropdown ul').append(navOptions);


          //.step2 (number of pages)
          $('#header-center .step2').text($('#header-dropdown ul li').length);

          // DROPDOWN CODE
          // update dropdown
          var dropdownItems = $('#header-dropdown ul li');

          // if dropdownItems exists, add 'current' class to appropriate list-item
          if (dropdownItems.length > 0) {
            // remove dynamically added .indicators
            //$('.indicator').remove();
            // set current dropdown item position
            dropdownItems.eq(curriculum.global.utils.paginate.currentPagePosition).addClass('current').append('<span class="indicator">You&rsquo;re here</span>');
          } else {
            //console.log('dropdownItems does not exist');
          }

          // dropdown click states
          $('#header-center').mouseover(function (e) { // Empty block.
            e.preventDefault();
            $(this).toggleClass('active');
            $('#header-dropdown').show();
          });

          $('#header-dropdown li a').live('click', function (e) {
            e.preventDefault();
          });

          //handle when a new section is clicked.
          $('#header-dropdown li.ready').live('click', function (e) {
            if (!$(this).hasClass('current')) {
              //handle current class manipulation
              $('#header-dropdown li.current').removeClass('current');
              $(this).addClass('current');

              //handle indicator manipulation
              $('#header-dropdown li .indicator').remove();
              $(this).append('<span class="indicator">You&rsquo;re here</span>');


              //move to new page
              var selectedPath = $(this).attr('data-path');


              //reset the currentPagePosition variable based on the new selected path
              for (i = 0; i < pages.length; i++) {
                if (pages[i].path === selectedPath) {
                  curriculum.global.utils.paginate.currentPagePosition = i;
                }
              }

              $("#header-dropdown").hide('fast').removeClass('hover active');

              //point page over to new selection
              $("#content-container .content").fadeOut(pageTransitionDuration, function () {
                curriculum.global.utils.paginate.navigateSpecific(selectedPath);
              });
            }

          });


          // dropdown hover states
          $('#header-center').hover(function () {
            $(this).addClass('hover');
          }, function () {
            $("#header-dropdown").hide();
            $("#header-center").removeClass('active hover');
          });

        }, // END initNavigation()

        updateNavigation: function () { // this function keeps the global step navigation up to date with whatever page change was made.
          //this moves the indicator element around based on a page change
          var thisPageNumber = curriculum.global.utils.paginate.currentPagePosition;
          var thisPageName = pages[thisPageNumber].path;

          $(".left-button").removeClass("invisible");

          // enabled lesson steps the user has already seen
          var dropdownItems = $('#header-dropdown li');
          dropdownItems.removeClass('ready');
          // if dropdownItems exists and forward navigation isn't locked (before data load), add 'ready' class to appropriate list-item
          if (!this.forwardNavLocked && dropdownItems.length > 0) {
            for (var x = 0; x < userData.User.Lesson1Step; x++) {
              dropdownItems.eq(x).addClass('ready').addClass('completed');
            }
            dropdownItems.eq(userData.User.Lesson1Step).addClass('ready');
          }

          $('#header-dropdown li.current').removeClass('current');
          //handle indicator manipulation
          $('#header-dropdown li .indicator').remove();

          //determine whether the page is a parent or child page
          if (pages[thisPageNumber].parent) {
            var parentPageName = pages[thisPageNumber].parent + '.html';
            $('#header-dropdown li[data-path="' + parentPageName + '"]').addClass('current').append('<span class="indicator">You&rsquo;re here</span>');
          } else {
            $('#header-dropdown li[data-path="' + thisPageName + '"]').addClass('current').append('<span class="indicator">You&rsquo;re here</span>');

          }
        }, //END updateNavigation()
        loadJS: function () {
          //loop through the js node of this selected page



          $.each(pages[curriculum.global.utils.paginate.currentPagePosition].js, function (key, value) {
            var jsFile = pages[curriculum.global.utils.paginate.currentPagePosition].js[key];
            var fileref = document.createElement('script');
            //remove existing ilfe
            $('script[src="' + jsFile + '"]').remove();

            if ($.browser.msie) {
              jsFile += '?' + Math.round(new Date().getTime() / 1000);
            }

            LazyLoad.js(jsFile);

          });


        }, //END loadJS
        navigate: function () {

          $.ajax(curriculum.global.utils.paginate.nextURL, {
            cache: false,
            success: function (response, params) {
              pagePass = false;
              var html = response;
              curriculum.global.utils.paginate.loadJS();
              $(window).unbind('resize');

              $("#content-container .content").html(html);
              curriculum.global.utils.paginate.animateProgress();
              curriculum.global.viewport.getNewViewport();

              //make sure that this new section can be navigated to via main nav now that it has been visited for the 1st time
              $('#header-dropdown li[data-path="' + pages[curriculum.global.utils.paginate.currentPagePosition].path + '"]').addClass('ready');
              $("#content-container .content").show().css({ opacity: 0 });

              // re-init global common scripts
              setTimeout(curriculum.global.common.init, 600);

              //update the nav item
              curriculum.global.utils.paginate.updateNavigation();
            }
          });

          $('#total').fadeIn();
          $('#up-next').fadeIn();
          $('#restore').fadeOut();
        },

        //This function updates the NextURL and pagePosition(Current and CurrentExpense) variables
        updateNewPageInfo: function (goBack) {
          if (goBack) {
            curriculum.global.utils.paginate.currentPagePosition--;
            curriculum.global.utils.paginate.calculateNextURL();
          } else {
            curriculum.global.utils.paginate.currentPagePosition++;
            curriculum.global.utils.paginate.calculateNextURL();
          }
        },

        calculateNextURL: function () {
          if (curriculum.global.utils.paginate.skipToURL === true) {
            curriculum.global.utils.paginate.nextURL = curriculum.global.utils.paginate.skipToURL;
            curriculum.global.utils.paginate.skipToURL = null;
          } else {
            curriculum.global.utils.paginate.nextURL = pages[curriculum.global.utils.paginate.currentPagePosition].path;
          }
        }, // END calculateNextURL()

        next: function () {
          /**
          fade out current page
          begin loading content once fadeOut is complete
          then load in the next page
          */
          if (pagePass) {
            if (animationPass) {
              ///header nav
              //mark current page as completed in nav before page switch is made
              $('#header-dropdown li[data-path="' + pages[curriculum.global.utils.paginate.currentPagePosition].path + '"]').addClass('completed');

              if (pages[curriculum.global.utils.paginate.currentPagePosition + 1] &&
                  pages[curriculum.global.utils.paginate.currentPagePosition + 1].main) {
                var nextStep = pages[curriculum.global.utils.paginate.currentPagePosition + 1].step;
                var currentStep = nextStep - 1;
                if (currentStep > userData.User.Lesson1Step) userData.User.Lesson1Step = currentStep;
              }

              save.global.utils.saveData();
              // retrieve userData from server so that Ids are updated
              curriculum.global.utils.server.retrieveFromServer(userData.userGuid, userData);

              curriculum.global.utils.paginate.updateNewPageInfo(false);
              animationPass = false;

              $("#content-container .content").fadeOut(pageTransitionDuration, function () {
                curriculum.global.utils.paginate.navigate();
                //$("#content-container .content").delay(100).fadeIn('slow');
              });
            }
          } else {
            save.global.utils.errors();
          }

        },
        prev: function () {
          //fade out current page
          //begin loading content once fadeOut is complete
          //then load in the next page
          //save.global.utils.saveData();
          if (animationPass) {

            curriculum.global.utils.paginate.updateNewPageInfo(true);
            animationPass = false;

            $("#content-container .content").fadeOut(pageTransitionDuration, function () {
              curriculum.global.utils.paginate.navigate();
              //$("#content-container .content").delay(100).fadeIn('slow');

            });
          }
        },

        updateContext: function (step1, chapter, valueMessage, upNext, nextButton) {
          //update all the text respectively using the data that gets sent over from the local JS files
          //header
          $('#header-center .step1').text("Step " + step1);

          //chapter
          $('#header-center .chapter').text(pages[curriculum.global.utils.paginate.currentPagePosition].title);

          //footer
          $('#total .value-message').text(valueMessage);
          $('#up-next .second').text(upNext);

          if (!nextButton) {
            nextButton = "keep going";
          }

          $('#footer .right .right-button').text(nextButton);

        }, // END updateContext()

        animateProgress: function () {

          if (curriculum.global.utils.paginate.currentPagePosition <= 2) {
            var progress = (curriculum.global.utils.paginate.currentPagePosition + 1) * 100 / 15;
          }
          else {
            var progress = (curriculum.global.utils.paginate.currentPagePosition + 1) * 100 / pages.length;
          }

          $('#header-progress .progress').animate({
            width: progress + '%'
          });

        }, // END animateProgress()

        navigateSpecific: function (nextURL) {
          $.ajax(nextURL, {
            cache: false,
            success: function (response) {
              var html = response;
              curriculum.global.utils.paginate.loadJS();
              $("#content-container .content").html(html);
              curriculum.global.utils.paginate.animateProgress();
              curriculum.global.viewport.getNewViewport();

              $("#content-container .content").show().css({ opacity: 0 });

              // re-init global common scripts
              setTimeout(curriculum.global.common.init, 600);

              //update the nav item
              curriculum.global.utils.paginate.updateNavigation();
            }
          });

          $('#total').fadeIn();
          $('#up-next').fadeIn();
          $('#restore').fadeOut();
        }, // END navigateSpecific()
        navigateLeftColumn: function (who) {
          $.ajax(who, {
            cache: false,
            success: function (response) {
              var html = response;
              $("#content-left .content").html(html);
            }
          });
        } //END navigateLeftColumn
      }, // END: paginate
      animateGraph: {
        refresh: function () {
          var income = curriculum.global.utils.totalIncome.value();
          var expenses = curriculum.global.utils.totalExpenses.value();
          var cashflow = income - expenses;

          $('.total-income-value').text(income).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
          $('.total-expense-value').text(expenses).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

          if (cashflow < 0) {
            cashflowStatus = 'negative';
          } else if (cashflow > 0) {
            if (cashflow > goalCalculated) { //TODO
              cashflowStatus = 'extra-cash';
            } else {
              cashflowStatus = 'reached-goal';
            }
          }

          var incomeGraph = Math.round(income / 20);
          var expenseGraph = Math.round(expenses / 20);

          if (incomeGraph >= 100 || expenseGraph >= 100) {
            if (incomeGraph > expenseGraph) {
              expenseGraph = Math.round(expenseGraph * (100 / incomeGraph));
              incomeGraph = 100;
            } else if (incomeGraph < expenseGraph) {
              incomeGraph = Math.round(incomeGraph * (100 / expenseGraph));
              expenseGraph = 100;
            } else {
              incomeGraph = 100;
              expenseGraph = 100;
            }
          }
          if (incomeGraph <= 4) {
            incomeGraph = 4;
          }
          if (expenseGraph <= 4) {
            expenseGraph = 4;
          }

          // this check for modified height is to prevent the slight flicker in height when triggering animate with unchanged values
          var curentIncomeHeight = Math.round(($('#vis #bars #income').height() / $('#vis #bars #income').parent().height()) * 100);
          var curentExpenseHeight = Math.round(($('#vis #bars #expenses').height() / $('#vis #bars #expenses').parent().height()) * 100);

          if (curentIncomeHeight != incomeGraph) {
            //animate
            $('#vis #bars #income').stop(true, false).animate({
              height: incomeGraph + '%'
            }, 1000, function () {

            });
          }

          if (curentExpenseHeight != expenseGraph) {
            //animate
            $('#vis #bars #expenses').stop(true, false).animate({
              height: expenseGraph + '%'
            }, 1000, function () {
            });
          }
        }
      }, //END animateIncome
      isEmpty: function (value) {
        if (!$.trim(value + '').length) {
          return true;
        } else {
          return false;
        }
      }, // END isEmpty()
      cleanInput: {
        init: function (value) {
          //pulls out dollar signs and commas from value and returns it as just numbers
          cleanInput = Number(value.replace(/[^0-9\.-]+/g, "")); // Unescaped '-'.
          return Math.round(cleanInput);
        },
        freeForm: function (value){
          cleanInput = value.replace(/[^A-Za-z0-9 %]+/g, "");
          return cleanInput;
        },
        freeFormNoPerctg: function (value){
          cleanInput = value.replace(/[^A-Za-z0-9]+/g, "");
          return cleanInput;
        }
      }, // END cleanInput
      fixConflictingID: function (element) {
        $(element).each(function (key, value) {
          $(element).find('.data-repeat').each(function () {
            currentID = $(this).attr('id');
            $(this).attr('id', currentID + '-' + key);
          });
          $(element).find('.label-repeat').each(function () {
            currentID = $(this).attr('for');
            $(this).attr('for', currentID + '-' + key);
          });
        });
      }, //END fixConflictingID
      grepArray: function (array, value) {
        array = $.grep(array, function (v) {
          return v !== value;
        });
      },
      linkTo: function (href) {
        // NOTE: instead of window.location creating a blank form and submitting to preserve back button
        var $form = $('<form method="GET" action="' + href + '" />');
        $("body").append($form);
        $form.submit();
      },
      showLoginPrompt: function (redirecthref) {
        var CBOX_CLOSED = "cbox_closed",
        $saveProxy = $("#loginSubmit");

        $saveProxy.data('redirect', redirecthref);
        $("#loginSubmit").trigger("click");

        $(document).unbind(CBOX_CLOSED).bind(CBOX_CLOSED, function() {
          setTimeout(function() {
            curriculum.global.utils.linkTo(redirecthref);
          }, 4500);
        });

        $("#forgotPasswordSubmit").click(function() {
          $(document).unbind(CBOX_CLOSED);
        });
      },
      totalIncome: {
        value: function () {
          var totalIncome = 0;
          for (var key in userData.income) {
            if (userData.income.hasOwnProperty(key)) {
              var income = userData.income[key];
              totalIncome += curriculum.global.utils.determineRate.init(income.value, income.time);
            }
          }
          return totalIncome;
        }
      },
      totalExpenses: {
        value: function () {
          var totalExpenses = 0;
          if (userData.expenseList !== undefined) {
            $.each(userData.expenseList, function (k, v) {
              totalExpenses += v;
            });
          }

          return totalExpenses;
        }
      },
      totalNonCreditExpenses: {
        value: function () {
          var totalNonCreditExpenses = 0;
          if (userData.nonCreditExpenseList !== undefined) {
            $.each(userData.nonCreditExpenseList, function (k, v) {
              totalNonCreditExpenses += v;
            });
          }

          return totalNonCreditExpenses;
        }
      }
    }, // END: utils
    viewport: {
      init: function () {
        curriculum.global.viewport.windowResize();
        curriculum.global.viewport.scrollShadows();
      },
      getNewViewport: function () {
        curriculum.global.viewport.contentHeight = $('#content-container .content').outerHeight();
        curriculum.global.viewport.viewableHeight = $('html').outerHeight() - $('#header').outerHeight() - $('#footer').outerHeight();
        curriculum.global.viewport.maxScroll = curriculum.global.viewport.contentHeight - curriculum.global.viewport.viewableHeight;


        if (curriculum.global.viewport.contentHeight > curriculum.global.viewport.viewableHeight) {
          // scroll = true;
          $('#footer .top').addClass('shadow');
        } else {
          // scroll = false;
          $('#footer .top').removeClass('shadow');
        }



      },
      windowResize: function () {
        /**
        This runs on resize and is used to determines if the
        height of the content is greater than the viewable area
        if so it will set 'scroll' to true and
        */
        $(window).resize(function () {
          curriculum.global.viewport.getNewViewport();
          if (curriculum.global.viewport.contentHeight > curriculum.global.viewport.viewableHeight) {
            // scroll = true;
            $('#footer .top').addClass('shadow');
          } else {
            // scroll = false;
            $('#footer .top').removeClass('shadow');
          }
        });
      },
      scrollShadows: function () {
        /**
        SCROLLING
        //add shadow to the header when content scrolls behind it
        */

        $('#content-container .content').waypoint(function (direction) {
          if (direction === 'down') {
            $('#header .bottom').addClass('shadow');
          } else {
            $('#header .bottom').removeClass('shadow');
          }
        }, {
          offset: $('#header').height()
        });

        //determine if we should use that shadow on load
        if (curriculum.global.viewport.contentHeight > curriculum.global.viewport.viewableHeight) {
          // scroll = true;
          $('#footer .top').addClass('shadow');
        } else {
          // scroll = false;
          $('#footer .top').removeClass('shadow');
        }

        //check to see if we need to add shadow to the bottom footer when page starts to scroll
        $(window).scroll(function () {
          if (scroll) {
            if ($('html').scrollTop() >= curriculum.global.viewport.maxScroll) {
              $('#footer .top').removeClass('shadow');
            } else {
              $('#footer .top').addClass('shadow');
            }
          }
        });
        /**
        END SCROLLING
        */
      }, // END: scrollShadows
      topShadow: function () {
        $('#content-container .content').waypoint(function (direction) {
          if (direction === 'down') {
            $('.local-header .bottom').addClass('active');
          } else {
            $('.local-header .bottom').removeClass('active');
          }
        }, {
          offset: $('#header').height()
        });
      }, //END topShadow
      animateViewport: {
        normal: function () {
          var viewportSpeed = 850;
          //MAIN
          $('#content-container').animate({
            width: '60.6%',
            left: '5.3%'
          }, viewportSpeed, $.noop).removeClass("wide").removeClass("inverse").addClass("normal");

          $("#footer").animate({ width: '55.5%' }, viewportSpeed);
          $("#header .bottom").animate({ width: '65.9%' }, viewportSpeed);
          $("#content-container .content").stop(true, true).animate({ width: '91.6%', opacity: 1 }, viewportSpeed, function () { animationPass = true });
          //right
          $('#content-visual').animate({
            opacity: 1,
            width: '34%',
            minWidth: '34%'
          }, viewportSpeed, function () {
            // Animation complete.
          });

          //left
          $('#content-left').animate({
            minWidth: '34%',
            width: '34%',
            left: '-34%'
          }, viewportSpeed, function () {


          });
        },
        wide: function () {
          var viewportSpeed = 850;

          $('#content-container').animate({
            width: '89.5%',
            left: '5.3%'
          }, viewportSpeed, $.noop).removeClass("normal").removeClass("inverse").addClass("wide");

          $("#footer").animate({ width: '89%' }, viewportSpeed);
          $("#header .bottom").animate({ width: '100%' }, viewportSpeed);
          $("#content-container .content").stop(true, true).animate({ width: '100%', opacity: 1 }, viewportSpeed, function () { animationPass = true });
          //right
          $('#content-visual').animate({
            minWidth: '0%',
            width: '0'
          }, viewportSpeed, function () {
            // Animation complete.
          });
          //left
          $('#content-left').animate({
            minWidth: '34%',
            width: '34%',
            left: '-34%'
          }, viewportSpeed, function () {
            // Animation complete.

          });
        },
        inverse: function () {
          var viewportSpeed = 850;
          $('#content-container').animate({
            width: '60.6%',
            left: '36.5%'
          }, viewportSpeed, $.noop).removeClass("normal").removeClass("wide").addClass("inverse");

          $("#header .bottom").animate({ width: '0%' }, viewportSpeed);
          $("#footer").animate({ width: '60%' }, viewportSpeed);
          $("#content-container .content").stop(true, true).animate({ width: '91.6%', opacity: 1 }, viewportSpeed, function () { animationPass = true });

          //right
          $('#content-visual').animate({
            minWidth: '0%',
            width: '0'
          }, viewportSpeed, function () {
            // Animation complete.
          });

          //left
          $('#content-left').animate({
            left: '0%',
            minWidth: '34%',
            width: '34%',
            opacity: 1
          }, viewportSpeed, function () {

          });
        }
      } // END animateViewport
    }, // END viewPort
    social: {
      init: function () {
        // curriculum.global.social.loadSocial();
      },
      loadSocial: function () {
        ASA.fb.fbEnsureInit(FB.getLoginStatus(function (response) {
          if (response.status === 'connected' && response.authResponse !== undefined) {
            ASA.fb.getProfileImageCurriculum();
          }
        }));
      }
    }, //END social
    saveAndExit: {
            init: function () {
            $('.save-and-exit').click(function (e) {
                e.preventDefault();
                var sEventName = 'lesson:overall:saveAndExit';

                //trigger saveAndExt event, then continue below
                curriculum.global.tracking.trigger(sEventName);

                if (pages[curriculum.global.utils.paginate.currentPagePosition].step === 5) {
                    SALT.trigger('content:todo:completed');
                }

                // Temporarily killing next because we only want to validate
                // and not actually go to the next step. However, we want to
                // turn this back on if the user decides to continue after
                // a failed validation.
                //var tmp = curriculum.global.utils.paginate.next;
                //curriculum.global.utils.paginate.next = $.noop;

                // save.global.utils.errors();
                // if (pagePass) {
                //   save.global.utils.saveData();
                // }

                // curriculum.global.utils.paginate.next = tmp;

                // See if user is logged in vs. anonymous
                if( userData.individualId == EMPTY_GUID ) {
                    curriculum.global.utils.showLoginPrompt($(this).attr("href"));
                } else {
                    if (curriculum.global.utils.getCookie('IndividualId') !== null) {
                        // uses href to submit form and navigate to home page
                        //for first page make sure data is valid, then save
                        if (pages[curriculum.global.utils.paginate.currentPagePosition].step === 1) {
                            if (save.global.utils.saveAndExitStep1Validation()) {
                                save.global.utils.saveData();
                            }
                        } else {
                            save.global.utils.saveData();
                        }
                        curriculum.global.utils.linkTo($(this).attr("href"));
                    } else {
                        curriculum.global.utils.showLoginPrompt($(this).attr("href"));
                    }
                }
            });
        }
    }, // END saveAndExit UpdateUserGuidAndSave
    SaveAfterAuthenticate: {
        UpdateUserGuidAndSave:function () {
            require(['salt/models/SiteMember'], function(SiteMember) {
                //SYNC hasn't fired yet so use the raw object

                SiteMember.done(function(siteMember) {
                    if (siteMember.IsAuthenticated === 'true') {
                        //If response.IndividualId is populated, the user is recognized as an ASA User
                        if (SiteMember.get('IndividualId') != EMPTY_GUID) {
                            curriculum.global.utils.server.getUserIdByIndiviualId(SiteMember.get('IndividualId'), function(userId, IndividualId) {
                                SALT.trigger('content:todo:inProgress');
                                if (userId != null) {
                                    // Update to A Lessons UserData
                                    userData.userGuid = userId;
                                    userData.individualId = IndividualId;
                                    userData.User.UserId = userId
                                    userData.User.IndividualId = IndividualId
                                }
                                if (save) {
                                    //update cookie than save to server
                                    curriculum.global.utils.setCookie('UserGuid', userId);
                                    save.global.utils.saveData();
                                }
                            });
                        } else {
                            // save as data with current anonymous UserGuid
                            save.global.utils.saveData()
                        }
                    }
                });
            });
        }
    }, // END SaveAfterAuthetic

  // Other common scripts
    common: {
      init: function () {
          // Open rel="external" links in new window
          $('a[rel*=external]').click(function () {
            this.target = "_blank";
          });

          // Fix Placeholders in non-compliant browsers
          if (!Modernizr.input.placeholder) {

            $('[placeholder]').focus(function () {
              var input = $(this);
              if (input.val() == input.attr('placeholder')) {
                input.val('');
                input.removeClass('placeholder');
              }
            }).blur(function () {
              var input = $(this);
              if (input.val() == '' || input.val() == input.attr('placeholder')) {
                input.addClass('placeholder');
                input.val(input.attr('placeholder'));
              }
            }).blur();
            $('[placeholder]').parents('form').submit(function () {
              $(this).find('[placeholder]').each(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                  input.val('');
                }
              });
            });
          }
      }
    },
    // END common scripts
  };
  /*********** END curriculum global var ****************/

  $(function () {
   var individualId = curriculum.global.utils.getCookie('IndividualId');
   var remeberMe = curriculum.global.utils.getCookie('RememberMe');
    if (remeberMe !== null){
            if (individualId === null){
               var baseUrl = document.location.protocol + "//" + document.location.host,
                lessonLandingUrl = baseUrl + "/index.html?ReturnUrl=/lesson1/";
                window.location = lessonLandingUrl;
                return false;
            }
    }

    curriculum.global.tracking.init();

    // Throw empty colorbox for loading screen until things are ready
    $("body").addClass("lessons-skin");

    $.colorbox({
      initialWidth: 50,
      initialHeight: 50
    });

    var setUserData = function (userId) {
      if (userId === undefined || userId === null || userId == 0) {
        return;
      }

      curriculum.global.utils.setCookie("UserGuid", userId);

      // Load User data
      userData = new Object();
      curriculum.global.utils.server.retrieveFromServer(userId, userData);

      curriculum.global.utils.init();
    };

    var getOrCreateUser = function (individualId) {
        if (individualId == EMPTY_GUID) {
            // Anonymous user
            curriculum.global.utils.server.createUser(individualId, setUserData);
        } else {
            // Logged In user
            document.cookie = 'IndividualId=' + individualId + '; PATH=/;  Domain=saltmoney.org;';
            $(document.body).click(function () {
                SALT.trigger('sessionTimeOut:reset');
            });
            curriculum.global.utils.server.getUserIdByIndiviualId(individualId, function (userId) {
                if (userId != null) {
                    // Existing Lessons User
                    setUserData(userId);
                } else {
                    // New Lessons User
                    curriculum.global.utils.server.createUser(individualId, setUserData);
                }
                SALT.trigger('content:todo:inProgress');
            });
        }
    };

    var CheckForExistingLesson = function (){
        require(['salt/models/SiteMember'], function(SiteMember) {
        SiteMember.done(function(siteMember) {

            var userId = curriculum.global.utils.getCookie('UserGuid');
            var individualId = curriculum.global.utils.getCookie('IndividualId');
            //try Athenticated first
            if (siteMember.IsAuthenticated === 'true')
            {
              //If response.IndividualId is populated, the user is recognized as an ASA User
              userInfo.state = '11',
              userInfo.siteMemberId = siteMember.MembershipId;

              if(siteMember.IndividualId === individualId && userId)
              {
                 //setUserData(userId);

                 getOrCreateUser(siteMember.IndividualId);
              }
              else
              {
                getOrCreateUser(siteMember.IndividualId);
              }
            }
            else if( userId )
            {
              //If siteMember.IsAuthenticate isn't populated or can't be found, the user isn't logged in or an error occured
              //Try userId next
              setUserData(userId);
            }
            // else if( individualId )
            // {
            //   //If last  attempt before with individualId giving and creating a new lesson
            //   getOrCreateUser(individualId);
            // }
            else
            {
              //If response.IndividualId  isn't populated or can't be found, the user isn't logged in or an error occured
              getOrCreateUser(EMPTY_GUID);
            }
          }).fail(function() {
             //all else failed creat a new one
            getOrCreateUser(EMPTY_GUID);
          });
      });
    };

    // var userId = curriculum.global.utils.getCookie('UserGuid');
    // var individualId = curriculum.global.utils.getCookie('IndividualId');

    CheckForExistingLesson();
  });

    SALT.on('content:todo:completed', function() {
        SiteMember.done(function (siteMember) {
          var todoObject = { MemberID: siteMember.MembershipId, ContentID: '101-2702', RefToDoStatusID: 2, RefToDoTypeID: 1 };
          SALT.services.upsertTodo(function () {}, JSON.stringify(todoObject));
        });
    });
    SALT.on('content:todo:inProgress', function() {
        SiteMember.done(function (siteMember) {
          var todoObject = { MemberID: siteMember.MembershipId, ContentID: '101-2702', RefToDoStatusID: 4, RefToDoTypeID: 1 };
          SALT.services.upsertTodo(function () {}, JSON.stringify(todoObject));
        });
    });

  $(window).load(function () {
    // Let's load addthis once everything else is kicked off so it doesn't hang everything else
    curriculum.global.utils.initSharing();
  });
});
