define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.ExtendedRepayment = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
    template: 'graph-modifiers/extended-repayment',
    id: 'extendedRepayment',
    className: 'repayment-option',

    events: {
      'click .compare': 'triggerCompareOverlay',
      'click .update-graph': 'handleUpdateGraph',
      'keyup #years': 'handleYearsKeyUp'
    },
    afterRender: function(){
      var thisView = this;
      //mark default value (or pull from model if its already ben set)
      $('#years').val(this.model.getNumberOfYears());
      
      //apply dropkick
      $('#years').dropkick({change: function(value, label){
        thisView.updateYears(value,label);
      }});
    },
    updateYears: function(value, label){
      this.model.set({'modifiedYears': value});
    },
    handleYearsKeyUp: function(event){
      this.updateModifiedYearsProperty();
    },
    updateModifiedYearsProperty: function(){
      var extendedYears = $('#years').val();
      var extendedYears = Number(extendedYears.replace(/[^0-9\.-]+/g,""));

      this.model.set({'modifiedYears': extendedYears});
    },
      
    buildHeaderCopyHtml: function() {
      var headerCopy = "If you pay over a period of <strong>" + this.model.getNumberOfYears() + "</strong> years, you'll pay <strong>" + $.commify(this.model.getMonthlyPayment(0).toFixed(2), { prefix:'$' }) +  " every month.</strong>";
      return headerCopy;
    } // END calculateHeaderCopy();
  }));

  return Views;

});