define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/standard-repayment",
  "lesson3/modules/favorites",
  "lesson3/modules/compare",
  "lesson3/modules/footer",
  "lesson3/modules/highcharts-graph",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, repaymentModifier, favorites, compare, footer, highChartsGraph, Backbone, asaPlugins) {
  var Views = {};
  var viewsHolder = {};
  var repaymentOptions = {};
  var selectRepaymentOptions = [{name:'Standard repayment', div:'standardRepayment'}];

  //step 2 content view
  Views.step2Content = Backbone.View.extend({
    template: 'content/step2',
    className: 'step2-content',

    initialize: function(options) {
      var highChartsViews = this.insertView(new highChartsGraph.Views.HighChartsGraph());
      viewsHolder.highChartsViews = highChartsViews;
      viewsHolder.content = this;

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
  Views.step2Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 2, stepName: 'Real cost over time'});
      this.insertView(new header.Views.Header({
        model: hdr,
        viewsHolder: viewsHolder,
        repaymentModels: selectRepaymentOptions
      }));
    }
  });

  Views.step2BarGraph = Backbone.View.extend({
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

  Views.step2GraphModifiers = Backbone.View.extend({
    initialize: function() {
      repaymentOptions.standardRepayment = this.insertView(new repaymentModifier.Views.StandardRepayment({
        template: "graph-modifiers/standard-repayment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.standardRepayment
      }));
    }
  });

  Views.step2Favorites = Backbone.View.extend({
    initialize: function() {
      var fav = new favorites.Model({selectRepaymentOptions: [{name:'Standard repayment', div:'standardRepayment'}]});
      this.insertView(new favorites.Views.Favorites({
        model: fav,
        collection: app.router.collections,
        viewsHolder: viewsHolder,
        repaymentOptions: repaymentOptions
      }));
    }
  });

  Views.step2Compare = Backbone.View.extend({
    initialize: function() {
      viewsHolder.compareView = this.insertView(new compare.Views.Compare({
        viewsHolder: viewsHolder
      }));
    }
  });

  //footer view
  Views.step2Footer = Backbone.View.extend({
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
      this.saveRepaymentOptions();
      app.router.setLessonStep(2);
      app.router.globalNavigation(null, 'next');

      if( app.user.get('Lesson3Step') < 2 ) {
        app.user.set('Lesson3Step', 2);
      }

      app.user.save();

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

    saveRepaymentOptions: function(){
      try {
        app.router.repaymentModels.standardRepayment.save();
      }
      catch(e) {
      }
    },

    handlePrevButton: function(event){
      event.preventDefault();

      app.router.globalNavigation(null, 'prev');

      return false;
    }
  });

  return Views;
});
