define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views 
  "lesson2/modules/one-time-expense/views",

  // Plugins
  "lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var OneTimeExpense = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  OneTimeExpense.Model = Backbone.Model.extend({
    url: app.serverUrl + '/Lesson2/OneTimeExpense/' ,
    validate: function(attrs, opts) {
      opts = opts || {};
      
      if(opts.init){
        //return;
        
      } else {
        var isValid = true;

        var error = {};
        var name = $.trim(attrs.name);
        var value = $.trim(attrs.value);
        
        error.name = false;
        error.value = false;
        
        if(name.length < 1){
          error.name = 'you need a name for this expense';
          isValid = false;
        } else{
          error.name = false;
        }

        if (value.length < 2) {
          error.value = 'you need a VALUE';
          isValid = false;
        } else {
          error.value = false;
        }

        if(isValid){
          // If the attributes are valid, don't return anything from validate http://backbonejs.org/#Model-validate
        } else {  
          return error;
        }
        
      }
    },
    modelValidate: function(options){
      options = options || {};
      var modelValid = true;
      
      if (options.silent || !this.validate) return true;

      var error = this.validate(this.attributes, options);
      
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
    parse: function(resp,xhr) {
      if(resp!=null) {
        var oneTimeExpense = resp;
        oneTimeExpense.id = resp.OneTimeExpenseId;
        oneTimeExpense.name = resp.Name;
        oneTimeExpense.value = resp.Value;
        oneTimeExpense.month = resp.Month;
        return oneTimeExpense;
      }
    },
    updateForServer: function() {
			var updatedProps = {
        Value: this.get('value'),
        Name: this.get('name'),
        Month: this.get('month'),
        OneTimeExpenseId: this.get('id'),
      };
      this.set(updatedProps,{silent: true});
    }
    

  });

  OneTimeExpense.OneTimeExpenseList = Backbone.Collection.extend({
    model: OneTimeExpense.Model,
    localStorage: new Store("OneTimeExpenses-backbone"),
    url: app.serverUrl + '/Lesson2/OneTimeExpense',
    
    nextOrder: function(){
      if (!this.length) {
        return 1;
      }

      return this.last().get("iteration") + 1;
    },

    newAttributes: function() {

      var iteration = this.nextOrder();
      return {
        name: '',
        value: '',
        month: 0,
        iteration: iteration, 
       // id: 'purchase' + iteration
      };
    },
     parse: function(resp) {
      if( resp != null) {
        for(var i =0;i<resp.length;i++){
          //Need to increment since the number returned by next Order is based on the number of elements in the collection,
          // and at this point we havent added the new element
          resp[i].iteration = this.nextOrder() + i;
        }
        return resp;
      }
    }

  });


  // Attach the Views sub-module into this module.
  OneTimeExpense.Views = Views;

  // Required, return the module for AMD compliance
  return OneTimeExpense;

});
