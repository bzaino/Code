define([
  "lesson2/app",
  "lesson2/modules/header",
  "lesson2/modules/bar-graph",
  "lesson2/modules/graph-modifiers",
  "lesson2/modules/footer",
  "lesson2/modules/new-recurring-expense",
  "lesson2/modules/imported-recurring-expense",

  // Libs
  "backbone",
  "lesson2/plugins/asa-plugins"
],

function(app, header, barGraph, graphModifiers, footer, NewRecurringExpense,ImportedRecurringExpense, Backbone, asaPlugins) {

  var Views = {};
  var viewsHolder = {};


  //step 2 content view
  Views.step2Content = Backbone.View.extend({
    template: 'content/step2',
    className: 'step2-content',
    afterRender: function() {
      var hasImported = (this.recurringCollection.importedLength > 0) ? true : false;

      // Only show the "hey look, we imported your expenses message, if they are in fact imported"
      if( hasImported == true && app.user.get("Lesson2Step") < 2 ) {
        $(".info.hidden").removeClass("hidden");
      }

      if( !hasImported && this.recurringCollection.length === 0 ) {
        $(".add-another").trigger("click");
      }
    },
    initialize: function(options) {
      this.importedCollection = options.importedCollection;
      this.recurringCollection = options.recurringCollection;
      this.contentModel = options.model;
      this.barGraph = options.barGraph;
      viewsHolder.content = this;

      //FIND OUT IF WE CAN IMPORT
      //IMPORT ALL

      //IF NOT IMPORTING THEN CREATE THE INITIAL NEW RECURRING EXPENSE
      //in this instance, we are  forcing an import
      //create new recurring collection

      //this.recurringCollection.create(this.recurringCollection.newAttributes(),{init: true});
      this.insertView("#recurring-expenses",new NewRecurringExpense.Views.NewRecurringExpenseList({
        collection: this.recurringCollection
      }));

      /**
       * create imported expenses
       * should be pulled from server
       */
      if(!this.model.attributes.hasImported){
        this.model.set({hasImported: true});
      }

      //let the recurringCollection know how many imported models there are
      //this is used to maintain a consistent tab index
      this.recurringCollection.importedLength = this.importedCollection.length;

      /**
       * create imported expenses collection
       */
      if( app.user.get("Lesson2Step") < 2 ) {
        this.insertView("#former-expenses", new ImportedRecurringExpense.Views.ImportedRecurringExpenseList({
          collection: this.importedCollection,
          viewsHolder: viewsHolder
        }));
      }

      this.bind('updateBarGraphExpenses', this.updateBarGraphExpenses);

      this.activeStep = app.router.activeStep;
      this.preloaded = (this.recurringCollection.length > 0 || this.importedCollection.length > 0);

      app.wt.trigger('lesson:step:start', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:    this.activeStep,
          preloaded: this.preloaded
        }
      });
    },

    events: {
      'click .add-another': 'insertNewRecurringExpense',
      'keyup .new-recurring-name': 'handleRecurringNameChange',
      'focus .new-recurring-name': 'handleRecurringNameFocus',
      'keyup .new-recurring-value': 'handleRecurringValueChange',
      'click #recurring-expenses .btn-remove': 'removeNewRecurringExpense'
    },

    handleRecurringNameFocus: function(event){
      app.router.recurringExpensesAutocomplete();
    },

    handleRecurringNameChange: function(event) {
      var newRecurringExpenseID = parseInt($(event.target).parents('div').attr('data-iteration'));

      var singleRecurringExpense = this.recurringCollection.where({iteration: newRecurringExpenseID});

      singleRecurringExpense[0].set({ name: $(event.target).val() },{silent: true});
      var currentInputID = $("input:focus").attr("id");
      var cleanInput = $('#'+ currentInputID).val().replace(/[^A-Za-z0-9 %]+/g, "");
      $('#'+ currentInputID).val(cleanInput);

    },

    handleRecurringValueChange: function(event) {
      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      var newRecurringExpenseID = parseInt($(event.target).parents('div').attr('data-iteration'));
      var cleanInput = Number($(event.target).val().replace(/[^0-9\.-]+/g,"")); // Unescaped '-'.
      var singleRecurringExpense = this.recurringCollection.where({iteration: newRecurringExpenseID});

      singleRecurringExpense[0].set({ value: cleanInput },{silent: true});

      this.updateBarGraphExpenses();

    },

    removeNewRecurringExpense: function(event) {
      var newRecurringExpenseID = parseInt($(event.target).parents('div').attr('data-iteration'));
      var singleRecurringExpense = this.recurringCollection.where({iteration: newRecurringExpenseID});

      //destroy eliminates the element from local storage
      //by calling backbone.sync
      singleRecurringExpense[0].destroy();
      this.recurringCollection.remove(singleRecurringExpense[0]);

      this.updateBarGraphExpenses();

      return false;
    },

    insertNewRecurringExpense: function(event) {
      var newModel = this.recurringCollection.add(this.recurringCollection.newAttributes(),{init: true});
      return false;
    },

    updateBarGraphExpenses: function() {
      var sumRecurringValues = 0;
      //Add all recurring inputs
      $('.new-recurring-value').each(function(){
        var recurringValue = Number($(this).val().replace(/[^0-9\.-]+/g,""));
        sumRecurringValues += recurringValue;
      });

      //Add all imported inputs
      var sumImportedValues = 0;
      if( app.user.get("Lesson2Step") < 2 ) {
        $('.imported-value').each(function(){
          var importedValue = Number($(this).val().replace(/[^0-9\.-]+/g,""));
          sumImportedValues += importedValue;
        });
      }

      var totalSum = sumRecurringValues + sumImportedValues;

      //set barGraph
      this.barGraph.set({expenses: totalSum});
      //trigger bargraph view handleBarChange func
      viewsHolder.barGraph.trigger('handleBarChange');

      //udpate footer value
      if(totalSum >= 1){
        $('#total .value').text('$'+totalSum);
      } else{
        $('#total .value').text('$0');
      }

      //Set total collection sum
      this.contentModel.set({ collectionSum: totalSum },{silent: true});

    }

  });

  //Header view
  Views.step2Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 2, stepName: 'Recurring Expenses'});
      this.insertView(new header.Views.Header({model: hdr}));
    }
  });

  //right column graph view
  Views.step2BarGraph = Backbone.View.extend({
    initialize: function(options) {
      this.barGraph = options.barGraph;
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        model: this.barGraph,
        viewsHolder: viewsHolder
      }));
    }
  });

  Views.step2GraphModifiers = Backbone.View.extend({
    initialize: function() {
      this.insertView(new graphModifiers.Views.GraphModifiers({
        viewsHolder:viewsHolder
      }));
    }
  });

  //footer view
  Views.step2Footer = Backbone.View.extend({
    initialize: function(options) {
      this.recurringCollection = options.recurringCollection;
      this.importedCollection = options.importedCollection;
      this.barGraph = options.barGraph;
      viewsHolder.content = this;
      this.contentModel = options.model;
      var ftr = new footer.Model({amount: 0, description: 'recurring expenses', subtitle: 'Are you making any one-time purchases?', nextButton: 'Keep Going'});
      this.insertView(new footer.Views.Footer({model: ftr}));
    },

    events:{
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    afterRender: function() {
      this.getData();
    },

    getData: function() {

      //Add all imported values
      var sumImportedValues = 0;
      if( app.user.get("Lesson2Step") < 2 ) {
        for(i=0; i < this.importedCollection.length; i++ ) {
           sumImportedValues += this.importedCollection.models[i].attributes.value;
        }
      }

      //Add all recurring values
      var sumRecurringValues = 0;
      for(i=0; i < this.recurringCollection.length; i++ ) {
         sumRecurringValues += this.recurringCollection.models[i].attributes.value;
      }

      var totalSum = parseInt(sumRecurringValues) + parseInt(sumImportedValues);

      this.contentModel.set({ collectionSum: totalSum },{silent: true});

      if(totalSum >= 1){
        $('#total .value').text('$'+totalSum);
      } else {
        $('#total .value').text('$0');
      }

      //set barGraph
      this.barGraph.set({expenses: totalSum});
    },

    handleNextButton: function(event) {
      var recurringCollectionValid  = true;
      var importedCollectionValid   = true;

      /**
      	imported Collection validation
      */
      this.importedCollection.each(function (element, index, list) {
          if (!element.modelValidate(Views.Step2ImportedValidationLogic)) {
              importedCollectionValid = false;
              //display error message
              $('.error-msg').fadeIn();
          }
      });

      if( app.user.get("Lesson2Step") < 2 ) {
        if (importedCollectionValid) {
            //hide error message
            $('.error-msg').fadeOut();

            //If all imported expenses are validated, then add them to RecurringCollection
            var thisRecurringCollection = this.recurringCollection;
            this.importedCollection.each(function (element, index, list) {
                thisRecurringCollection.add(element);
            });
        }
      }

      /**
      	recurring Collection validation
      */
      this.recurringCollection.each(function (element, index, list) {
          if (!element.modelValidate(Views.Step2ValidationLogic)) {
              recurringCollectionValid = false;
              //display error message
              $('.error-msg').fadeIn();
          }
      });

      if (recurringCollectionValid) {
          //hide error message
          $('.error-msg').fadeOut();

					$.ajax({
            url: app.serverUrl + '/Lesson2/RecurringExpense/',
            type: 'delete',
            async: false
          });

          this.recurringCollection.each(function (element, index, list) {
          element.save();
        });
      }

      if(importedCollectionValid == true && recurringCollectionValid == true){
        this.contentModel.save();
        this.barGraph.save();

        if( app.user.get('Lesson2Step') < 2 ) {
          app.user.set('Lesson2Step', 2);
        }

        app.user.save();

        app.wt.trigger('lesson:step:complete', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:    Views.step2Content.activeStep,
            preloaded: Views.step2Content.preloaded
          }
        });

        app.router.globalNavigation(null, 'next');
      }

      return false;
    }, //END handleNextButton();
    handlePrevButton: function(event){
      event.preventDefault();

      app.router.globalNavigation(null, 'prev');
      return false;
    }
  });

  Views.Step2ValidationLogic = {
    error:function(model, error){
      var ele = $('div[data-iteration=' + model.get('iteration') + ']');
      if (error.name) {
        ele.find('.new-recurring-name').addClass('error');
      } else {
        ele.find('.new-recurring-name').removeClass('error');
      }
      if (error.value) {
        ele.find('.new-recurring-value').addClass('error');
      } else {
        ele.find('.new-recurring-value').removeClass('error');
      }

    },
    success: function(model, error){
      var ele = $('div[data-iteration=' + model.get('iteration') + ']');

      if(!error) {
        ele.find('.new-recurring-name').removeClass('error');
        ele.find('.new-recurring-value').removeClass('error');
      }

    }
  };

  Views.Step2ImportedValidationLogic = {
    error:function(model, error){
      var ele = $('.imported-recurring-expense[data-iteration=' + model.get('iteration') + ']');

      if (error.value) {
        ele.find('.imported-value').addClass('error');
      } else {
        ele.find('.imported-value').removeClass('error');
      }

    },
    success: function(model, error){
      var ele = $('.imported-recurring-expense[data-iteration=' + model.get('iteration') + ']');

      if(!error) {
        ele.find('.imported-value').removeClass('error');
      }

    }
  };

  return Views;

});


