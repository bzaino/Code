define([
  "lesson3/app",

  // Libs
  "backbone"
],

function(app, Backbone) {
  
  var Views = {};
  var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1
  var monthsShort = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];    

  var date = new Date();
  var monthOffset = date.getMonth() + 1;
  
  Views.GraphModifiers = Backbone.View.extend({
    className: 'content',
        
    initialize: function(options) {
      this.parentView = options.viewsHolder.content;

      

      
    },
    events: {
      'change input[type=checkbox]': 'handleCheckboxChange',
      'change #increase-payment': 'increasePayment',
      'keyup #increase-payment-value': 'increasePaymentValueKeyUp',
      'change #increase-payment-value': 'increasePaymentValue',
      'change #extra-payment': 'extraPayment',
      //'change #extra-payment-value': 'extraPaymentValue',
      'keyup #extra-payment-value': 'extraPaymentValueKeyUp',
      'change #change-interest': 'changeInterest',
      'change #modified-interest': 'changeInterest',
      'click #interest-less': 'handleInterestLess',
      'click #interest-more': 'handleInterestMore',
      'keyup #modified-interest': 'modifiedInterest',
      'click #update': 'handleUpdateGraph'
    },

    handleUpdateGraph: function(event){  
      this.parentView.trigger('updateGraph');
      return false;
    },
    afterRender: function(){
      
      //placeholder text IE
      $('input[type=text]').placeHolder();
      $('input[type=checkbox]').uniform();
      var parentView = this.parentView; //this is used by the change function for dropkick, scope changes
      
      var dropDown = this.$el.find('.dropdown');
      
      
      this.$el.find('.dropdown').dropkick({theme: 'large',change: function(value, label) {
      
        parentView.trigger('handleExtraPaymentMonthChange', value);

      }});
    },
    handleCheckboxChange: function(event){
      var element = $(event.target);
      var parent  = element.parents('.section');
      

      if(element.is(':checked')){
        parent.find('.hidden-modifiers').slideDown('fast');
      } else{
        parent.find('.hidden-modifiers').slideUp('fast');
      }
      
      
    },
    increasePayment: function(event){
      //checkbox toggle
      this.parentView.trigger('handleIncreasePayment', event);
    },
    increasePaymentValue: function(event){
      //input
      this.parentView.trigger('handleIncreasePaymentValueChange', event);
    },
    increasePaymentValueKeyUp: function(event){
      //input
      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      
      this.parentView.trigger('handleIncreasePaymentValueChange', event);
    },
    extraPayment: function(event){
      // checkbox toggle
      this.parentView.trigger('handleExtraPayment', event);
    },
    extraPaymentValue: function(event){
      // input
      
      this.parentView.trigger('handleExtraPaymentValueChange', event);
    },
    extraPaymentValueKeyUp: function(event){
      
      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      this.parentView.trigger('handleExtraPaymentValueKeyUp', event);
    },
    changeInterest: function(event){
      // checkbox toggle
      this.parentView.trigger('handleChangeInterest', event);
    },
    modifiedInterest: function(event){
      // input key up / change
      this.parentView.trigger('handleModifiedInterestChange', event);
    },
    handleInterestLess: function (event) {
      var interestElement = $(event.target).siblings('input');
      var value = interestElement.val();
      var cleanValue = value.replace(/[^0-9\.-]+/g, "");
      if (!Number(cleanValue)) {
        cleanValue = 0;
      }
      var interest = parseFloat(cleanValue);

      interest = (interest - .5);

      if (interest < 0) {
        interest = 0;
      } else if (interest > 100) {
        interest = 100;
      } 
     
      interestElement.val(interest + '%');

      this.parentView.trigger('handleModifiedInterest', event);
      return false;
    },
    handleInterestMore: function (event) {
      var interestElement = $(event.target).siblings('input');
      var value = interestElement.val();
      var cleanValue = value.replace(/[^0-9\.-]+/g, "");
      if (!Number(cleanValue)) {
        cleanValue = 0;
      }
      var interest = parseFloat(cleanValue);

      interest = (interest + .5);

      if (interest < 0) {
        interest = 0;
      } else if (interest > 100) {
        interest = 100;
      } 
        
      interestElement.val(interest + '%');

      this.parentView.trigger('handleModifiedInterest', event);
      return false;
    }


  });

  return Views;

});
