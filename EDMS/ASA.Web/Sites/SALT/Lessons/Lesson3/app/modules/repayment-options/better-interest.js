define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/modules/repayment-options/repayment-view-mixin"
],

function(app, Backbone, repaymentViewMixin) {
  
  var Views = {};

  Views.BetterInterest = Backbone.View.extend(
    _.extend({}, repaymentViewMixin,{
      template: "graph-modifiers/better-interest",
      id: "betterInterest",
      className: 'repayment-option',
      
      initialize: function(options) {
        //When overriding a mixin function we need to be sure to call base mixin code
        repaymentViewMixin.initialize.call(this, options);
        
        //Add custom event to base property
        
        //var newEvents = {'keyup #set-timeline': 'handleTimeLineKeyUp'};
        //_.extend(repaymentViewMixin.events, newEvents);
      /*

        this.parentView = options.viewsHolder.highChartsViews;
        this.viewsHolder = options.viewsHolder;
        this.bind('calculateAndDisplay', this.calculateAndDisplay);
        this.viewsHolder = options.viewsHolder;
        
*/
      },
      
      afterRender: function(){
        $('#better-interest').val( (parseFloat(this.model.getMonthlyInterestRate() * 12 * 100)).toFixed(1) + '%' );
      },
      events: {
        'click .update-graph': 'handleUpdateGraph',
        'click .compare': 'triggerCompareOverlay',
        'change #better-interest': 'handleInterestChange',
        'blur #better-interest': 'handleInterestChange',
        'click #interest-less': 'handleInterestLess',
        'click #interest-more': 'handleInterestMore',
        'keyup #better-interest': 'handleInterestKeyUp'
      },
      handleInterestChange: function(event){
        var element = $('#better-interest');
        
        
        if(parseFloat(element.val()) > (parseFloat(this.model.get('interestRate') * 100)).toFixed(1)){
          element.val('3.86');
        } else if(element.val() < 0.1){
          element.val('0.1');
        }
        
        element.val(parseFloat(element.val()) + '%');
        
        
        modifiedInterestRate = parseFloat(element.val()) * .01;
        this.model.set({modifiedInterestRate: modifiedInterestRate});
      },
      handleInterestKeyUp: function(event){
        var element = $(event.target);
        
        
        //ensure the data is a number only
        element.val(element.val()).toNumber();
        
        
      },
      handleInterestLess: function(event){
        var interestElement = $(event.target).siblings('input');
        var interest   = parseFloat(interestElement.val());
        
        interest = (interest - .5);
        
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
        
        interest = (interest + .5);
        
        
        if(interest > (parseFloat(this.model.get('interestRate') * 100)).toFixed(1)){
          interest = (parseFloat(this.model.get('interestRate') * 100)).toFixed(1);
        } else {
          interestElement.val(parseFloat(interest.toFixed(2) ));
        }
        
        this.handleInterestChange();
        return false;
      },
      buildHeaderCopyHtml: function(){
        var numberOfYears = this.model.getNumberOfYears();
        var interestRate = (parseFloat(this.model.getMonthlyInterestRate() * 12 * 100)).toFixed(1);
        
        headerCopy = "With an interest rate of <strong>" + interestRate + "%,</strong> you'll have a more affordable monthly payment.";
        
        this.parentView.trigger('updateHeaderCopy', headerCopy);
      }
  }));

  return Views;

});