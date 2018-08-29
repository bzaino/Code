define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, asaPlugins, repaymentViewMixin) {
  
  var Views = {};
 
  Views.SetTimeline = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
      
    template: "graph-modifiers/set-timeline",
    id: "setTimeline",
    className: 'repayment-option',

    initialize: function(options){
      //When overriding a mixin function we need to be sure to call base mixin code
      repaymentViewMixin.initialize.call(this, options);
    },
    events: {
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay'
    },
    afterRender: function(){
      var thisView = this;
      //mark default value (or pull from model if its already ben set)
      $('#set-timeline').val(this.model.getNumberOfYears());
      
      
      //apply dropkick
      $('#set-timeline').dropkick({change: function(value, label){
        thisView.updateTimelineProperty(value,label);
      }});
    },
    buildHeaderCopyHtml: function(){
      var headerCopy = "If you pay <strong>" + $.commify(this.model.getMonthlyPayment(0), { prefix:'$' }) + "</strong> each month, you'll pay off your loan in <strong>" + this.model.getNumberOfYears() +  " years.</strong>";
      return headerCopy;
    },
    updateTimelineProperty: function(value, label){
      this.model.set({'years': value});
    }
  }));
  
  return Views;

});