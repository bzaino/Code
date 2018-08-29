define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.IncomeBased = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
    template: 'graph-modifiers/income-based-repayment',
    id: 'incomeBased',
    className: 'repayment-option',

    events: {
      'click .compare': 'triggerCompareOverlay',
      'click .update-graph': 'handleUpdateGraph'
    },
    afterRender: function(){
      //pre-set existing values
      $('#ib-salary').val(this.model.get('income')).formatCurrency({ roundToDecimalPlace: 0 });
      $('#dependents').val(this.model.get('dependents'));
    },
    handleUpdateGraph: function(){
      this.calculateAndDisplay();
      return false;
    },
    buildHeaderCopyHtml: function() {
      var headerCopy = this.model.getHeaderCopy();
      return headerCopy;
    } // END calculateHeaderCopy();
  }));

  return Views;

});