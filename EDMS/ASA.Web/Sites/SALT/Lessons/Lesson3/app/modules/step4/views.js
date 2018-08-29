define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/favorites",
  "lesson3/modules/compare",
  "lesson3/modules/footer",
  "lesson3/modules/highcharts-graph",
  "lesson3/modules/graduated-repayment",
  "lesson3/modules/income-based-repayment",
  "lesson3/modules/extended-repayment",
  "lesson3/modules/income-sensitive-repayment",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, favorites, compare, footer, highChartsGraph, graduatedRepayment, incomeBased, extendedRepayment,incomeSensitive, Backbone, asaPlugins) {
  var Views = {};
  var viewsHolder = {};
  var repaymentOptions = {};
  var selectRepaymentOptions = [{name:'Graduated Payment', div:'graduatedRepayment'}, {name: 'Income-based', div: 'incomeBased'}, {name: 'Extended', div:'extendedRepayment'}, {name: 'Income-sensitive', div: 'incomeSensitive'}];

  //step 4 content view
  Views.step4Content = Backbone.View.extend({
    template: 'content/step4',
    className: 'step4-content',

    initialize: function(options) {
      var highChartsViews = this.insertView(new highChartsGraph.Views.HighChartsGraph());
      viewsHolder.highChartsViews = highChartsViews;

      this.activeStep = app.router.activeStep;
      this.preloaded = (this.collection.length > 0);

      app.wt.trigger('lesson:step:start', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:    this.activeStep,
          preloaded: this.preloaded
        }
      });

    }
  });

  //Header view
  Views.step4Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 2, stepName: 'Real cost over time'});
      this.insertView(new header.Views.Header({
        model: hdr,
        repaymentModels: selectRepaymentOptions
      }));
    }
  });

  Views.step4BarGraph = Backbone.View.extend({
    initialize: function(options) {
      this.barGraph = options.barGraph;
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        collection: options.collection,
        model: this.barGraph,
        viewsHolder: viewsHolder
      }));
    }
  });

  Views.step4GraphModifiers = Backbone.View.extend({
    initialize: function() {
      /**
        insert all sub modifiers into this view
      */

      repaymentOptions.graduatedRepayment = this.insertView(new graduatedRepayment.Views.GraduatedRepayment({
        template: "graph-modifiers/graduated-repayment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.graduatedRepayment
      }));

      repaymentOptions.incomeBased = this.insertView(new incomeBased.Views.IncomeBased({
        template: "graph-modifiers/income-based-repayment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.incomeBased
      }));

      repaymentOptions.extendedRepayment = this.insertView(new extendedRepayment.Views.ExtendedRepayment({
        template: "graph-modifiers/extended-repayment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.extendedRepayment
      }));

      repaymentOptions.incomeSensitive = this.insertView(new incomeSensitive.Views.IncomeSensitive({
        template: "graph-modifiers/income-sensitive",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.incomeSensitive
      }));

    }
  });

  Views.step4Favorites = Backbone.View.extend({
    initialize: function() {
      var fav = new favorites.Model({selectRepaymentOptions: selectRepaymentOptions});
      this.insertView(new favorites.Views.Favorites({
        model: fav,
        collection: app.router.collections,
        viewsHolder: viewsHolder,
        repaymentOptions: repaymentOptions
      }));
    }
  });

  Views.step4Compare = Backbone.View.extend({
    initialize: function() {
      viewsHolder.compareView = this.insertView(new compare.Views.Compare({
        viewsHolder: viewsHolder
      }));
    }
  });

  //footer view
  Views.step4Footer = Backbone.View.extend({
    initialize: function(options) {
      this.parentView = options.model;

      var ftr = new footer.Model({amount: 0, description: '', subtitle: '', nextButton: "Keep Going", showSum: false});
      this.thisView = this.insertView(new footer.Views.Footer({ model: ftr }));
    },

    events: {
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    handleNextButton: function(event){
      this.parentView.save();
      this.thisView.saveRepaymentOptions(selectRepaymentOptions);
      app.router.setLessonStep(4);
      app.router.globalNavigation(null, 'next');

      this.preloaded = (this.collection.length > 0);
      app.wt.trigger('lesson:step:complete', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:   app.user.get('Lesson3Step'),
          preloaded: this.preloaded
        }
      });

      return false;
    },

    handlePrevButton: function(event){
      event.preventDefault();

      app.router.globalNavigation(null, 'prev');

      return false;
    }
  });

  return Views;
});
