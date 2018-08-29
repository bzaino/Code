var defArray = [
    'require',
 // Application. 
    'lesson3/app',
    'lesson3/modules/loan-type',
    'lesson3/modules/bar-graph',
    'lesson3/modules/favorites',
    'lesson3/modules/compare',
    'lesson3/modules/user',
 /* Repayment models 
    step 2 repayment model */
    'lesson3/modules/standard-repayment',
 // step 3 repayment models 
    'lesson3/modules/pay-faster',
    'lesson3/modules/set-timeline',
 // 'lesson3/modules/better-interest', 
 // step 4 repayment models 
    'lesson3/modules/graduated-repayment',
    'lesson3/modules/income-based-repayment',
    'lesson3/modules/extended-repayment',
    'lesson3/modules/income-sensitive-repayment',
 // step 5 repayment models 
    'lesson3/modules/in-school-deferment',
    'lesson3/modules/hardship-deferment',
    'lesson3/modules/forbearance'
];

/* create an array of nav options 
   router uses this to manage the state of the app */
var navOptions = [
    {name: 'Your Balance', page: 'step/1',visited: false, completed: false, introModalChecked: false, firstTime: true},
    {name: 'Standard Repayment', page: 'step/2',visited: false, completed: false},
    {name: 'Pay it down faster', page: 'step/3',visited: false, completed: false},
    {name: 'Lowering your payments', page: 'step/4',visited: false, completed: false},
    {name: 'Deferment and Forbearance', page: 'step/5',visited: false, completed: false},
    {name: 'Review your favorites', page:'step/6', visited: false, completed: false}
];

// TODO -- Externalize these into a config file 
var beginStep =1,endStep=navOptions.length;
    for(var i=beginStep;i<=endStep;i++) {
    defArray.push('lesson3/modules/step'+i);
}

// use this only while developing to bypass page checking 
var freeSteps = true;

define(defArray,
function (require) {
    var app = require('lesson3/app');
        LoanType = require('lesson3/modules/loan-type');
        BarGraph = require('lesson3/modules/bar-graph');
        Favorites = require('lesson3/modules/favorites');
        Compare = require('lesson3/modules/compare');
        User = require('lesson3/modules/user');
        steps = {};

  for (var i = beginStep; i <= endStep; i++) {
    steps[i] = require('lesson3/modules/step' + i);
  }

 // step 3 repayment models 
var standardRepayment = require('lesson3/modules/standard-repayment');
    payFaster = require('lesson3/modules/pay-faster');
    setTimeline = require('lesson3/modules/set-timeline');
 /* SWD-3312 QC6977 
    betterInterest = require('lesson3/modules/better-interest');
    step 4 repayment models */
    graduatedRepayment = require('lesson3/modules/graduated-repayment');
    incomeBased = require('lesson3/modules/income-based-repayment');
    extendedRepayment = require('lesson3/modules/extended-repayment');
    incomeSensitive = require('lesson3/modules/income-sensitive-repayment');
 // step 5 repayment models 
    inSchoolDeferment = require('lesson3/modules/in-school-deferment');
    hardShipDeferment = require('lesson3/modules/hardship-deferment');
    forbearance = require('lesson3/modules/forbearance');

 // Defining the application router, you can attach sub routers here. */

var Router = Backbone.Router.extend({
    activeStep: 0,
    currentStep: 0,
    theBarGraph: null,
    repaymentModels: {},
    models: {},
    collections: {},

    routes: {
    '': 'checkStep',
    'step/:n': 'checkStep'
    },

    initialize: function () {

      if (Backbone.Asa.readCookie('UserGuid')!== null ) {
        Backbone.Asa.User.userGuid = Backbone.Asa.readCookie('UserGuid');
      }

      var user = this.getOrCreateUser();

      app.user = user;

      this.initializeCollections();

      /* Load repayment models
      Must be after collections initialized, since we're looking in loanType collection for totalBalance if available */
      this.initializeRepaymentModels();
    },

    initializeCollections: function () {
      // loan type model 
      this.collections.loanType = new LoanType.LoanTypeList();
      this.collections.loanType.fetch({ silent: true, async: false });

      if (this.collections.loanType.models !== null && this.collections.loanType.models[0] !== null && typeof this.collections.loanType.models[0] !== 'undefined') {
        app.baseLoanTotal = this.collections.loanType.models[0].get('sum');
      }

      // favorites model 
      this.collections.favorites = new Favorites.FavoritesList();
      this.collections.favorites.fetch({ silent: true });

      // compare model 
      this.collections.compare = new Compare.CompareList();
      this.collections.compare.fetch({ silent: true });
    },

    initializeRepaymentModels: function () {
      // Passed to repaymentModel constructors to override default attributes 
      var initializeParameters = {}; 

      // Pull totalBalance from loanType collection's sum, set all repaymentModel's totalBalance 
      var totalBalance = 0;

      // Get totalBalance value from collection, if available 
      if (typeof this.collections.loanType.models[0] !== 'undefined') {
        totalBalance = this.collections.loanType.models[0].get('sum');
        initializeParameters = { 'totalBalance': totalBalance };
      }

      // Step 2 
      this.repaymentModels.standardRepayment = new standardRepayment.Model(initializeParameters);
      this.repaymentModels.standardRepayment.fetch({ silent: true });
      // Step 3 
      this.repaymentModels.payFaster = new payFaster.Model(initializeParameters);
      this.repaymentModels.payFaster.fetch({ async: false, silent: true });
      this.repaymentModels.setTimeline = new setTimeline.Model(initializeParameters);
      this.repaymentModels.setTimeline.fetch({ async: false, silent: true });
      /* SWD-3312 QC6977
      this.repaymentModels.betterInterest = new betterInterest.Model(initializeParameters);
      this.repaymentModels.betterInterest.fetch({ async: false, silent: true });

      Step 4 */
      this.repaymentModels.graduatedRepayment = new graduatedRepayment.Model(initializeParameters);
      this.repaymentModels.graduatedRepayment.fetch({ async: false, silent: true });
      this.repaymentModels.incomeBased = new incomeBased.Model(initializeParameters);
      this.repaymentModels.incomeBased.fetch({ async: false, silent: true });
      this.repaymentModels.extendedRepayment = new extendedRepayment.Model(initializeParameters);
      this.repaymentModels.extendedRepayment.fetch({ async: false, silent: true });
      this.repaymentModels.incomeSensitive = new incomeSensitive.Model(initializeParameters);
      this.repaymentModels.incomeSensitive.fetch({ async: false, silent: true });
      // Step 5 
      this.repaymentModels.inSchoolDeferment = new inSchoolDeferment.Model(initializeParameters);
      this.repaymentModels.inSchoolDeferment.fetch({ async: false, silent: true });
      this.repaymentModels.hardShipDeferment = new hardShipDeferment.Model(initializeParameters);
      this.repaymentModels.hardShipDeferment.fetch({ async: false, silent: true });
      this.repaymentModels.forbearance = new forbearance.Model(initializeParameters);
      this.repaymentModels.forbearance.fetch({ async: false, silent: true });
    },

    checkStep: function (n) {
      // Coming from index, n is undefined; default page load to step 1
      if (typeof (n) === 'undefined') {
        n = 1;
        /* have the router trigger step 1
           app.router.navigate(navOptions[0].page, {trigger: true});*/
      } else {
        n = Number(n);
      }

      if (navOptions[n - 1].visited === true || freeSteps) {
        this.step(n);
      } else {
        app.router.navigate(navOptions[0].page, { trigger: true });

      }
    },
    animations: {
      showContent: function (el) {
        var animationDuration = 1050;
        $(el).find('#step').hide().fadeIn(animationDuration);
        $('body').show();

      },
      animationDuration: 850,
      showRight: function (el) {
        var animationDuration = 1050;

        $(el).find('#right-column').css('right', '-34%').animate({
          right: '0%'
        }, this.animationDuration);

        $(el).find('#content').css('left', '34%').animate({
          left: '0%'
        }, this.animationDuration);

        $(el).find('#left-column').css('left', '0%').animate({
          left: '-34%'
        }, this.animationDuration);

        $(el).find('#footer').css('left', '34%').animate({
          left: '0%'
        }, this.animationDuration);
      },
      showLeft: function (el) {
        var animationDuration = 1050;

        $(el).find('#right-column').css('right', '0%').animate({
          right: '-34%'
        }, this.animationDuration);

        $(el).find('#left-column').css('left', '-34%').animate({
          left: '0'
        }, this.animationDuration);

        $(el).find('#content').css('left', '0%').animate({
          left: '34%'
        }, this.animationDuration);

        $(el).find('#footer').css('left', '0%').animate({
          left: '34%'
        }, this.animationDuration);

      },
      showFull: function (el) {
        var animationDuration = 1050;

        $(el).find('#right-column').animate({
          right: '-34%'
        }, this.animationDuration);

        $(el).find('#left-column').animate({
          left: '-34%'
        }, this.animationDuration);

        $(el).find('#content').animate({
          left: '0',
          width: '100%'
        }, this.animationDuration);

        $(el).find('#footer').animate({
          left: '0',
          width: '92%',
          marginRight: '4%',
          marginLeft: '4%'
        }, this.animationDuration);
      },
      forceLeft: function (el) {
        var animationDuration = 1050;

        $(el).find('#left-column .content').hide().fadeIn(animationDuration);
        $(el).find('#left-column').css('left', '0%');
     // $(el).find('#right-column').css('right', '0%'); 
        $(el).find('#content').css('left', '34%');
        $(el).find('#footer').css('left', '34%');

      },
      init: function (el) {
        $(el).find('#left-column').css('left', '-34%');
        $(el).find('#right-column').css('right', '0%');
        $(el).find('#content').css('left', '0%');
        $(el).find('#footer').css('left', '0%');
        $('body').hide().delay(400).fadeIn();
      }
    },

    changeDefaultSharingUrl: function () {
      var $addthisWrapper = $('.addthis_toolbox'),
          baseUrl = document.location.protocol + '//' + document.location.host,
          lessonLandingUrl = baseUrl + '/content/media/Lesson/Own-Your-Student-Loans/_/R-101-4479';
          
          $addthisWrapper.attr('addthis:url', lessonLandingUrl);
    },

    setupSharing: function () {
      var self = this;

      if (typeof addthis !== 'undefined') {
        this.changeDefaultSharingUrl();
        this.resetSharing();
      } else {
        $.getScript('//s7.addthis.com/js/300/addthis_widget.js#pubid=xa-50f5e9a91f8c0bfc&async=1', function () {
          if (typeof addthis !== 'undefined') {
            self.changeDefaultSharingUrl();
            addthis.init();
          }
        });
      }
    },

    resetSharing: function () {
      addthis.toolbox('.addthis_toolbox');
    },

    showLoginPrompt: function(redirecthref) {
      var CBOX_CLOSED = 'cbox_closed',
      $saveProxy = $('#loginSubmit');
      $saveProxy.data('redirect', redirecthref);
      $('#loginSubmit').trigger('click');
      $(document).unbind(CBOX_CLOSED).bind(CBOX_CLOSED, function() {
        app.router.linkTo(redirecthref);
      });
      $(document).delegate('#forgotPasswordSubmit', 'click', function() {
        $(document).unbind(CBOX_CLOSED);
      });
    },

    linkTo: function(href) {
   // NOTE: instead of window.location creating a blank form and submitting to preserve back button
      var $form = $('<form method="GET" action="' + href + '" />');
      $('body').append($form);
      $form.submit();
    },

    step: function (n) {
      var viewOptions = viewOptions || {};
      viewOptions.collection = this.collections.loanType;

   // mark this step as visited
      navOptions[n - 1].visited = true;
      this.activeStep = n;

   // set create and set same models for the entire experience
      if (n === 1) {
        this.models.step1 = new steps[1].Model();
        this.models.step1.fetch({ silent: true });
        viewOptions.model = this.models.step1;
      } else if (n === 2) {
     // step model 
        this.models.step2 = new steps[2].Model();
        this.models.step2.fetch({ silent: true });
        viewOptions.model = this.models.step2;
      } else if (n === 3) {
     // step model 
        this.models.step3 = new steps[3].Model();
        this.models.step3.fetch({ silent: true });
        viewOptions.model = this.models.step3;
      } else if (n === 4) {
     // step model 
        this.models.step4 = new steps[4].Model();
        this.models.step4.fetch({ silent: true });
        viewOptions.model = this.models.step4;
      } else if (n === 5) {
        // step model 
        this.models.step5 = new steps[5].Model();
        this.models.step5.fetch({ silent: true });
        viewOptions.model = this.models.step5;
      } else if (n === 6) {
     // step model 
        this.models.step6 = new steps[6].Model();
        this.models.step6.fetch({ silent: true });
        viewOptions.model = this.models.step6;
      }

      this.handleStepTransitionAnimation(n, viewOptions);
      this.setupSharing();
    }, 
    // END step

    getOrCreateUser: function() {
      var successCallback = function(model, response, options) {
        Backbone.Asa.User.userGuid = model.get('id');
        if (!Backbone.Asa.User.userGuid) {
          user = new User.Model({ id: null });
          user.save(null, { async: false, wait: true, success: successCallback });
        };
        Backbone.Asa.User.setUserCookie(Backbone.Asa.User.userGuid);
        var lesson2Step = model.get('Lesson3Step') || 0;
        for(var i = 0; i < lesson2Step; i++){
          navOptions[i].completed = true;
          if (navOptions[i+1]) navOptions[i+1].visited = true;
        }
      };

      var errorCallback = function(model,response,options) {
        if(response.status === 404) {
          model.set('id', null);
          model.save(null, {async:false, wait: true, success: successCallback });
          user = model;
        }
      };

      var user;

      if (Backbone.Asa.User.individualId && !Backbone.Asa.User.userGuid) {
     // We have an IndividualId but no user; try to look up the user by IndividualId
        user = new User.Model({ IndividualId: Backbone.Asa.User.individualId });
        user.fetch({ url: user.url + 'IndividualId/' + Backbone.Asa.User.individualId, async: false, success: successCallback, error: errorCallback });
        if (user.id === null && Backbone.Asa.User.userGuid)  { 
          user.id = Backbone.Asa.User.userGuid; 
        }
      } else if (Backbone.Asa.User.userGuid) {
     // We know the UserId; try to look up the user
        user = new User.Model({ id: Backbone.Asa.User.userGuid });
        user.fetch({ url: user.url + Backbone.Asa.User.userGuid, async: false, success: successCallback, error: errorCallback });
      } else {
     // We don't have any Ids; create a new anonymous user
        user = new User.Model({ id: null });
        user.save(null, { async: false, wait: true, success: successCallback });
      }

      return user;
    },

    handleStepTransitionAnimation: function (stepNum, viewOptions) {
      
      var proxyAnimateFunction;
          contentView = new steps[stepNum].Views['step' + stepNum + 'Content'](viewOptions);
          headerView = new steps[stepNum].Views['step' + stepNum + 'Header'](viewOptions);
          barGraphView = new steps[stepNum].Views['step' + stepNum + 'BarGraph'](viewOptions);
          graphModifiersView = new steps[stepNum].Views['step' + stepNum + 'GraphModifiers'](viewOptions);
          favoritesView = new steps[stepNum].Views['step' + stepNum + 'Favorites'](viewOptions);
          compareView = new steps[stepNum].Views['step' + stepNum + 'Compare'](viewOptions);
          footerView = new steps[stepNum].Views['step' + stepNum + 'Footer'](viewOptions);
          options = {};
      

      /* STEP TRANSITIONS 
      When currentStep === 0 it means it is the 1st time loading app */
      
      if (this.currentStep === 0) {
        proxyAnimateFunction = this.animations.init;
      }
      if (this.currentStep > 1 && stepNum > 1 && stepNum < 6) {
        proxyAnimateFunction = this.animations.forceLeft;
      } else if (stepNum > 1 && stepNum < 6) {
        proxyAnimateFunction = this.animations.showLeft;
      } else if (stepNum > 5) {
        proxyAnimateFunction = this.animations.showFull;
      } else if (this.currentStep > 1 && stepNum === 1) {
        proxyAnimateFunction = this.animations.showRight;
      }

      var showContentFunction = this.animations.showContent;

      options.animate = function (el) {

        if (proxyAnimateFunction) {
          proxyAnimateFunction(el);
        }

        showContentFunction(el);
      };

      app.useLayout('main', options).setViews({
        // Attach the root content View to the layout.
        '#step': contentView,
        '#header': headerView,
        '#right-column': barGraphView,
        '#repayment-options': graphModifiersView,
        '#select-repayment-options': favoritesView,
        '#compare': compareView,
        '#footer': footerView
      }).render(options.animate);

      // Update currentStep after all animation/transitions completed from prev step
      this.currentStep = stepNum;
    },

    globalNavigation: function (page, direction) {

      /*
      if navigating to a specific page,
      check to see if its been visited before in the navOptions array
      before navigating to it
      otherwise do nothing
      */
      if (page !== null) {
        if (navOptions[page].visited === true) {
       // $('#header-dropdown ul').find('li[data-iteration=' + app.router.activeStep + ']').addClass('completed');
          app.router.navigate(navOptions[page].page, { trigger: true });
        }
      }

      
      // if direction is set
      
      if (direction) {
        var nextStep;

        if (direction === 'next') {
          nextStep = parseInt(this.currentStep, 10);

          if (nextStep < navOptions.length) {
            // mark the step you're coming from as completed
            navOptions[nextStep - 1].completed = true;

            // mark the step you're moving to as ready
            // then navigate to it
            navOptions[nextStep].visited = true;
            app.router.navigate(navOptions[nextStep].page, { trigger: true });
          }
        } else if (direction === 'prev') {
          nextStep = parseInt(this.currentStep, 10) - 2;
          app.router.navigate(navOptions[nextStep].page, { trigger: true });
        }
      }
    }, 
// END globalNavigation
    
    setLessonStep: function(step) {
      var userStep = app.user.get('Lesson3Step');
      if (step > userStep) {
        app.user.set('Lesson3Step', step);
        app.user.save();
      }
    }

  });

  return Router;

});

