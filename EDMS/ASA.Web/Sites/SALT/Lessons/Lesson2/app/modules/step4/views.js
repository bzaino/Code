define([
  "lesson2/app",
  "lesson2/modules/step4",
  "lesson2/modules/header",
  "lesson2/modules/bar-graph",
  "lesson2/modules/graph-modifiers",
  "lesson2/modules/footer",

  // Libs
  "backbone",
  "lesson2/plugins/asa-plugins"
],

function(app, step4, header, barGraph, graphModifiers, footer, Backbone, asaPlugins) {

  var Views = {};
  var viewsHolder = {};

  //step 4 content view
  Views.step4Content = Backbone.View.extend({
    template: 'content/step4',
    className: 'step4-content',
    model: 'step4',

    initialize: function(options) {
      this.model = options.model;
      viewsHolder.content = this;

      this.activeStep = app.router.activeStep;
      this.preloaded = (this.model.get('payment') > 0);

      app.wt.trigger('lesson:step:start', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:    this.activeStep,
          preloaded: this.preloaded
        }
      });
    },
    afterRender: function(){
      this.getData();
      //placeholder text IE
      $('input[type=text]').placeHolder();
      $('input#cc-minimum-payment').uniform();
    },
    getData: function(){
      $('#cc-payment').val(this.model.get('payment')).formatCurrency({ roundToDecimalPlace: 0 });

      var checkedPayment = this.model.get('minimumPayment');
      if (checkedPayment == true) {
        $('#cc-minimum-payment').attr('checked',true);
      } else {
        $('#cc-minimum-payment').attr('checked',false);
      }


    },
    events: {
      'keyup #cc-payment': 'handlePaymentChange',
      'change #cc-minimum-payment': 'handleMinimumPaymentChange'
    },

    handlePaymentChange: function(event) {
      var element = $(event.target);

      element.toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      var cleanInput = Number(element.val().replace(/[^0-9\.-]+/g,""));
      this.model.set({payment: cleanInput}, {silent: true});

      if(element.val().length >= 1){
        $('#total .value').text($(event.target).val());
      } else{
        $('#total .value').text('$0');
      }
    },

    handleMinimumPaymentChange: function(event) {
      if ($(event.target).is(':checked')) {
        this.model.set({minimumPayment: true}, {silent: true});
      }
      else {
        this.model.set({minimumPayment: false}, {silent: true});
      }

    }
  });

  //header view
  Views.step4Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 4, stepName: 'Your Payment'});
      this.insertView(new header.Views.Header({model: hdr}));
    }
  });

  //right column graph view
  Views.step4BarGraph = Backbone.View.extend({
    initialize: function(options) {
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        model: options.barGraph,
        viewsHolder:viewsHolder
      }));
    }
  });

  //left column graph modifiers
  Views.step4GraphModifiers = Backbone.View.extend({
    initialize: function() {
      this.insertView(new graphModifiers.Views.GraphModifiers({
        viewsHolder: viewsHolder.content
      }));
    }
  });

  //footer view
  Views.step4Footer = Backbone.View.extend({
    initialize: function(options) {
      this.step4Model = options.model;

      var ftr = new footer.Model({amount: 0, description: 'monthly payment', subtitle: 'Options to reduce your revolving debt.', nextButton: 'Keep Going'});
      this.insertView(new footer.Views.Footer({model: ftr}));
    },

    events:{
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    afterRender: function() {
      this.getData();
    },

    getData: function() {
      var footerValue = this.step4Model.get('payment');
      if(footerValue >= 1){
        $('#total .value').text('$'+footerValue);
      } else{
        $('#total .value').text('$0');
      }
    },

    handleNextButton: function(event) {
      event.preventDefault();
      var step4Valid = true;

      if(!this.model.modelValidate(Views.Step4ValidationLogic)) {
        step4Valid = false;
        //display error message
        $('.error-msg').fadeIn();
      }

      if(step4Valid){

        this.step4Model.save();
        //hide error message
        $('.error-msg').fadeOut();

        if( app.user.get('Lesson2Step') < 4 ) {
          app.user.set('Lesson2Step', 4);
        }

        app.user.save();

        app.wt.trigger('lesson:step:complete', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:    Views.step4Content.activeStep,
            preloaded: Views.step4Content.preloaded
          }
        });

        app.router.globalNavigation(null, 'next');
      }

      return false;
    },
    handlePrevButton: function(event){
      event.preventDefault();

      app.router.globalNavigation(null, 'prev');
      return false;
    }
  });

  Views.Step4ValidationLogic = {
    error:function(model, error){

      if (error.payment) {
        $('#cc-payment').addClass('error');
      } else {
        $('#cc-payment').removeClass('error');
      }

    },
    success: function(model, error){

      if(!error) {
        $('#cc-payment').removeClass('error');
      }

    }
  };

  return Views;

});
