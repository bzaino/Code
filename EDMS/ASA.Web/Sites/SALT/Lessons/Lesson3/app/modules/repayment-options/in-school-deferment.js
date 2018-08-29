define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};

  Views.InSchoolDeferment = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
      
    template: "graph-modifiers/in-school-deferment",
    id: "inSchoolDeferment",
    className: 'repayment-option',

    events: {
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay',
      'click .qualify': 'handleQualifyOverlay'
    },
    afterRender: function(){
      var thisView = this;
      //set default value
      
      $('#deferment-months').val(this.model.get('defermentMonths'));

      //apply dropkick
      $('#deferment-months').dropkick({change: function(value, label){
        thisView.updateDefermentMonthsProperty(value,label);
      }});
    },
    handleUpdateGraph: function(){
      this.updateDefermentMonthsProperty();
      this.calculateAndDisplay();
      return false;
    },
    updateDefermentMonthsProperty: function(){
      var value = $('#deferment-months').val();
      this.model.set({'defermentMonths': value});
    },
    buildHeaderCopyHtml: function(){
      var headerCopy = "If you defer payments for <strong>" + this.model.get('defermentMonths') + "</strong> months, you’ll pay off your loans in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    }
  }));
  
  return Views;

});