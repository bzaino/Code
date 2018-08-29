define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins,repaymentViewMixin) {
  
  var Views = {};
 
  Views.PayFaster = Backbone.View.extend(
    _.extend({}, repaymentViewMixin, {
    template: 'graph-modifiers/pay-faster',
    id: 'payFaster',
    className: 'repayment-option',

    initialize: function(options){
      //When overriding a mixin function we need to be sure to call base mixin code
      repaymentViewMixin.initialize.call(this, options);
    },
    events:{
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay',
      'keyup #pay-more': 'handlePayMoreKeyUp'
    },
    afterRender: function(){
      //set default value
      $('#pay-more').val(this.model.get('additionalMonthlyPayment')).formatCurrency({ roundToDecimalPlace: 0 });
    },
    buildHeaderCopyHtml: function(){
      var headerCopy = "If you pay <strong>" + $.commify(this.model.get('additionalMonthlyPayment'), { prefix:'$' }) + " more</strong> each month, you'll pay off your loan in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    }, 
    handlePayMoreKeyUp: function(event){
      var value = $(event.target).val();      
      value = value.replace(/[^0-9]+/g,"");
      $(event.target).val(value).formatCurrency({ roundToDecimalPlace: 0 });
      this.updateAddlMonthlyPaymentProperty(value);
    },
    updateAddlMonthlyPaymentProperty: function(cleanedValue){
      var additionalMonthlyPayment = +cleanedValue;
      this.model.set({'additionalMonthlyPayment': additionalMonthlyPayment});
    }
  }));

  return Views;

});