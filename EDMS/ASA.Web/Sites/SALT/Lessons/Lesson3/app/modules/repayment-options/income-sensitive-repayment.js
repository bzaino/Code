define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.IncomeSensitive = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
    template: 'graph-modifiers/income-sensitive-repayment',
    id: 'incomeSensitive',
    className: 'repayment-option',

    initialize: function(options) {
      var _this = this;

      //When overriding a mixin function we need to be sure to call base mixin code
      repaymentViewMixin.initialize.call(this, options);

      SALT.on('incomePctPayment:changed', function (incomePctPayment) {
        _this.setIncomePctPaymentDisplayField(incomePctPayment);
      });
      SALT.on('income:changed', function (income) {
        _this.setIncomeDisplayField(income);
      });
    },

    events: {
      'click .compare': 'triggerCompareOverlay',
      'click .update-graph': 'handleUpdateGraph',
      'change #income': 'handleInterestChange',
      'click #interest-less': 'handleInterestLess',
      'click #interest-more': 'handleInterestMore',
      'keyup #income': 'handleInterestKeyUp'
    },
    afterRender: function(){
      //pre-set existing values
      this.setIncomeDisplayField(this.model.get('income'));
      this.setIncomePctPaymentDisplayField(this.model.get('incomePctPayment'));
    },
    setIncomeDisplayField: function(value) {
      $('#is-salary').val(value).formatCurrency({ roundToDecimalPlace: 0 });
    },
    setIncomePctPaymentDisplayField: function(value) {
      $('#income').val(value + '%');
    },
    handleUpdateGraph: function(){
      this.updateProperties();
      this.calculateAndDisplay();
      return false;
    },
    updateProperties: function(){
      this.updateIncomePctPayment();
    },
    updateIncomePctPayment: function(){
      var element = $('#income');
      var value = element.val();
      var cleanValue = parseFloat(value.replace(/[^0-9\.-]+/g,""));
      
      this.model.set({'incomePctPayment': cleanValue});
    },
    handleInterestChange: function(event){
      var element = $('#income');
      
      
      
      if(parseFloat(element.val()) > this.model.get('incomePercentageMaxMin')[0]){
        element.val(this.model.get('incomePercentageMaxMin')[0]);
      } else if(element.val() < this.model.get('incomePercentageMaxMin')[1]){
        element.val(this.model.get('incomePercentageMaxMin')[1]);
      }
      
      element.val(parseFloat(element.val()) + '%');
      
      
      var incomePctPayment = parseFloat(element.val());
      this.model.set({ 'incomePctPayment': incomePctPayment });
    },
    handleInterestKeyUp: function(event){
      var element = $(event.target);
      element.val(element.val().replace(/[^0-9]+/g,""));
      
      
      //ensure the data is a number only
      element.val(element.val()).toNumber();
      
      
    },
    handleInterestLess: function(event){
      var interestElement = $(event.target).siblings('input');
      var interest   = parseFloat(interestElement.val());
      
      interest = (interest - 0.5);
      
      if(interest < 0){
        interest = 0.1;
      } else{
        
        interestElement.val(parseFloat(interest.toFixed(2) ));
      }
      
      this.handleInterestChange();
      return false;
    },
    handleInterestMore: function(event){
      var interestElement = $(event.target).siblings('input');
      var interest   = parseFloat(interestElement.val());
      
      interest = (interest + 0.5);
      
      
      if(interest > this.model.get('incomePercentageMaxMin')[0]){
        interest = (parseFloat(this.model.get('interestRate') * 100)).toFixed(1);
      } else {
        interestElement.val(parseFloat(interest.toFixed(2) ));
      }
      
      this.handleInterestChange();
      return false;
    },
    buildHeaderCopyHtml: function() {
      var initialPayment = $.commify(this.model.getMonthlyPayment(0).toFixed(2), {prefix: '$'});
      var finalMonth = this.model.getNumberOfYears() * 12 - 1;
      var finalPayment = $.commify(this.model.getMonthlyPayment(finalMonth).toFixed(2), {prefix: '$'});
      var headerCopy = "With this plan, your payments would start at <strong>" + initialPayment +  " per month</strong> and grow to <strong>" + finalPayment + " per month.</strong>";
      return headerCopy;
    } // END calculateHeaderCopy();
  }));

  return Views;

});