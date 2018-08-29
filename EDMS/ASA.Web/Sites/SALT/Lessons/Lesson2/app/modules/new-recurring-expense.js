define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views 
  "lesson2/modules/new-recurring-expense/views",

  // Plugins
  "lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var NewRecurringExpense = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  NewRecurringExpense.Model = Backbone.Model.extend({

    url: app.serverUrl + '/Lesson2/RecurringExpense/',
    validate: function(attrs, opts) {
    
      
      opts = opts || {};
      
      if(opts.init){
        //return;
        
      } else{
        var isValid = true;

        var error = {};
        var name = $.trim(attrs.name);
        var value = $.trim(attrs.value);

        if (name.length < 1) {
          error.name = 'you need a name for this expense';
          isValid = false;
        } else {
          error.name = false;
        }

        if (value.length < 1 || value == 0) {
          error.value = 'you need a VALUE';
          isValid = false;
        } else {
          error.value = false;
        }

        if (isValid) {
          // If the attributes are valid, don't return anything from validate http://backbonejs.org/#Model-validate
        } else {
          return error;
        }
        
      }
    },
    modelValidate: function(options){
      options = options || {};
      
      if (options.silent || !this.validate) return true;

      var error = this.validate(this.attributes, options);
      var modelValid = true;


      if(error) {
        modelValid = false;
        if (options && options.error) {
          options.error(this, error, options);
        } else {
          this.trigger('error', this, error, options);
        }      
      } else {
        if (options && options.success) {
          options.success(this, error, options);
        } else {
          this.trigger('success', this, options);
        }      
      }

      return modelValid;

    },

    parse: function(resp,xhr){     
      if(resp!= null) {
        var recurringExpense = resp;
        recurringExpense.id = resp.RecurringExpenseId;
        recurringExpense.name = resp.Name;
        recurringExpense.value = resp.Value;

        return recurringExpense;
      }
    },
    updateForServer: function(){
      var updatedProps = {
        Value: this.get('value'),
        Name: this.get('name'),
        RecurringExpenseId: this.get('id')
      };
      this.set(updatedProps,{silent: true});
    }
    

  });

  NewRecurringExpense.NewRecurringExpenseList = Backbone.Collection.extend({
    model: NewRecurringExpense.Model,
    localStorage: new Store("NewRecurringExpense-backbone"),
    url: app.serverUrl + '/Lesson2/RecurringExpense',
    initialize: function(){

    },

    nextOrder: function(){
      if (!this.length) {
        return this.importedLength + 1;
      }

      return this.last().get("iteration") + 1;
    },

    newAttributes: function() {
      var iteration = this.nextOrder();
      return {
        name: '',
        value: '',
        iteration: iteration
        //id: 'r' + iteration
      };
    },
    parse: function(resp) {
      if(resp != null) {
        for(var i=0;i<resp.length;i++){
          //Need to increment since the number returned by next Order is based on the number of elements in the collection,
          // and at this point we havent added the new element
          resp[i].iteration = this.nextOrder() + i;
        }
        return resp;
      }
    }
  });

  // Attach the Views sub-module into this module.
  NewRecurringExpense.Views = Views;

  // Required, return the module for AMD compliance
  return NewRecurringExpense;

});
