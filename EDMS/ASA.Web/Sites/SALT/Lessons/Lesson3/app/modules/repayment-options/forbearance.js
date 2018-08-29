define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.Forbearance = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
      
    template: "graph-modifiers/forbearance",
    id: "forbearance",
    className: 'repayment-option',

    events: {
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay',
      'click .qualify': 'handleQualifyOverlay',
    },
    afterRender: function(){
      var thisView = this;
      //mark default value (or pull from model if its already ben set)
      $('#forbearanceMonths').val(this.model.get('forbearanceMonths'));
      
      
      //apply dropkick
      $('#forbearanceMonths').dropkick({change: function(value, label){
        thisView.updateForbearanceMonthsProperty(value,label);
      }});
    },
    handleUpdateGraph: function(){
      this.updateForbearanceMonthsProperty();
      this.calculateAndDisplay();
      return false;
    },
    buildHeaderCopyHtml: function(){
      var headerCopy = "After putting your loans in forbearance for <strong>" + this.model.get('forbearanceMonths')  + " months</strong>, you'll pay off your loans in <strong>" + this.model.getNumberOfYears() + " years.</strong>";
      
      //If you pay <strong>" + $.commify(parseInt(this.model.get('monthlyPayment')), { prefix:'$' }) + "</strong> each month, you'll pay off your loan in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    },
    updateForbearanceMonthsProperty: function(){
      var newValue = parseInt($('#forbearanceMonths').val());
      this.model.set({'forbearanceMonths': newValue});
    }
  }));
  
  return Views;

});