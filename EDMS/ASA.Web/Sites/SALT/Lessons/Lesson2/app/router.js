var defArray = [
  "require",
  // Application.
  "lesson2/app",
  "lesson2/modules/one-time-expense",
  "lesson2/modules/bar-graph",
  "lesson2/modules/new-recurring-expense",
  "lesson2/modules/imported-recurring-expense",
  "lesson2/modules/graph-modifiers",
  "lesson2/modules/user",
  "lesson2/modules/summary"
];

//create an array of nav options
//router uses this to manage the state of the app
var navOptions = [
  {name: 'Your Balance', page: 'step/1',visited: true, completed: false, introModalChecked: false, firstTime: true},
  {name: 'Recurring Expenses', page: 'step/2',visited: false, completed: false},
  {name: 'One-Time purchases', page: 'step/3',visited: false, completed: false},
  {name: 'Your Payment', page: 'step/4',visited: false, completed: false},
  {name: 'Real cost over time', page: 'step/5',visited: false, completed: false}
];
var beginStep =1,endStep=navOptions.length;//Externalize these into a config file
for(var i=beginStep;i<=endStep;i++) {
  defArray.push('lesson2/modules/step'+i);
}

//use this only while developing to bypass page checking
var freeSteps = true;

define(defArray,
function (require) {
  var app = require('lesson2/app');
  var OneTimeExpense = require('lesson2/modules/one-time-expense');
  var BarGraph = require('lesson2/modules/bar-graph');
  var NewRecurringExpense = require('lesson2/modules/new-recurring-expense');
  var ImportedRecurringExpense = require('lesson2/modules/imported-recurring-expense');
  var GraphModifiers = require('lesson2/modules/graph-modifiers');
  var User = require('lesson2/modules/user');
  var Summary = require('lesson2/modules/summary');
  var steps = {};


  for (var i = beginStep; i <= endStep; i++) {
    steps[i] = require('lesson2/modules/step' + i);
  }

  // Defining the application router, you can attach sub routers here.

  var Router = Backbone.Router.extend({
    routes: {
      "": "checkStep",
      "step/:n": "checkStep",
      "setuser/:type": "testUser",
      "summary": "summary"
    },
    _routeToRegExp: function(route) {
      route = Backbone.Router.prototype._routeToRegExp.call(this, route);
      return new RegExp(route.source, 'i'); // Just add the 'i'
    },
    summary: function () {
      var content = new Summary.Views.summary();

      this.collections.importedCollection = new ImportedRecurringExpense.ImportedRecurringExpenseList();
      this.collections.importedCollection.fetch({ silent: true, async: false, url: app.serverUrl + '/ImportedExpense' });


      app.useLayout("main").setViews({
        // Attach the root content View to the layout.
        "#content": content
      }).render();

    },
    checkStep: function (n) {

      //Coming from index, n is undefined; default page load to step 1
      if (typeof (n) == 'undefined') {
        n = 1;
        // have the router trigger step 1
        // app.router.navigate(navOptions[0].page, {trigger: true});
      } else {
        n = Number(n);
      }

      if (navOptions[n - 1].visited === true || freeSteps) {
        this.step(n);
      } else {
        app.router.navigate(navOptions[0].page, { trigger: true });

      }
    },
    activeStep: 0,
    currentStep: 0,
    theBarGraph: null,
    models: {},
    collections: {},
    animations: {
      showContent: function (el) {
        var animationDuration = 1050;
        $(el).find('#step').hide().fadeIn(animationDuration);
      },
      animationDuration: 850,
      showLeft: function (el) {

        var animationDuration = 1050;

        $(el).find('#right-column').css('right', '-34%').animate({
          right: "0%"
        }, this.animationDuration);

        $(el).find('#content').css('left', '34%').animate({
          left: "0%"
        }, this.animationDuration);

        $(el).find('#left-column').css('left', '0%').animate({
          left: "-34%"
        }, this.animationDuration);

        $(el).find('#footer').css('left', '34%').animate({
          left: "0%"
        }, this.animationDuration);
      },
      showRight: function (el) {
        var animationDuration = 1050;

        $(el).find('#right-column').animate({
          right: "-34%"
        }, this.animationDuration);

        $(el).find('#left-column').animate({
          left: "0"
        }, this.animationDuration);

        $(el).find('#content').animate({
          left: "34%"
        }, this.animationDuration);

        $(el).find('#footer').animate({
          left: "34%"
        }, this.animationDuration);

      }
    },
    getOrCreateUser: function () {
      var user;

      var successCallback = function (model, response, options) {
        Backbone.Asa.User.userGuid = model.get('id');
        Backbone.Asa.User.setUserCookie(Backbone.Asa.User.userGuid);
        var lesson2Step = model.get('Lesson2Step') || 0;
        for (var i = 0; i < lesson2Step; i++) {
          navOptions[i].completed = true;
          if (navOptions[i + 1]) navOptions[i + 1].visited = true;
        }
      };

      var errorCallback = function (model, response, options) {
        if (response.status == 404) {
          model.set('id', null);
          model.save(null, { async: false, wait: true, success: successCallback });
          user = model;
        }
      };

      if (Backbone.Asa.User.individualId && !Backbone.Asa.User.userGuid) {
        // We have an IndividualId but no user; try to look up the user by IndividualId
        user = new User.Model({ IndividualId: Backbone.Asa.User.individualId });
        user.fetch({ url: user.url + 'IndividualId/' + Backbone.Asa.User.individualId, async: false, success: successCallback, error: errorCallback });
      } else if (Backbone.Asa.User.userGuid) {
        // We know the UserId; try to look up the user
        user = new User.Model({ id: Backbone.Asa.User.userGuid });
        user.fetch({ url: user.url, async: false, success: successCallback, error: errorCallback });
      } else {
        // We don't have any Ids; create a new anonymous user
        user = new User.Model({ id: null });
        user.save(null, { async: false, wait: true, success: successCallback });
      }

      return user;
    },

    testUser: function (userType) {
      //throwaway code for testing user interaction
      //navigate to setuser/<userType>
      //where userType is one of:
      // default which uses the default created backend user
      // new which creates a new user

      var UserGuid = "00000000-0000-0000-0000-000000000004";

      switch (userType) {
      case 'default':
        break;

      case 'new':
        UserGuid = guid();
        break;
      }

      if (userType != 'clear') {
        Backbone.Asa.createCookie('UserGuid', UserGuid, 1);
      } else {
        Backbone.Asa.eraseCookie('UserGuid');
      }

    },

    changeDefaultSharingUrl: function () {
      var $addthisWrapper = $(".addthis_toolbox"),
          baseUrl = document.location.protocol + "//" + document.location.host,
          lessonLandingUrl = baseUrl + "/content/media/Lesson/Master-Your-Plastic/_/R-101-4332";

      $addthisWrapper.attr("addthis:url", lessonLandingUrl);
    },

    setupSharing: function () {
      var self = this;

      if (typeof addthis !== "undefined") {
        this.changeDefaultSharingUrl();
        this.resetSharing();
      } else {
        $.getScript("//s7.addthis.com/js/300/addthis_widget.js#pubid=xa-50f5e9a91f8c0bfc&async=1", function () {
          if (typeof addthis !== "undefined") {
            self.changeDefaultSharingUrl();
            addthis.init();
          }
        });
      }
    },

    resetSharing: function () {
      addthis.toolbox(".addthis_toolbox");
    },

    showLoginPrompt: function(redirecthref) {
      var CBOX_CLOSED = "cbox_closed",
      $saveProxy = $("#loginSubmit");

      $saveProxy.data('redirect', redirecthref);
      $("#loginSubmit").trigger("click");

      $(document).unbind(CBOX_CLOSED).bind(CBOX_CLOSED, function() {
        app.router.linkTo(redirecthref);
      });

      $(document).delegate("#forgotPasswordSubmit", "click", function() {
        $(document).unbind(CBOX_CLOSED);
      });
    },

    linkTo: function(href) {
      // NOTE: instead of window.location creating a blank form and submitting to preserve back button
      var $form = $('<form method="GET" action="' + href + '" />');
      $("body").append($form);
      $form.submit();
    },

    step: function (n) {

      if (Backbone.Asa.readCookie('UserGuid') !== null) {
        Backbone.Asa.User.userGuid = Backbone.Asa.readCookie('UserGuid');
      }
      var self = this;
      var user = this.getOrCreateUser();

      //mark this step as visited
      navOptions[n - 1].visited = true;
      this.activeStep = n;
      var viewOptions = viewOptions || {};
      viewOptions.user = user;
      app.user = user;

      user.on('sync', function() {
        app.wt.trigger("lesson:overall:saveToServer", {
          step: {
            number: self.activeStep
          }
        });
      });

      if (Backbone.Asa.User.startTime === null) {
        Backbone.Asa.User.startTime = new Date();

        app.wt.trigger('lesson:overall:start', {
          user: Backbone.Asa.User.userGuid,
          time: Backbone.Asa.User.startTime
        });
      }

      if (!this.theBarGraph) {
        this.theBarGraph = new BarGraph.Model();
        // this.theBarGraph.fetch();
      } else {
        this.theBarGraph = this.theBarGraph;
      }

      //set create and set same models for the entire experience
      if (n >= 1) {
        if (!this.models.step1) {
          this.models.step1 = new steps[1].Model();
          this.models.step1.fetch({ silent: true, async: false });
        }

        this.theBarGraph.set({ balance: this.models.step1.get('balance') });

        if (n == 1) viewOptions.model = this.models.step1;
      }
      if (n >= 2) {
        if (!this.models.step2) {
          this.models.step2 = new steps[2].Model();
          this.models.step2.fetch({ silent: true, async: false });
        }

        this.theBarGraph.set({ expenses: this.models.step2.get('collectionSum') });

        if (n >= 2) {
          viewOptions.model = this.models.step2;

          //globalize imported expenses Collection
          //TODO this collection should be locally scoped to step 2 since we're no longer using it outside of that
          //If this is the user's first time hitting step 2 and the importedCollection is null, pull in imported expenses
          if (!this.collections.importedCollection) {
            this.collections.importedCollection = new ImportedRecurringExpense.ImportedRecurringExpenseList();
          }
          viewOptions.importedCollection = this.collections.importedCollection;
        }

        //globalize recurring expenses collection
        this.collections.recurringCollection = new NewRecurringExpense.NewRecurringExpenseList();
        this.collections.recurringCollection.importedLength = this.collections.importedCollection.length;
        this.collections.recurringCollection.fetch({ silent: true, async: false });
        viewOptions.recurringCollection = this.collections.recurringCollection;
      }
      if (n >= 3) {
        if (!this.models.step3) {
          this.models.step3 = new steps[3].Model();
          this.models.step3.fetch({ silent: true, async: false });
        }
        if (n == 3) viewOptions.model = this.models.step3;

        this.collections.oneTimeExpense = new OneTimeExpense.OneTimeExpenseList();
        this.collections.oneTimeExpense.fetch({ silent: true, async: false });
        viewOptions.collection = this.collections.oneTimeExpense;

        if (this.collections.oneTimeExpense.models) {
          var purchases = 0;
          for (var x = 0; x < this.collections.oneTimeExpense.models.length; x++) {

            var oneTimeValue = Number(this.collections.oneTimeExpense.models[x].get('value'));
            purchases += oneTimeValue;
          }
          this.theBarGraph.set({ purchases: purchases });
        }
      }
      if (n >= 4) {
        if (!this.models.step4) {
          this.models.step4 = new steps[4].Model();
          this.models.step4.fetch({ silent: true, async: false });
        }
        if (n == 4) viewOptions.model = this.models.step4;
      }
      if (n >= 5) {
        if (!this.models.step5) {
          this.models.step5 = new steps[5].Model();
          this.models.step5.fetch({ silent: true, async: false });
        }

        if (!this.models.graphModifiersModel) {
          this.models.graphModifiersModel = new GraphModifiers.Model();
          this.models.graphModifiersModel.fetch({ silent: true, async: false });
          this.models.graphModifiersModel.on('change', function (arg1, arg2) { });
        }

        if (n == 5) {
          viewOptions.graphModifiersModel = this.models.graphModifiersModel;
          viewOptions.model = this.models.step5;
        }
      }

      //set barGraph every time
      viewOptions.barGraph = this.theBarGraph;
      viewOptions.models = this.models;
      viewOptions.collections = this.collections;
      var contentView, headerView, barGraphView, graphModifiersView, footerView;

      contentView = new steps[n].Views['step' + n + 'Content'](viewOptions);
      headerView = new steps[n].Views['step' + n + 'Header'](viewOptions);
      barGraphView = new steps[n].Views['step' + n + 'BarGraph'](viewOptions);
      graphModifiersView = new steps[n].Views['step' + n + 'GraphModifiers'](viewOptions);
      footerView = new steps[n].Views['step' + n + 'Footer'](viewOptions);
      var options = {};
      var proxyAnimateFunction;

      if (this.currentStep < 5 && n == 5) {
        proxyAnimateFunction = this.animations.showRight;
      } else if (this.currentStep == 5) {
        proxyAnimateFunction = this.animations.showLeft;
      }

      var showContentFunction = this.animations.showContent;

      options.animate = function (el) {
        showContentFunction(el);
        if (proxyAnimateFunction) {
          proxyAnimateFunction(el);
        }
      };

      app.useLayout("main", options).setViews({
        // Attach the root content View to the layout.
        "#step": contentView,
        "#header": headerView,
        "#right-column": barGraphView,
        "#left-column": graphModifiersView,
        "#footer": footerView
      }).render(options.animate);
      this.currentStep = n;

      this.setupSharing();

    }, // END step

    globalNavigation: function (page, direction) {

      /**
      if navigating to a specific page,
      check to see if its been visited before in the navOptions array
      before navigating to it
      otherwise do nothing
      */
      if (page !== null) {
        if (navOptions[page].visited === true) {
          //$('#header-dropdown ul').find('li[data-iteration=' + app.router.activeStep + ']').addClass('completed');
          app.router.navigate(navOptions[page].page, { trigger: true });
        }
      }

      /**
      if direction is set
      */
      if (direction) {
        var nextStep;

        if (direction == 'next') {
          nextStep = parseInt(this.currentStep, 10);

          if (nextStep < navOptions.length) {
            //mark the step you're coming from as completed
            navOptions[nextStep - 1].completed = true;

            //mark the step you're moving to as ready
            //then navigate to it
            navOptions[nextStep].visited = true;
            app.router.navigate(navOptions[nextStep].page, { trigger: true });
          }
        } else if (direction == 'prev') {
          nextStep = parseInt(this.currentStep, 10) - 2;
          app.router.navigate(navOptions[nextStep].page, { trigger: true });
        }
      }
    }, //END globalNavigation

    recurringExpensesAutocomplete: function () {
      $('#recurring-expenses .new-recurring-name').autocomplete({
        source: this.availableExpenses(),
        position: {
          my: "right top",
          at: "right bottom",
          offset: "0 20"
        }
      });



    },
    availableExpenses: function () {
      var expensesArray = [];
      //      $.ajax({async: false,url:app.serverUrl+'/ExpenseType',success: function(data){
      //        $.each(data, function(k,v) {
      //          expensesArray.push(v.Name);
      //        });
      //      }});

      return expensesArray;

    }
  });

  return Router;

});
