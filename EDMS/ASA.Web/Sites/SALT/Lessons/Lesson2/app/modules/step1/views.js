define([
    'lesson2/app',
    'lesson2/modules/step1',
    'lesson2/modules/header',
    'lesson2/modules/bar-graph',
    'lesson2/modules/graph-modifiers',
    'lesson2/modules/footer',

    // Libs
    'backbone',
    'salt',
    'lesson2/plugins/asa-plugins'
],

function(app, step1, header, barGraph, graphModifiers, footer, Backbone, SALT) {

  var Views = {};
  var viewsHolder = {};

  Views.step1Content = Backbone.View.extend({
    template: 'content/step1',
    className: 'step1-content',
    model: 'step1',

    initialize: function(options){
      this.model = options.model;
      this.barGraph = options.barGraph;
      viewsHolder.content = this;

      this.activeStep = app.router.activeStep;
      this.preloaded = !this.model.isNew();

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
        });
      }
      navOptions[0].firstTime = false;
    },
    afterRender: function(){
      this.getData();
      //placeholder text IE
      $('input[type=text]').placeHolder();
    },

    getData: function(){
      $('#current-balance').val(this.model.get('balance')).formatCurrency({ roundToDecimalPlace: 0 });
      $('#interest-rate').val(this.model.get('interestRate'));
    },

    events: {
      'keyup #current-balance': 'handleBalanceChange',
      'keyup #interest-rate': 'handleInterestChange',
      'focus #interest-rate' : 'showInterestTooltip',
      'blur #interest-rate' : 'hideInterestTooltip'
    },

    handleBalanceChange: function(event){
      var element = $(event.target);
      //format input value
      element.toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      var cleanInput = Number(element.val().replace(/[^0-9\.-]+/g,"")); // Unescaped '-'.
      //set the step1 model
      this.model.set({balance: cleanInput}, {silent: true});

      //set barGraph
      this.barGraph.set({balance: cleanInput});
      //trigger bargraph view handleBarChange func
      viewsHolder.barGraph.trigger('handleBarChange');

      //udpate footer value
      if(element.val().length > 1){
        $('#total .value').text($(event.target).val());
      } else{
        $('#total .value').text('$0');
      }

    },
    handleInterestChange: function(event){

      //validate the name entry fields
      var interestValue = $(event.target).val();

      if (!/^\d+(\.\d{0,2})?$/.test(interestValue)) {
        interestValue = interestValue.replace(/[^\d]/g,"");
      }

      $(event.target).val(interestValue);

      var interest = parseFloat($(event.target).val());

      if(interest < 0){
        interest = 0;
        $(event.target).val(interest);
      }

      if(interest > 100){
        interest = 100;
        $(event.target).val(interest);
      }

      this.model.set({interestRate: $(event.target).val()}, {silent: true});
    },
    showInterestTooltip: function(event){
      $(event.target).next('.tip').fadeIn();
    },
    hideInterestTooltip: function(event){
      $(event.target).next('.tip').fadeOut();
    }

  });

  Views.step1Header = Backbone.View.extend({
    initialize: function(options) {
      var hdr = new header.Model({currentStep: 1, stepName: "Your Balance"});
      this.insertView(new header.Views.Header({model: hdr}));
      this.barGraph = options.barGraph;
      this.step1Model = options.model;
    },
     events:{
        'step1SaveData .save-and-exit' : 'saveData'
    },
    saveData: function (event) {
        if(this.model.modelValidate(Views.Step1SaveAndExitValidationUIDisplayLogic)) {
            this.step1Model.save();
            this.barGraph.save();

            if( app.user.get('Lesson2Step') < 1 ) {
                app.user.set('Lesson2Step', 1);
            }

            app.user.save();

            app.wt.trigger('lesson:step:complete', {
                user: Backbone.Asa.User.userGuid,
                time: new Date(),
                step: {
                    number:    Views.step1Content.activeStep,
                    preloaded: Views.step1Content.preloaded
                }
            });
        }
    },
  });

  Views.step1BarGraph = Backbone.View.extend({
    initialize: function(options) {
      this.barGraph = options.barGraph;
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
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

  Views.step1Footer = Backbone.View.extend({
    initialize: function(options) {
      this.barGraph = options.barGraph;
      this.step1Model = options.model;

      var ftr = new footer.Model({amount: 0, description: 'credit balance', subtitle: 'What are your recurring expenses?', nextButton: 'Keep Going', noPrev: true});
      this.insertView(new footer.Views.Footer({model: ftr}));
    },
    afterRender: function() {
      $('#footer').find('.value').html('$' + this.step1Model.get('balance')).formatCurrency({ roundToDecimalPlace: 0 });
    },
    events:{
      'click .right-button': 'handleNextButton'
    },
    handleNextButton: function(event) {
      event.preventDefault();
      var step1Valid = true;

      if(!this.model.modelValidate(Views.Step1ValidationLogic)) {
        step1Valid = false;
        //display error message
        $('.error-msg').fadeIn();
      }

      if(step1Valid){
        //hide error message
        $('.error-msg').fadeOut();
        this.step1Model.save();
        this.barGraph.save();

        navOptions[0].introModalChecked = true;

        if( app.user.get('Lesson2Step') < 1 ) {
          app.user.set('Lesson2Step', 1);
        }

        app.user.save();

        app.wt.trigger('lesson:step:complete', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:    Views.step1Content.activeStep,
            preloaded: Views.step1Content.preloaded
          }
        });

        app.router.globalNavigation(null, 'next');
      }

      return false;
    }
  });

  Views.Step1ValidationLogic = {
    error:function(model, error){
      if (error.balance) {
        $('#current-balance').addClass('error');
      } else {
        $('#current-balance').removeClass('error');
      }

      if (error.interestRate) {
        $('#interest-rate').addClass('error');
      } else {
        $('#interest-rate').removeClass('error');
      }
    },
    success: function(model, error){
      if(!error) {
        $('#current-balance').removeClass('error');
        $('#interest-rate').removeClass('error');
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
