define([
  "lesson3/app",

  // Libs
  "backbone",
  'salt'
],

function(app, Backbone, SALT) {
  
  var Views = {};

 

  Views.Footer = Backbone.View.extend({
    template: "nav/footer",
    className: 'wrapper',
        
    initialize: function() {
      var _this = this;

      SALT.on('totalBalanceFooter:update', function () {
        _this.setTotalBalanceField();
      });
    },
    afterRender: function(){
      //Previous Button
      if(this.model.attributes.noPrev){
        $('.left-button').hide();
      }
      
      //Footer Sum
      if(this.model.attributes.showSum === false){
        $('#footer .left #total').hide();
        
      }
      
      //Footer description
      if(this.model.attributes.showDescription === false){
        $('#footer #up-next').hide();
        
      }
      
      //up next
      var upNextName = app.router.activeStep;
      if(app.router.activeStep < navOptions.length){
        $('#footer #up-next .second').text(navOptions[upNextName].name);  
      }
      
      $('#footer #total .value').text(app.router.collections.loanType.models[0].get('sum')).formatCurrency({ roundToDecimalPlace: 0 });
      
      
    },
    setTotalBalanceField: function() {
      $('#footer #total .value').text(app.router.collections.loanType.models[0].get('sum')).formatCurrency({ roundToDecimalPlace: 0 });
    },

    saveRepaymentOptions: function(repaymentOptions){
      for(i = 0; i < repaymentOptions.length; i++){
        repaymentOption = repaymentOptions[i].div;
        app.router.repaymentModels[repaymentOption].save();
      }
    },
    serialize: function() {
      return {
        //amount: this.model.get("amount").formatCurrency({ roundToDecimalPlace: 0 }),
        nextButton: this.model.get("nextButton"),
        next: this.model.get("next")
      };
    }
  });

  return Views;

});