define([
  'lesson2/app',

// Libs
  'backbone'
],

function (app, Backbone) {

  var Views = {};
  var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1
  var date = new Date();
  var monthOffset = date.getMonth() + 1;

  Views.OneTimeExpense = Backbone.View.extend({
    template: 'content/one-time-expense',
    className: 'wrapper',

    initialize: function () {
      this.model.on("remove", function () {
        this.remove();
      }, this);
    },

    serialize: function () {
      return {
        name: this.model.get('name'),
        value: this.model.get('value'),
        month: this.model.get('month'),
        order: this.model.get('iteration')
      };
    }

  });

  Views.OneTimeExpenseList = Backbone.View.extend({
    className: 'question',
    beforeRender: function () {
      this.collection.each(function (oneTimeExpense) {
        this.insertView(new Views.OneTimeExpense({

          model: oneTimeExpense,
          afterRender: function () {


            var ele = $('.one-time-expense[data-iteration=' + this.model.get('iteration') + ']');

            if (this.model.collection.length == 3) {
              $('.add-another').hide();
            } else {
              $('.add-another').show();
            }

            ele.find('a.btn-remove').css('display', 'inline-block');

            //remove the remove button from the 1st element every time the collection is rendered
            //$('.one-time-expense[data-iteration=1]').find('a.btn-remove').css('display','none');

            //repopulate input fields
            ele.find('.one-time-name').val(this.model.get('name'));
            ele.find('.one-time-value').val(this.model.get('value')).formatCurrency({ roundToDecimalPlace: 0 });
            
            if (this.model.get('month') == 0) {
              var nextMonth = (monthOffset + 1) % 12;
              this.model.set({ month: nextMonth }, { silent: true });
            }
            ele.find('.one-time-month').val(this.model.get('month'));

          }
        }));
      }, this);


    },
    initialize: function (options) {
      this.content = options.viewsHolder.content;
      this.collection.on('add', function (oneTimeExpense, opts) {
        this.render();
      }, this);

      this.collection.on('remove', function () {
        this.render();
      }, this);
    },

    afterRender: function () {
      //placeholder text IE
      $('input[type=text]').placeHolder();

      this.prepareSelectElements();

    },
    prepareSelectElements: function () {
      var dropDown = this.$el.find('.dropdown-large');
      var theCollection = this.collection;

      /**
      	
      loop through existing select elements
      match the model based on the data-iteration attribute
      */
      $.each(dropDown, function (key, value) {

        var oneTimeExpense = $(value).parents('.one-time-expense').attr('data-iteration');
        oneTimeExpense = parseInt(oneTimeExpense);

        var theModel = theCollection.where({ iteration: oneTimeExpense });


        /**
        	
        loop through the months and append them to each select element
        making sure to apply a "selected" attribute when matching the model's selected value
        it will default to zero if there hasn't been a selection made
        	
        */
        for (var i = 0; i < 12; i++) {
          var monthKey = (i + monthOffset) % 12;
          if (monthKey == (theModel[0].get('month') - 1)) {
            thisMonth = '<option value="' + (monthKey + 1) + '" selected="selected">' + monthsFull[monthKey] + '</option>';
          } else {
            thisMonth = '<option value="' + (monthKey + 1) + '">' + monthsFull[monthKey] + '</option>';
          }


          $(dropDown[key]).append(thisMonth);

        };



      });


      /**
      	
      apply the dropkick plugin
      	
      */
      var elView = this;
      this.$el.find('.dropdown-large').dropkick({ theme: 'large', change: function (value, label) {
        //call another function 
        var oneTimeExpense = this.parents('.one-time-expense');
        elView.bind('MonthChange', elView.handleMonthChange);
        elView.trigger('MonthChange', oneTimeExpense, value);
      }});

    },
    events: {
      'click .btn-remove': 'removeOneTimeExpense'
    },

    handleMonthChange: function (oneTimeExpense, value) {
      var oneTimeExpenseID = parseInt(oneTimeExpense.attr('data-iteration'));

      var singleExpense = this.collection.where({ iteration: oneTimeExpenseID });
      singleExpense[0].set({ month: value }, { silent: true });
    },


    removeOneTimeExpense: function (event) {
      event.preventDefault();

      var oneTimeExpenseID = parseInt($(event.target).parents('.one-time-expense').attr('data-iteration'));
      var singleExpense = this.collection.where({ iteration: oneTimeExpenseID });

      //destory local storage
      singleExpense[0].destroy();
      this.collection.remove(singleExpense[0]);

      this.content.trigger('updateBarGraphExpenses');
      return false;
    }

  });

  return Views;

});
