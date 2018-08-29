define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step1/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step1 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step1.Model = Backbone.Model.extend({
    
    localStorage: new Store("step1-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step1',
      balance: ''
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
    }

  });

  // Attach the Views sub-module into this module.
  step1.Views = Views;

  // Required, return the module for AMD compliance
  return step1;

});
