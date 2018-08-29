define([
  "lesson2/app",
  "lesson2/modules/header",
  "lesson2/modules/bar-graph",
  "lesson2/modules/graph-modifiers",
  "lesson2/modules/footer",
  "lesson2/modules/one-time-expense",

// Libs
  "backbone",
  "lesson2/plugins/asa-plugins"
],

function (app, header, barGraph, graphModifiers, footer, OneTimeExpense, Backbone, asaPlugins) {

  var Views = {};
  var viewsHolder = {};

  //step 3 content view
  Views.step3Content = Backbone.View.extend({
    template: 'content/step3',
    className: 'step3-content',

    initialize: function (options) {
      this.collection = options.collection;
      this.barGraph = options.barGraph;
      viewsHolder.content = this;
      this.bind('updateBarGraphExpenses', this.updateBarGraphExpenses);

      this.activeStep = app.router.activeStep;
      this.preloaded = (this.collection.length > 0);

      if (this.collection.models.length < 1) {
        //insert first expense item on page load
        this.collection.add(this.collection.newAttributes(), { init: true });
      }

      var oneTimeExpenseList = new OneTimeExpense.Views.OneTimeExpenseList({
        collection: this.collection,
        viewsHolder: viewsHolder
      });
      oneTimeExpenseList.delegateEvents();
      this.insertView(".question", oneTimeExpenseList);

      app.wt.trigger('lesson:step:start', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:    this.activeStep,
          preloaded: this.preloaded
        }
      });

    },

    afterRender: function () {

    },

    events: {
      'keyup .currency': 'moneyChange',
      'click .add-another': 'insertOneTimeExpense',
      'keyup .one-time-name': 'handleNameChange',
      'keyup .one-time-value': 'handleValueChange'
    },

    handleNameChange: function (event) {
      var oneTimeExpenseID = parseInt($(event.target).parents('.one-time-expense').attr('data-iteration'));

      var singleExpense = this.collection.where({ iteration: oneTimeExpenseID });

      singleExpense[0].set({ name: $(event.target).val() }, { silent: true });

      var currentInputID = $("input:focus").attr("id");
      var cleanInput = $('#'+ currentInputID).val().replace(/[^A-Za-z0-9 %]+/g, "");
      $('#'+ currentInputID).val(cleanInput);
    },

    handleValueChange: function (event) {
      var oneTimeExpenseID = parseInt($(event.target).parents('.one-time-expense').attr('data-iteration'));

      var cleanInput = Number($(event.target).val().replace(/[^0-9\.-]+/g, "")); // Unescaped '-'.

      var singleExpense = this.collection.where({ iteration: oneTimeExpenseID });
      singleExpense[0].set({ value: cleanInput }, { silent: true });

      this.updateBarGraphExpenses();
    },

    handleMonthChange: function (oneTimeExpense, value) {
      var oneTimeExpenseID = parseInt(oneTimeExpense.attr('data-iteration'));

      var singleExpense = this.collection.where({ iteration: oneTimeExpenseID });
      singleExpense[0].set({ month: value }, { silent: true });
    },


    moneyChange: function () {
      this.$el.find('.currency').toNumber().formatCurrency({ roundToDecimalPlace: 0 });
    },

    insertOneTimeExpense: function (event) {
      event.preventDefault();

      if (this.collection.length < 3) {
        var newModel = this.collection.create(this.collection.newAttributes(), { init: true });
      }

      return false;
    },

    updateBarGraphExpenses: function () {
      var sumOneTimeValues = 0;
      //Add all recurring inputs
      $('.one-time-value').each(function () {
        var oneTimeValue = Number($(this).val().replace(/[^0-9\.-]+/g, ""));
        sumOneTimeValues += oneTimeValue;
      });

      //set barGraph
      this.barGraph.set({ purchases: sumOneTimeValues });
      //trigger bargraph view handleBarChange func
      viewsHolder.barGraph.trigger('handleBarChange');

      //udpate footer value
      if (sumOneTimeValues >= 1) {
        $('#total .value').text('$' + sumOneTimeValues);
      } else {
        $('#total .value').text('$0');
      }
    }

  });

  //Header view
  Views.step3Header = Backbone.View.extend({
    initialize: function () {
      var hdr = new header.Model({ currentStep: 3, stepName: 'One-time purchases' });
      this.insertView(new header.Views.Header({ model: hdr }));
    }
  });

  //right column graph view
  Views.step3BarGraph = Backbone.View.extend({
    initialize: function (options) {
      this.barGraph = options.barGraph;
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        model: this.barGraph,
        viewsHolder: viewsHolder
      }));
    }
  });

  Views.step3GraphModifiers = Backbone.View.extend({
    initialize: function () {
      this.insertView(new graphModifiers.Views.GraphModifiers({
        viewsHolder: viewsHolder
      }));
    }
  });

  //footer view
  Views.step3Footer = Backbone.View.extend({
    initialize: function (options) {
      this.contentModel = options.model;
      this.collection = options.collection;
      this.barGraph = options.barGraph;

      var ftr = new footer.Model({ amount: 0, description: 'total one-time purchases', subtitle: 'How much do you typically pay down?', nextButton: 'Keep Going' });
      this.insertView(new footer.Views.Footer({ model: ftr }));
    },
    events: {
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    afterRender: function () {
      this.getData();
    },

    getData: function () {
      //Add all imported values
      var sumOneTimeValues = 0;
      for (i = 0; i < this.collection.length; i++) {
        sumOneTimeValues += this.collection.models[i].attributes.value;
      }

      var totalSum = sumOneTimeValues;

      if (totalSum >= 1) {
        $('#total .value').text('$' + totalSum);
      } else {
        $('#total .value').text('$0');
      }
    },

    handleNextButton: function (event) {
      event.preventDefault();

      var collectionValid = true;

      //check if only one
      var purchasesCount = $('.one-time-expense').length;
      var purchasesName = $('.one-time-expense').first().find('.one-time-name').val(); //$('#one-time-name-1').val();
      var purchasesValue = $('.one-time-expense').first().find('.one-time-value').val(); //$('#one-time-value-1').val();
      var cleanPurchasesValue = purchasesValue ? Number(purchasesValue.replace(/[^0-9\.-]+/g, "")) : 0;

      if (purchasesCount > 1 || (purchasesName && purchasesName.length > 0) || cleanPurchasesValue > 0) {

        this.collection.each(function (element, index, list) {
          if (!element.modelValidate(Views.Step3ValidationLogic)) {
            collectionValid = false;
            //display error message
            $('.error-msg').fadeIn();
          }
        });

        if (collectionValid) {
          //hide error message
          $('.error-msg').fadeOut();
          this.barGraph.save();
          this.contentModel.save();

					$.ajax({
            url: app.serverUrl + '/Lesson2/OneTimeExpense/',
            type: 'delete',
            async: false
          });

          this.collection.each(function (element, index, list) {
            //remove empty models from collection
            var name = $.trim(element.attributes.name);
            /**
            if the name attribute is empty,
            then we should assume the user does not want to add this expense
            and we should remove it from the collection before saving and moving on
            */

            element.save();

            if (name.length == 0) {
              element.collection.remove(element);
            }


          });

          if( app.user.get('Lesson2Step') < 3 ) {
            app.user.set('Lesson2Step', 3);
          }

          app.user.save();

          app.wt.trigger('lesson:step:complete', {
            user: Backbone.Asa.User.userGuid,
            time: new Date(),
            step: {
              number:    Views.step3Content.activeStep,
              preloaded: Views.step3Content.preloaded
            }
          });

          app.router.globalNavigation(null, 'next');
        }
      } else {
        if( app.user.get('Lesson2Step') < 3 ) {
          app.user.set('Lesson2Step', 3);
        }

        app.user.save();

        app.wt.trigger('lesson:step:complete', {
          user: Backbone.Asa.User.userGuid,
          time: new Date(),
          step: {
            number:    Views.step3Content.activeStep,
            preloaded: Views.step3Content.preloaded
          }
        });

        app.router.globalNavigation(null, 'next');
      }

      return false;
    },
    handlePrevButton: function (event) {
      event.preventDefault();

      app.router.globalNavigation(null, 'prev');
      return false;
    }
  });

  Views.Step3ValidationLogic = {
    error: function (model, error) {
      var ele = $('.one-time-expense[data-iteration=' + model.get('iteration') + ']');
      if (error.name) {
        ele.find('.one-time-name').addClass('error');
      } else {
        ele.find('.one-time-name').removeClass('error');
      }
      if (error.value) {
        ele.find('.one-time-value').addClass('error');
      } else {
        ele.find('.one-time-value').removeClass('error');
      }

    },
    success: function (model, error) {
      var ele = $('.one-time-expense[data-iteration=' + model.get('iteration') + ']');

      if (!error) {
        ele.find('.one-time-name').removeClass('error');
        ele.find('.one-time-value').removeClass('error');
      }

    }
  };

  return Views;

});
