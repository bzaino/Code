define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};

  Views.HardShipDeferment = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
      
    template: "graph-modifiers/hardship-deferment",
    id: "hardShipDeferment",
    className: 'repayment-option',

    events: {
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay',
      'click .qualify': 'handleQualifyOverlay',
      'keyup #pay-more': 'handlePayMoreValidation'
    },
    afterRender: function(){
      var thisView = this;
      //set default value
      $('#deferment-hardship-months').val(this.model.get('defermentMonths'));
      //$('#pay-more').val(this.model.get('additionalMonthlyPayment')).formatCurrency({ roundToDecimalPlace: 0 });

      //apply dropkick
      $('#deferment-hardship-months').dropkick({ change: function (value, label) {
        thisView.updateDefermentMonthsProperty();
      }});
    },
    handleUpdateGraph: function(){
      this.updateDefermentMonthsProperty();
      //this.updateAdditionalPaymentProperty();
      this.calculateAndDisplay();
      return false;
    },
    buildHeaderCopyHtml: function(){
      var headerCopy = "If you defer payments for <strong>" + this.model.get('defermentMonths') + "</strong> months, you’ll pay off your loans in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    },
    updateDefermentMonthsProperty: function(){
      var newValue =$('#deferment-hardship-months').val();
      this.model.set({'defermentMonths': newValue});
    },
    updateAdditionalPaymentProperty: function(){
      var newValue = $('#pay-more').val();
      newValue = newValue.replace(/[^0-9]/g, "");
      this.model.set({'additionalMonthlyPayment': newValue});
    },
    handlePayMoreValidation: function(event){
      var value = $(event.target).val();
      $(event.target).val(value).formatCurrency({ roundToDecimalPlace: 0 });
    }
    
  }));
  
  return Views;

});