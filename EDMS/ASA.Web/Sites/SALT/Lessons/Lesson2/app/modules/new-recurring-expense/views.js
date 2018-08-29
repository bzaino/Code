define([
  'lesson2/app',

  // Libs
  'backbone'
],

function(app, Backbone) {
  
  var Views = {};

  Views.NewRecurringExpense = Backbone.View.extend({
    template: 'content/new-recurring-expense',
    tagName: 'li',
    afterRender: function(){

    },
    
    initialize: function(options) {
      this.model.on("remove", function() {
        this.remove();
      }, this);
    },

    serialize: function() {
      return {
        name: this.model.get('name'),
        value: this.model.get('value'),
        order: this.model.get('iteration')
      };
    }

  });
  
  Views.NewRecurringExpenseList = Backbone.View.extend({
    tagName: 'ul',
    afterRender: function(){
      //placeholder text IE
      $('input[type=text]').placeHolder();
      //app.router.recurringExpensesAutocomplete();
    },
    beforeRender: function() {
      this.collection.each(function(newRecurringExpense) {
        this.insertView(new Views.NewRecurringExpense({

          model: newRecurringExpense,
          afterRender: function() {
            var ele = $('div[data-iteration=' + this.model.get('iteration') + ']');

            //re-populate input fields
            ele.find('.new-recurring-name').val(this.model.get('name'));
            ele.find('.new-recurring-value').val(this.model.get('value')).formatCurrency({ roundToDecimalPlace: 0 });

          }
        }));
      }, this);
      
      
    },
    initialize: function() {
      this.collection.on('add', function(newRecurringExpense,opts) {
        this.render();
      }, this);

      this.collection.on('remove', function() {
        this.render();
      }, this);
    }
    

  });

  return Views;

});
