define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/pay-faster",
  "lesson3/modules/set-timeline",
  "lesson3/modules/better-interest",
  "lesson3/modules/favorites",
  "lesson3/modules/compare",
  "lesson3/modules/footer",
  "lesson3/modules/highcharts-graph",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, payFasterModifier, setTimelineModifier, betterInterestModifier, favorites, compare, footer, highChartsGraph, Backbone, asaPlugins) {
  var Views = {};
  var viewsHolder = {};
  var repaymentOptions = {};
  var selectRepaymentOptions = [{name:'Pay more each month', div:'payFaster'},{name: 'Set your own timeline', div: 'setTimeline'}];

  //step 3 content view
  Views.step3Content = Backbone.View.extend({
    template: 'content/step3',
    className: 'step3-content',

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

    },

    events: function() {

    }
  });

  //Header view
  Views.step3Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 2, stepName: 'Real cost over time'});
      this.insertView(new header.Views.Header({
        model: hdr,
        repaymentModels: selectRepaymentOptions
      }));
    }
  });

  Views.step3BarGraph = Backbone.View.extend({
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

  Views.step3GraphModifiers = Backbone.View.extend({
    initialize: function() {
      /**
        insert all sub modifiers into this view
      */
      repaymentOptions.payFaster = this.insertView(new payFasterModifier.Views.PayFaster({
        template: "graph-modifiers/pay-faster",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.payFaster
      }));

      repaymentOptions.setTimeline = this.insertView(new setTimelineModifier.Views.SetTimeline({
        template: "graph-modifiers/set-timeline",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.setTimeline
      }));
      //SWD-3312 QC6977
      // repaymentOptions.betterInterest = this.insertView(new betterInterestModifier.Views.BetterInterest({
      //   template: "graph-modifiers/better-interest",
      //   viewsHolder: viewsHolder,
      //   model: app.router.repaymentModels.betterInterest
      // }));
    }
  });

  Views.step3Favorites = Backbone.View.extend({
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

  Views.step3Compare = Backbone.View.extend({
    initialize: function() {
      viewsHolder.compareView = this.insertView(new compare.Views.Compare({
        viewsHolder: viewsHolder
      }));
    }
  });

  //footer view
  Views.step3Footer = Backbone.View.extend({
    initialize: function(options) {
      this.parentView = options.model;

      var ftr = new footer.Model({amount: 0, description: '', subtitle: '', nextButton: "Keep Going", showSum: false});
      this.thisView = this.insertView(new footer.Views.Footer({model: ftr}));
    },

    events: {
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    handleNextButton: function(event){
      this.parentView.save();
      this.thisView.saveRepaymentOptions(selectRepaymentOptions);
      app.router.setLessonStep(3);
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
