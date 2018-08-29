define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/favorites",
  "lesson3/modules/compare",
  "lesson3/modules/footer",
  "lesson3/modules/highcharts-graph",
  "lesson3/modules/in-school-deferment",
  "lesson3/modules/hardship-deferment",
  "lesson3/modules/forbearance",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, favorites, compare, footer, highChartsGraph, inSchoolDeferment, hardShipDeferment, forbearance, Backbone, asaPlugins) {
  var Views = {};
  var viewsHolder = {};
  var repaymentOptions = {};
  var selectRepaymentOptions = [{name: 'In-school deferment', div: 'inSchoolDeferment'}, {name: 'Unemployment deferment', div:'hardShipDeferment'}, {name: 'Forbearance', div: 'forbearance'}];

  //step 5 content view
  Views.step5Content = Backbone.View.extend({
    template: 'content/step5',
    className: 'step5-content',

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
  Views.step5Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 5, stepName: 'Deferment & Forbearance'});
      this.insertView(new header.Views.Header({
        model: hdr,
        repaymentModels: selectRepaymentOptions
      }));
    }
  });

  Views.step5BarGraph = Backbone.View.extend({
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

  Views.step5GraphModifiers = Backbone.View.extend({
    initialize: function() {
      /**
        insert all sub modifiers into this view
      */

      repaymentOptions.inSchoolDeferment = this.insertView(new inSchoolDeferment.Views.InSchoolDeferment({
        template: "graph-modifiers/in-school-deferment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.inSchoolDeferment
      }));

      repaymentOptions.hardShipDeferment = this.insertView(new hardShipDeferment.Views.HardShipDeferment({
        template: "graph-modifiers/hardship-deferment",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.hardShipDeferment
      }));

      repaymentOptions.forbearance = this.insertView(new forbearance.Views.Forbearance({
        template: "graph-modifiers/forbearance",
        viewsHolder: viewsHolder,
        model: app.router.repaymentModels.forbearance
      }));
    }
  });

  Views.step5Favorites = Backbone.View.extend({
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

  Views.step5Compare = Backbone.View.extend({
    initialize: function() {
      viewsHolder.compareView = this.insertView(new compare.Views.Compare({
        viewsHolder: viewsHolder
      }));
    }
  });

  //footer view
  Views.step5Footer = Backbone.View.extend({
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
      app.router.setLessonStep(5);
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
