define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.GraduatedRepayment = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
    template: 'graph-modifiers/graduated-repayment',
    id: 'graduatedRepayment',
    className: 'repayment-option',
    events: {
      'click .compare': 'triggerCompareOverlay'
    },
    buildHeaderCopyHtml: function() {
      var headerCopy = "Your initial monthly payment is <strong>" + $.commify(this.model.getMonthlyPayment(0).toFixed(2), { prefix:'$' }) + "</strong>.  Those payments increase later, allowing you to still pay off your loan in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    } // END calculateHeaderCopy();
  }));

  return Views;

});