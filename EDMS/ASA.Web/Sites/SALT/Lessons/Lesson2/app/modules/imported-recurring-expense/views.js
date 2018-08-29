define([
  'lesson2/app',

  // Libs
  'backbone'
],

function(app, Backbone) {
  
  var Views = {};

  Views.ImportedRecurringExpense = Backbone.View.extend({
    template: 'content/imported-recurring-expense',
    tagName: 'div',
        
    initialize: function() {

      this.model.on("remove", function() {
        this.remove();
      }, this);
      
      
    },
    afterRender: function(){
      this.$el.find('.imported-value').formatCurrency({ roundToDecimalPlace: 0 });
    },

    serialize: function() {
      return {
        name: this.model.get('name'),
        value: this.model.get('value'),
        order: this.model.get('iteration')
      };
    }

  });
  
  Views.ImportedRecurringExpenseList = Backbone.View.extend({
    tagName: 'div',
    id: 'imported',

    beforeRender: function() {
      this.collection.each(function(importedRecurringExpense) {   
        this.insertView(new Views.ImportedRecurringExpense({
          model: importedRecurringExpense,
        }));
      }, this);

    },
    initialize: function(options) {
      this.parentView = options.viewsHolder.content;

      this.collection.on('add', function(importedRecurringExpense,opts) {
        this.render();
      }, this);

      this.collection.on('remove', function() {
        this.render();
      }, this);
    },

    events: {
      'keyup .imported-value': 'handleValueChange',
      'click .btn-remove': 'removeImportedRecurringExpense'
    },


    handleValueChange: function(event) {

      var importedRecurringExpenseID = parseInt($(event.target).parents('.imported-recurring-expense').attr('data-iteration'));
      
      $(event.target).formatCurrency({ roundToDecimalPlace: 0 });
      
      
      var cleanInput = Number($(event.target).val().replace(/[^0-9\.-]+/g,"")); // Unescaped '-'.
      
      var singleImportedRecurringExpense = this.collection.where({iteration: importedRecurringExpenseID});
      
      singleImportedRecurringExpense[0].set({ value: cleanInput },{silent: true});  

      //trigger barGraph change
      this.parentView.trigger('updateBarGraphExpenses');
    },

    removeImportedRecurringExpense: function(event) {
      event.preventDefault();

      var importedRecurringExpenseID = parseInt($(event.target).parents('.imported-recurring-expense').attr('data-iteration'));
      var singleImportedRecurringExpense = this.collection.where({iteration: importedRecurringExpenseID});
      
      singleImportedRecurringExpense[0].destroy();
      this.collection.remove(singleImportedRecurringExpense[0]);

      //trigger barGraph change
      this.parentView.trigger('updateBarGraphExpenses');
      return false;
    }

  });

  return Views;

});
