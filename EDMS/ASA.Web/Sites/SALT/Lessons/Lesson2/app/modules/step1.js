define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/step1/views",

  // Plugins
  "lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var step1 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step1.Model = Backbone.Model.extend({
    localStorage: new Store("step1-backbone"),
    url: app.serverUrl + '/Lesson2/CardInformation/' ,
    // Default attributes.
    defaults: {
      balance: '',
      interestRate: ''
    },

    validate: function(attrs, opts) {
      opts = opts || {};
      
      if(opts.init){
        //return;

      } else {
        var isValid = true;

        var error = {};
        var balance = attrs.balance;
        var interestRate = attrs.interestRate;
        
        if ($.trim(balance).length < 1 || balance == 0) {
          error.balance = 'you need a balance for this expense';
          isValid = false;
        } else {
          error.balance = false;
        }

        if ($.trim(interestRate).length < 1 || interestRate == 0) {
          error.interestRate = 'you need a interestRate';
          isValid = false;
        } else {
          error.interestRate = false;
        }


        if (isValid) {
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
      //Response varies based on specific conditions
      if(resp != null){
        var cardInformation;
        
        if (resp.hasOwnProperty('Balance') && resp.hasOwnProperty('InterestRate')) {
            cardInformation = resp;
        } else if (resp[0].hasOwnProperty('Balance') && resp[0].hasOwnProperty('InterestRate')) {
            cardInformation = resp[0];
        }
        
        if (typeof cardInformation !== 'undefined') { 
            cardInformation.balance = cardInformation.Balance;
            cardInformation.interestRate = cardInformation.InterestRate;
            cardInformation.id = cardInformation.CardInformationId;
        }
        return cardInformation;
      }
    },
    updateForServer: function() {
      
      var updatedProps = {Balance: this.get('balance'),
        InterestRate: this.get('interestRate')
      }
      this.set(updatedProps,{silent: true});
      
    }

  });

  // Attach the Views sub-module into this module.
  step1.Views = Views;

  // Required, return the module for AMD compliance
  return step1;

});
