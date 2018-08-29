define([
  "lesson3/app",

// Libs
  "backbone",
  'salt',

// Views
  "lesson3/modules/loan-type/views",

// Plugins
  "lesson3/plugins/backbone-asa-persistence",

//Base repayment model
  "lesson3/modules/baseRepayment"
],

function(app, Backbone, SALT, Views, LocalStorage, baseRepaymentModel) {

  // Create a new module
  var LoanType = app.module();

  // Define LoanType Model
  // extend from the baseRepaymentModel so we have access to the baseModel values
  // for initialization of screen values.
  LoanType.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/LoanType/',
    defaults: _.extend({
      referenceId: 'loanTypeSettingOfValues',
      name: 'LoanType-setting of values',
      income: 0,
      dependents: 1,
      state: 'other'
    }, baseRepaymentModel.Model.prototype.defaults),
    initialize: function() {
      var _this = this;
      SALT.on('state:changed', function (state) {
        _this.setState(state);
      });
      SALT.on('dependents:changed', function (dependents) {
        _this.setDependents(dependents);
      });
      SALT.on('income:changed', function (income) {
        _this.setIncome(income);
      });
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRate(interestRate);
      });
      SALT.on('totalBalance:changed', function (value) {
        _this.setSum(value);
        _this.setTotalBalance(value);
      });
    },
    setDependents: function(dependents){
      //set the model dependents
      this.set('dependents', dependents);
    },
    setIncome: function(income){
      //set the model income
      this.set('income', income);
    },
    setState: function(state){
      //set the model state
      this.set('state', state);
    },
    setTotalBalance: function(value){
      //set the model total balance
      this.set('totalBalance', value);
    },
    setSum: function(value){
      //set the model sum
      this.set('sum', value);
    },
    validate: function(attrs, opts) {
      opts = opts || {};
      
      if(opts.init){
        //return;

      } else {
        var isValid = true;
        
        var error = {};
        var value = $.trim(attrs.value);
        
        if(value.length < 1 || value == '0') {
          error.value = 'you need a VALUE';
          isValid = false;
        } else {
          error.value = false;
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

      if (error) {
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
      if(resp!== null) {

        var loanType = resp;
        loanType.id = loanType.LoanTypeId;
        loanType.loan = loanType.LoanName;
        loanType.value = loanType.LoanAmount;
        loanType.income = loanType.AnnualIncome;
        loanType.dependents = loanType.FinancialDependents;
        loanType.state = loanType.State;
        loanType.interestRate = loanType.InterestRate;

        return loanType;
      }
    },
    updateForServer: function() {
      var updatedProps = {
        AnnualIncome: this.get('income'),
        FinancialDependents: this.get('dependents'),
        State: this.get('state'),
        InterestRate: this.get('interestRate'),
        LoanAmount: this.get('value'),
        LoanName: this.get('loan')
      };
      this.set(updatedProps,{silent: true});
    }

  });

  LoanType.LoanTypeList = Backbone.Collection.extend({
    url: app.serverUrl + '/Lesson3/LoanType',
    
    model: LoanType.Model,
    localStorage: new Store("LoanType-backbone"),
    
    nextOrder: function(){
      if (!this.length) {
        return 1;
      }
      return this.last().get("iteration") + 1;
    },

    newAttributes: function() {
      var iteration = this.nextOrder();
      return {
        loan: '',
        value: 0,
        sum: 0,
        iteration: iteration        
      };
    },
    parse: function(resp) {
      if(resp != null) {
        var sum = 0;
        for(var i =0;i<resp.length;i++){
          //Need to increment since the number returned by next Order is based on the number of elements in the collection,
          // and at this point we havent added the new element
          resp[i].iteration = this.nextOrder() + i;
          sum += resp[i].LoanAmount;      
        }
        
        if(resp[0] != null) {
          resp[0].sum = sum;
        }
      }
      return resp;
    }

  });

  // Attach the Views sub-module into this module.
  LoanType.Views = Views;

  // Required, return the module for AMD compliance
  return LoanType;

});
