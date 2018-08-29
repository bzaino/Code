define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/step4/views"

  // Plugins
  //,"lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var step4 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step4.Model = Backbone.Model.extend({
  
    localStorage: new Store("step4-backbone"),
    url: app.serverUrl + '/Lesson2/CardInformation/',
    // Default attributes.
    defaults: {
      //id: 'step4',
      payment: '',
      minimumPayment: false
    },

    validate: function(attrs, opts) {
      opts = opts || {};
      
      if(opts.init){
        //return;

      } else {
        var isValid = true;
        
        var error = {};
        var payment = attrs.payment;

        if ($.trim(payment).length < 1 || payment == 0) {
          error.payment = 'you need a balance for this expense';
          isValid = false;
        } else {
          error.payment = false;
        }

        if(isValid){
          // If the attributes are valid, don't return anything from validate http://backbonejs.org/#Model-validate
        } else {  
          return error;
        } 
      }
    },
    modelValidate: function(options){
      options = options || {};
      var modelValid = true;
      if (options.silent || !this.validate) return true;

      var error = this.validate(this.attributes, options);


      if(error) {
        modelValid = false;
        if (options && options.error) {
          options.error(this, error, options);
        } else {
          this.trigger('error', this, error, options);
        }      
      } else {
        if (options && options.success) {
          options.success(this, error, options);
        } else {
          this.trigger('success', this, options);
        }      
      }

      return modelValid;
    },
    parse: function(resp,xhr) {
      if(resp != null){
        cardInformation = resp;

        cardInformation.payment = cardInformation.MonthlyPayment;
        cardInformation.minimumPayment = cardInformation.MakesMinimumPayment;
        return cardInformation;
      }
    },
    updateForServer: function() {
      
      var updatedProps = {
        MonthlyPayment: this.get('payment'),
        MakesMinimumPayment: this.get('minimumPayment')
      }
      this.set(updatedProps,{silent: true});
    }
      

  });

  // Attach the Views sub-module into this module.
  step4.Views = Views;

  // Required, return the module for AMD compliance
  return step4;

});
