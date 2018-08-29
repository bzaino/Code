define([
  "lesson2/app",

// Libs
  "backbone"
],

function (app, Backbone) {

  var Views = {};
  var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1
  var monthsShort = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];

  var date = new Date();
  var monthOffset = date.getMonth() + 1;

  Views.GraphModifiers = Backbone.View.extend({
    template: "content/left-content",
    className: 'content',

    initialize: function (options) {
      this.parentView = options.viewsHolder.content;
      this.activeStep = app.router.activeStep;

    },
    events: {
      'change input[type=checkbox]': 'handleCheckboxChange',
      'change #increase-payment': 'increasePayment',
      'keyup #increase-payment-value': 'increasePaymentValueKeyUp',
      'change #ignore-recurring': 'ignoreRecurringExpenses',
      'change #ignore-one-times': 'ignoreOneTimes',
      'change #increase-payment-value': 'increasePaymentValue',
      'change #extra-payment': 'extraPayment',
      'change #extra-payment-value': 'extraPaymentValue',
      'keyup #extra-payment-value': 'extraPaymentValueKeyUp',
      'change #change-interest': 'changeInterest',
      'change #modified-interest': 'changeInterest',
      'click #interest-less': 'handleInterestLess',
      'click #interest-more': 'handleInterestMore',
      'keyup #modified-interest': 'modifiedInterest',
      'click #update': 'handleUpdateGraph'
    },
    handleUpdateGraph: function (event) {
      this.parentView.trigger('updateGraph');

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'update-graph'
        },
        step: {
          number: this.activeStep
        }
      });

      return false;
    },
    afterRender: function () {
      this.getData();
      //placeholder text IE
      $('input[type=text]').placeHolder();
      $('input[type=checkbox]').uniform();
      var parentView = this.parentView; //this is used by the change function for dropkick, scope changes

      var dropDown = this.$el.find('.dropdown');
      var self = this;

      /**
      populate the one time payment month dropdown
      */
      var gmModel = this.model;
      $.each(monthsShort, function (key, value) {
        //Calculate the key entry of the month we're iterating 
        //through based on the current month.
        var monthKey = ((key + monthOffset) % 12);
        if (gmModel && monthKey === (gmModel.get('extraPaymentMonth') - 1)) {
          thisMonth = '<option selected="selected" value="' + (monthKey + 1) + '">' + monthsShort[monthKey] + '</option>';
        } else {
          thisMonth = '<option value="' + (monthKey + 1) + '">' + monthsShort[monthKey] + '</option>';
        }
        dropDown.append(thisMonth);

      });




      this.$el.find('.dropdown').dropkick({ change: function (value, label) {

        parentView.trigger('handleExtraPaymentMonthChange', value);
        parentView.trigger('handleExtraPayment', { target: $('#extra-payment') });

        app.wt.trigger("lesson:graph:optionChange", {
          graph: {
            option: 'extra-payment-month'
          },
          step: {
            number: self.activeStep
          }
        });
      }
      });


    },
    getData: function () {

      if (!this.model) {
        return;
      }
      this.parentView.initGraph();
      if (this.model.get('cashForRecurringExpenses') == true) {
        $('#ignore-recurring').attr('checked', true);
        this.ignoreRecurringExpenses({ target: $('#ignore-recurring') });
      } else {
        $('#ignore-recurring').attr('checked', false);
      }

      if (this.model.get('cashForOneTimePurchases') == true) {
        var oneTimes = $('#ignore-one-times');
        oneTimes.attr('checked', true);
        this.ignoreOneTimes({ target: oneTimes });
      } else {
        $('#ignore-one-times').attr('checked', false);
      }
      //increase-payment

      if (this.model.get('increaseMonthlyPaymentChecked') == true) {
        var increasePayment = $('#increase-payment');
        var increasePaymentInput = $('#increase-payment-value');
        increasePayment.attr('checked', true);
        this.handleCheckboxChange({ target: increasePayment });
        increasePaymentInput.val(this.model.get('increaseMonthlyPayment'));
        this.increasePaymentValueKeyUp({ target: increasePaymentInput });
        this.increasePayment({ target: increasePayment });
      } else {
        $('#increase-payment').attr('checked', false);
      }

      //change-interest
      if (this.model.get('lowerInterestRateChecked') == true) {
        var interestCheckBox = $('#change-interest');
        var interestInput = $('#modified-interest');
        interestCheckBox.attr('checked', true);
        this.handleCheckboxChange({ target: interestCheckBox });
        var interestFormatted = (this.model.get('lowerInterestRate') * 100).toFixed(3) + "%";
        interestInput.val(interestFormatted);
        this.modifiedInterest({ target: interestInput });
      } else {
        $('#change-interest').attr('checked', false);
      }

      //extra-payment
      if (this.model.get('extraPaymentChecked') == true) {
        var extraPaymentCheckBox = $('#extra-payment');
        var extraPaymentInput = $('#extra-payment-value');
        var extraPaymentMonth = $('#extra-payment-month');
        extraPaymentCheckBox.attr('checked', true);
        this.handleCheckboxChange({ target: extraPaymentCheckBox });
        extraPaymentInput.val(this.model.get('extraPaymentAmount'));
        extraPaymentMonth.val(this.model.get('extraPaymentMonth'));
        this.extraPaymentValueKeyUp({ target: extraPaymentInput });
        this.parentView.trigger('handleExtraPaymentMonthChange', this.model.get('extraPaymentMonth'));
        this.extraPayment({ target: extraPaymentCheckBox });

      } else {
        $('#extra-payment').attr('checked', false);
      }

    },
    handleCheckboxChange: function (event) {
      var element = $(event.target);
      var parent = element.parents('.section');


      if (element.is(':checked')) {
        parent.find('.hidden-modifiers').slideDown('fast');
      } else {
        parent.find('.hidden-modifiers').slideUp('fast');
      }


    },
    increasePayment: function (event) {
      //checkbox toggle
      this.parentView.trigger('handleIncreasePayment', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'increase-payment'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    ignoreRecurringExpenses: function (event) {
      //checkbox toggle
      this.parentView.trigger('handleIgnoreRecurringExpenses', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'ignore-recurring'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    increasePaymentValue: function (event) {
      //input
      this.parentView.trigger('handleIncreasePaymentValueChange', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'increase-payment-value'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    increasePaymentValueKeyUp: function (event) {
      //input
      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

      this.parentView.trigger('handleIncreasePaymentValueChange', event);
    },
    ignoreOneTimes: function (event) {
      // checkbox toggle
      this.parentView.trigger('handleIgnoreOneTimeExpenses', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'ignore-one-times'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    extraPayment: function (event) {
      // checkbox toggle
      this.parentView.trigger('handleExtraPayment', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'extra-payment'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    extraPaymentValue: function (event) {
      // input

      this.parentView.trigger('handleExtraPaymentValueChange', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'extra-payment-value'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    extraPaymentValueKeyUp: function (event) {

      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      this.parentView.trigger('handleExtraPaymentValueKeyUp', event);
      this.parentView.trigger('handleExtraPayment', { target: $('#extra-payment') });

    },
    changeInterest: function (event) {
      // checkbox toggle
      this.parentView.trigger('handleChangeInterest', event);

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'change-interest'
        },
        step: {
          number: this.activeStep
        }
      });
    },
    modifiedInterest: function (event) {
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

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'interest-less'
        },
        step: {
          number: this.activeStep
        }
      });

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

      app.wt.trigger("lesson:graph:optionChange", {
        graph: {
          option: 'interest-more'
        },
        step: {
          number: this.activeStep
        }
      });

      return false;
    }


  });

  return Views;

});
