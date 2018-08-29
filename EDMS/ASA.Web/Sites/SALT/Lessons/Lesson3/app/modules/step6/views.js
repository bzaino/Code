define([
  "lesson3/app",
  "lesson3/modules/header",
  "lesson3/modules/bar-graph",
  "lesson3/modules/favorites",
  "lesson3/modules/footer",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, header, barGraph, favorites, footer, Backbone, asaPlugins) {
  var Views = {};
  var viewsHolder = {};
  var repaymentOptions = {};

  //step 6 content view
  Views.step6Content = Backbone.View.extend({
    template: 'content/step6',
    className: 'step6-content',

    initialize: function(options) {
      this.favorites = app.router.collections.favorites;

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

    afterRender: function() {
      this.drawFavoriteChart();
    },

    drawFavoriteChart: function() {
      var favoriteLength = this.favorites.length;

      //check to see if any favorites were selected
      if(favoriteLength < 1){
        $('.table-container').hide();
        $('<div class="no-favorites"><h1>Whoops</h1><p>There’s nothing to see here because you didn’t bookmark anything along the way. You can just click "I’m Done" to finish the lesson and go to the summary.</p></div>').insertAfter('.description');
      }

      //make columns
      this.createColumn(favoriteLength);

      for(i=0;i<favoriteLength;i++){
        var k = i+1;
        //populate data
        this.populateListViewData(this.favorites.models[i].attributes.valueID, k);
      }
    },

    populateListViewData: function(value, iteration){
      var baseListViewElement = $('.step6-content');
      var repaymentModel = app.router.repaymentModels[value];
      var title = repaymentModel.get('name');
      var yearlyRepaymentPoints = repaymentModel.get('yearlyRepaymentPoints');
      var year0 = yearlyRepaymentPoints[0];
      var yearLast;
      var i;

      //loop through the repayment points and determine where the currentBalance hits zero
      //this will give us the true last point
      //we have to fill the array with null points due to a highcharts requirement
      for(i = 0; i < yearlyRepaymentPoints.length; i++){
        if(yearlyRepaymentPoints[i].currentBalance === 0){
          yearLast = yearlyRepaymentPoints[i];

          break;
        }
      }

      var principal = repaymentModel.get('totalBalance');

      //Matching the logic used in high-charts graph view, to ensure values match
      //default totalInterestPaid to 0 if not defined in yearLast repaymentPoint
      var totalInterestPaid = typeof yearLast.interestAccrued === 'undefined' ? 0 : yearLast.interestAccrued;
      var totalRepaid = $.commify((principal + totalInterestPaid).toFixed(2), { prefix:'$' });
      totalInterestPaid = $.commify(totalInterestPaid.toFixed(2), { prefix:'$' });
      var totalRepaymentPeriod = app.router.repaymentModels[value].getNumberOfYears() + ' years';
      var totalInterestAccumulated = 'missing';
      var monthlyPayment = $.commify(year0.monthlyPayment.toFixed(2), { prefix:'$' });
      var monthlyPayment1to2, monthlyPayment3to5, monthlyPayment6to10, monthlyPayment11to15;

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears1to2() == 'number'){
        monthlyPayment1to2 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears1to2()).toFixed(2), {prefix:'$'});
      } else{
        monthlyPayment1to2 = app.router.repaymentModels[value].calculateAveragePaymentYears1to2();
      }

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears3to5() == 'number'){
        monthlyPayment3to5 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears3to5()).toFixed(2), {prefix:'$'});
      } else{
        monthlyPayment3to5 = app.router.repaymentModels[value].calculateAveragePaymentYears3to5();
      }

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears6to10() == 'number'){
        monthlyPayment6to10 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears6to10()).toFixed(2), {prefix:'$'});
      } else{
        monthlyPayment6to10 = app.router.repaymentModels[value].calculateAveragePaymentYears6to10();
      }

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears11to15() == 'number'){
        monthlyPayment11to15 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears11to15()).toFixed(2), {prefix:'$'});
      } else{
        monthlyPayment11to15 = app.router.repaymentModels[value].calculateAveragePaymentYears11to15();
      }

      var gracePeriod = app.router.repaymentModels[value].get('gracePeriod');
      var deferment = app.router.repaymentModels[value].get('deferment');
      var forberance = app.router.repaymentModels[value].get('forbearance');

      var aggregateValues = [title, totalRepaid, totalRepaymentPeriod, totalInterestPaid, monthlyPayment, monthlyPayment1to2, monthlyPayment3to5, monthlyPayment6to10, monthlyPayment11to15, gracePeriod, deferment, forberance];

      for (i=0; i <= 11; i++) {
        for (var row = i; row <= i; row++) {
          for (var col = iteration; col <= iteration; col++) {
            var tableCell = $('.table').children().children()[row].children[col];
            $(tableCell).find('div').html(aggregateValues[i]);
          }
        }
      }
    },

    createColumn: function(favoriteLength) {
      var tableLength = favoriteLength;

      for(var z=1;z <= tableLength; z++) {
        for(var i=0; i <= 12; i++) {
          for (var row = i; row <= i; row++) {
            var currentRow = $('.table').children().children()[row];
            if(i === 0) {
              $(currentRow).append('<div class="th"><div></div></div>');
            } else {
              $(currentRow).append('<div class="td"><div></div></div>');
            }
          }
        }
      }
    }
  });

  //header view
  Views.step6Header = Backbone.View.extend({
    initialize: function() {
      var hdr = new header.Model({currentStep: 6, stepName: 'Review your favorites'});
      this.insertView(new header.Views.Header({model: hdr}));
    }
  });

  Views.step6BarGraph = Backbone.View.extend({});

  Views.step6GraphModifiers = Backbone.View.extend({});

  Views.step6Favorites = Backbone.View.extend({});

  Views.step6Compare = Backbone.View.extend({});

  //footer view
  Views.step6Footer = Backbone.View.extend({
    initialize: function(options) {
      this.parentView = options.model;

      var ftr = new footer.Model({amount: 0, showDescription: false, subtitle: '', nextButton: "I'M DONE", showSum: false});
      this.insertView(new footer.Views.Footer({model: ftr}));
    },

    events: {
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    handleNextButton: function(event){
      this.parentView.save();
      app.router.setLessonStep(6);
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

      app.wt.trigger('lesson:overall:complete', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        totalTime: ((Date.now() - Backbone.Asa.User.startTime) / 60000)
      });

        SALT.trigger('content:todo:completed');

      // See if user is logged in vs. anonymous
      if( Backbone.Asa.User.individualId === null || typeof Backbone.Asa.User.individualId === "undefined" ) {
        event.preventDefault();
        app.router.showLoginPrompt( $(event.target).attr("href") );
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
