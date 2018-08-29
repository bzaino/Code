define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {

  var Views = {};

  Views.StandardRepayment = Backbone.View.extend(
    _.extend({}, repaymentViewMixin, {
      //Extends from repaymentViewMixin to inherit functionality common to all repayment-option views
    
    template: "graph-modifiers/standard-repayment",
    id: 'standardRepayment',
    className: 'repayment-option',
    events: {
      'click .compare': 'triggerCompareOverlay'
    },
    buildHeaderCopyHtml: function() {
      //This method must be overriden by all repayment-option views to populate the header
      var monthlyPayment = $.commify(this.model.getMonthlyPayment().toFixed(2), { prefix:'$' });
      var headerCopy = "With a payment of <strong>" + monthlyPayment + " per month</strong> you'll pay off your loan in <strong>" + this.model.getNumberOfYears() +  " year(s).</strong>";
      
      return headerCopy;
    }// END calculateHeaderCopy();

  }));
  
  

  return Views;
});
