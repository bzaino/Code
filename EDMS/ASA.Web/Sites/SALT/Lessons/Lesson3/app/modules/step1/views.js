define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/graph-modifiers",
  "lesson3/modules/favorites",
  "lesson3/modules/compare",
  "lesson3/modules/footer",
  "lesson3/modules/loan-type",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, graphModifiers, favorites, compare, footer, LoanType, Backbone, asaPlugins) {

  var Views = {};
  var viewsHolder = {};

  Views.step1Content = Backbone.View.extend({
    template: 'content/step1',
    className: 'step1-content',
    model: 'step1',

    initialize: function(options){
      this.collection = app.router.collections.loanType;
      this.model = options.model;
      this.barGraph = app.router.theBarGraph;
      viewsHolder.content = this;

      //insert loan views
      if(this.collection.models.length < 1) {
        //insert first loan item on page load
        this.collection.add(this.collection.newAttributes(),{init: true});
      }

      var loanTypeList = new LoanType.Views.LoanTypeList({
        collection: this.collection,
        viewsHolder: viewsHolder
      });
      this.loanTypeList = loanTypeList;
      loanTypeList.delegateEvents();
      this.insertView(".question",loanTypeList);

      this.bind('updateBarGraph', this.updateBarGraph);

      this.activeStep = app.router.activeStep;
      this.preloaded = (this.collection.length > 0);

      //won't fire first time through
      if (navOptions[0].firstTime === false)
      {
        app.wt.trigger('lesson:step:start', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:    this.activeStep,
            preloaded: this.preloaded
          }
        }, true);
      }
      navOptions[0].firstTime = false;
    },

    afterRender: function(){

    },

    getData: function(){

    },

    events: {
      'click .add-another': 'insertLoanType'
    },

    insertLoanType: function(event) {
      event.preventDefault();

      if(this.collection.length < 3 ) {
        var newModel = this.collection.create(this.collection.newAttributes(),{init: true});
      }

      return false;
    },

    updateBarGraph: function() {
      //trigger bargraph view handleBarChange func
      viewsHolder.barGraph.trigger('handleBarChange');
    }

  });

  Views.step1Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 1, stepName: "Select options"});
      this.insertView(new header.Views.Header({
        model: hdr
      }));
    },
    events:{
        'step1SaveData .save-and-exit' : 'saveData'
    },
    saveData: function (event) {
        var collectionValid = true;

        this.collection.each(function(element,index, list) {
            if (!element.modelValidate(Views.Step1SaveAndExitValidationUIDisplayLogic)) {
                collectionValid = false;
            }
        });

        if(collectionValid) {
            $.ajax({
                url: app.serverUrl + '/Lesson3/LoanType/',
                type: 'delete',
                async: false
            });
          
            this.collection.each(function(element,index, list){
                //remove empty models from collection
                var loan = $.trim(element.attributes.loan);
                // if the name attribute is empty,
                // then we should assume the user does not want to add this expense
                // and we should remove it from the collection before saving and moving on
                element.save();

                if(loan.length === 0){
                    element.collection.remove(element);
                }
            });

            // before moving to the next step
            // we need to update the totalBalance
            var totalBalance = app.router.collections.loanType.models[0].get('sum');
            app.baseLoanTotal = totalBalance;

            app.user.set('Lesson3Step',1);
            app.user.save();

            this.preloaded = (this.collection.length > 0);
            app.wt.trigger('lesson:step:complete', {
                user: Backbone.Asa.User.userGuid,
                time: new Date(),
                step: {
                    number:   app.user.get('Lesson3Step'),
                     preloaded: this.preloaded
                }
            });
        }
    }
  });

  Views.step1BarGraph = Backbone.View.extend({
    initialize: function(options) {
      this.barGraph = options.barGraph;
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        collection: options.collection,
        model: this.barGraph,
        viewsHolder: viewsHolder
      }));
    }
  });

  Views.step1GraphModifiers = Backbone.View.extend({
    initialize: function() {
      this.insertView(new graphModifiers.Views.GraphModifiers({
        viewsHolder:viewsHolder
      }));
    }
  });
  Views.step1Favorites = Backbone.View.extend({
    initialize: function() {
      var fav = new favorites.Model({selectRepaymentOptions: []});
      this.insertView(new favorites.Views.Favorites({
        model: fav,
        collection: app.router.collections,
        viewsHolder: viewsHolder
      }));
    }
  });
  Views.step1Compare = Backbone.View.extend({
    initialize: function() {
      this.insertView(new compare.Views.Compare({}));
    }
  });
  Views.step1Footer = Backbone.View.extend({
    initialize: function(options) {
      this.contentModel = options.model;
      this.collection = app.router.collections.loanType;
      this.barGraph = app.router.theBarGraph;

      var ftr = new footer.Model({amount: 0, description: 'loan balance', subtitle: 'Explore repayment options', nextButton: 'Keep Going', noPrev: true});
      this.insertView(new footer.Views.Footer({model: ftr}));
    },
    events:{
      'click .right-button': 'handleNextButton'
    },
    handleNextButton: function(event) {
      event.preventDefault();
      var collectionValid = true;

      this.collection.each(function(element,index, list) {
        if(!element.modelValidate(Views.Step1ValidationLogic)) {
          collectionValid = false;
          //display error message
          $('.error-msg').fadeIn();
        }
      });

      if(collectionValid) {
        //hide error message
        $('.error-msg').fadeOut();
        //this.barGraph.save();

				$.ajax({
            url: app.serverUrl + '/Lesson3/LoanType/',
            type: 'delete',
            async: false
          });
          
        this.collection.each(function(element,index, list){
          //remove empty models from collection
          var loan = $.trim(element.attributes.loan);
          /**
            if the name attribute is empty,
            then we should assume the user does not want to add this expense
            and we should remove it from the collection before saving and moving on
          */

				
                  
          element.save();

          if(loan.length === 0){
            element.collection.remove(element);
          }



        });

          
        /**
          before moving to the next step
          we need to update the totalBalance for all of the
          repayment models and have them calculate their points correctly
        */

        var repaymentModels = app.router.repaymentModels;
        var totalBalance = app.router.collections.loanType.models[0].get('sum');
        app.baseLoanTotal = totalBalance;
        $.each(repaymentModels, function(key, value){

          app.router.repaymentModels[key].setTotalBalance(totalBalance);

        });

        app.user.set('Lesson3Step',1);
        app.user.save();

        this.preloaded = (this.collection.length > 0);
        app.wt.trigger('lesson:step:complete', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:   app.user.get('Lesson3Step'),
            preloaded: this.preloaded
          }
        });

        app.router.globalNavigation(null, 'next');
      }

      return false;
    }
  });

  Views.Step1ValidationLogic = {
    error:function(model, error){
      var ele = $('.loan-type[data-iteration=' + model.get('iteration') + ']');
      if (error.value) {
        ele.find('.loan-type-value').addClass('error');
      } else {
        ele.find('.loan-type-value').removeClass('error');
      }
    },
    success: function(model, error){
      var ele = $('.loan-type[data-iteration=' + model.get('iteration') + ']');
      if(!error) {
        ele.find('.loan-type-value').removeClass('error');
      }
    }
  };

    //stub out the error and success so we can get model to validate
    //at save and exit time, but with no UI visual display
    Views.Step1SaveAndExitValidationUIDisplayLogic = {
        error:function(model, error){
        },
        success: function(model, error){
        }
    };

  return Views;

});
