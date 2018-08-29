define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/graph-modifiers/views"

],

function(app, Backbone, Views) {

  // Create a new module
  var graphModifiers = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  graphModifiers.Model = Backbone.Model.extend({

    url: app.serverUrl + '/Lesson2/DebtReductionOptions/',

    defaults: {
      increaseMonthlyPaymentChecked: false,
      increaseMonthlyPayment: 0,
      cashForRecurringExpenses: false,
      cashForOneTimePurchases: false,
      extraPaymentChecked: false,
      extraPaymentAmount: 0,
      extraPaymentMonth: 0,
      lowerInterestRateChecked: false,
      lowerInterestRate: 0
    },
    validate: function(attrs, opts) {
      opts = opts || {};
      
      if(opts.init){
        //return;
        
      } else {
        var isValid = true;

        var error = {};

        if (attrs.lowerInterestRateChecked) {
          var value = $.trim(attrs.lowerInterestRate);
          if (value.length < 1 || value < 0) {
            error.interestRate = 'you selected lower interest rate, you must enter a value';
            isValid = false;
          } else {
            error.interestRate = false;
          }
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
    parse: function(resp,xhr){  
      if(resp != null){
        var graphModifier = $.isArray(resp) ? resp[0] : resp;
        if(graphModifier.ExtraPaymentAmount > 0){
          graphModifier.extraPaymentAmount = graphModifier.ExtraPaymentAmount;
          graphModifier.extraPaymentMonth = graphModifier.ExtraPaymentMonth;
          graphModifier.extraPaymentChecked = true;
        }
        if(graphModifier.IncreaseMonthlyPayment > 0) {
          graphModifier.increaseMonthlyPayment = graphModifier.IncreaseMonthlyPayment;
          graphModifier.increaseMonthlyPaymentChecked = true;
        }
        if(graphModifier.LoweredInterestRate > 0) {
          graphModifier.lowerInterestRate = graphModifier.LoweredInterestRate;
          graphModifier.lowerInterestRateChecked = true;
        }
        graphModifier.cashForRecurringExpenses = graphModifier.PayCashForRecurringExpenses;
        graphModifier.cashForOneTimePurchases = graphModifier.PayCashForOneTimePurchases;
        graphModifier.id = graphModifier.DebtReductionOptionsId;
        return graphModifier;
      }
    },
    updateForServer: function(){
      var updatedProps = {
        ExtraPaymentAmount: this.get('extraPaymentAmount'),
        ExtraPaymentMonth: this.get('extraPaymentMonth'),
        IncreaseMonthlyPayment: this.get('increaseMonthlyPayment'),
        LoweredInterestRate: this.get('lowerInterestRate'),
        PayCashForRecurringExpenses: this.get('cashForRecurringExpenses'),
        PayCashForOneTimePurchases: this.get('cashForOneTimePurchases')
      };
      this.set(updatedProps,{silent: true});
    }
    

  });


  // Attach the Views sub-module into this module.
  graphModifiers.Views = Views;

  // Required, return the module for AMD compliance
  return graphModifiers;

});
