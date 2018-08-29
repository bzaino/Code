define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, Backbone, asaPlugins/* ,addThis */) {

  var Views = {};

  Views.Compare = Backbone.View.extend({
    template: "graph-modifiers/compare",
    className: 'wrapper',

    initialize: function(options) {
      this.repaymentOptions = options.repaymentOptions;
      this.bind('handleCompareOverlay',this.handleCompareOverlay);
    },

    afterRender: function() {

    },

    events: {
      'click .compare': 'handleCompareOverlay'
    },

    handleCompareOverlay: function() {
      //open compare overlay
      $.fancybox.open({
            href : '#compare-overlay'
           },{
                autoSize    : false,
                modal: false,
                autoWidth: true,
                autoHeight: true,
                closeClick    : false,
                openEffect    : 'none',
                closeEffect    : 'none',
                padding: 0,
                afterLoad: function(){
                  $('a.ok').on('click', function(event){
                    $.fancybox.close(true);
                    return false;
                  });

                  $('a.print').on('click', function(event){
                    event.preventDefault();
                    window.print();
                    return false;
                  });

                  $('a.email').on('click', function(event){
                    event.preventDefault();
                    $(".addthis_button_email").trigger("click");
                  });
                },
                afterShow: function(){

                }
        });

      //Get current dropdown value
      var compareValue = $('#select-repayment-options').find('.dk_option_current a').attr('data-dk-dropdown-value');
      if(compareValue === 'standardRepayment') {
        this.populateListViewData(compareValue, 1);
      } else {
        var standardRepayment = 'standardRepayment';
        //populate data
        this.populateListViewData(standardRepayment, 1);
        this.populateListViewData(compareValue, 2);
      }
    },
    populateListViewData: function(value, iteration){
      var baseListViewElement = $('#compare-overlay');
      var title = app.router.repaymentModels[value].get('name');
      var yearlyRepaymentPoints = app.router.repaymentModels[value].get('yearlyRepaymentPoints');
      var year0 = yearlyRepaymentPoints[0];

      //var yearLast = yearlyRepaymentPoints[yearlyRepaymentPoints.length - 1];
      //loop through the repayment points and determine where the currentBalance hits zero
      //this will give us the true last point
      //we have to fill the array with null points due to a highcharts requirement
      for(var i = 0; i < yearlyRepaymentPoints.length; i++){
        if(yearlyRepaymentPoints[i].currentBalance == 0){
          var yearLast = yearlyRepaymentPoints[i];

          break;
        }

      }

      var principal = app.router.repaymentModels[value].get('totalBalance');

      var totalInterest = yearLast.interestAccrued;
      var totalRepaid = $.commify((principal + totalInterest).toFixed(2), { prefix:'$' });
      var totalRepaymentPeriod = app.router.repaymentModels[value].getNumberOfYears() + ' years';
      var totalInterestPaid = $.commify(totalInterest.toFixed(2), { prefix:'$' });
      var totalInterestAccumulated = 'missing';
      var monthlyPayment = $.commify(year0.monthlyPayment.toFixed(2), { prefix:'$' });

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears1to2() == 'number'){
        var monthlyPayment1to2 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears1to2()).toFixed(2), {prefix:'$'});
      } else{
        var monthlyPayment1to2 = app.router.repaymentModels[value].calculateAveragePaymentYears1to2();
      }

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears3to5() == 'number'){
        var monthlyPayment3to5 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears3to5()).toFixed(2), {prefix:'$'});
      } else{
        var monthlyPayment3to5 = app.router.repaymentModels[value].calculateAveragePaymentYears3to5();
      }



      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears6to10() == 'number'){
        var monthlyPayment6to10 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears6to10()).toFixed(2), {prefix:'$'});
      } else{
        var monthlyPayment6to10 = app.router.repaymentModels[value].calculateAveragePaymentYears6to10();
      }

      if(typeof app.router.repaymentModels[value].calculateAveragePaymentYears11to15() == 'number'){
        var monthlyPayment11to15 = $.commify((app.router.repaymentModels[value].calculateAveragePaymentYears11to15()).toFixed(2), {prefix:'$'});
      } else{
        var monthlyPayment11to15 = app.router.repaymentModels[value].calculateAveragePaymentYears11to15();
      }

      var gracePeriod = app.router.repaymentModels[value].get('gracePeriod');
      var deferment = app.router.repaymentModels[value].get('deferment');
      var forbearance = app.router.repaymentModels[value].get('forbearance');

      var aggregateValues = [title, totalRepaid, totalRepaymentPeriod, totalInterestPaid,  monthlyPayment, monthlyPayment1to2, monthlyPayment3to5, monthlyPayment6to10, monthlyPayment11to15, gracePeriod, deferment, forbearance];

      for(var i=0; i <= 11; i++) {
        for (var row = i; row <= i; row++) {
          for (var col = iteration; col <= iteration; col++) {
            var tableCell = $('.table').children().children()[row].children[col];
            $(tableCell).find('div').html(aggregateValues[i]);
          }
        }
      }
      this.populateSelect();
    },

    populateSelect: function() {
      var dropDown = $('#compare-overlay').find('.dropdown');
      var colTitles = [];
      var match = false;
      var repaymentModelsTitle = ['Select another plan'];
      var repaymentModelsID = ['selectAnotherPlan'];

      $('#compare-overlay .th').each(function (i, e) {
        if(!$(this).find('select').length) {
          colTitles.push($(e).text());
        }
      });

      $.each(app.router.repaymentModels, function(k,v){

          for (var i=0; i <= colTitles.length ;i++) {
            if(colTitles[i] === app.router.repaymentModels[k].attributes.name) {
              match = true;
              break;
            } else {
              match = false;
            }
          }

        if (match !== true) {
          repaymentModelsTitle.push(app.router.repaymentModels[k].attributes.name);
          repaymentModelsID.push(app.router.repaymentModels[k].attributes.referenceId);
        }
      });

      //remove old dropDown options & dropKick plugin
      $(dropDown).find('option').remove();
      $(dropDown).removeData('dropkick');
      $('#compare-overlay').find('.dk_container').remove();

      $.each(repaymentModelsTitle, function(key, value){
        var thisPlan = '<option value="' + repaymentModelsID[key] + '">' + repaymentModelsTitle[key] + '</option>';
        $(dropDown).append(thisPlan);
      });

      this.styleSelect();
    },

    styleSelect: function() {
      var elView = this;
      $('#compare-overlay').find('.dropdown').dropkick({theme: 'large', change: function(value, label){
        var name = $(this).attr('name');
        name = name.match(/\d/g);
        //call another function
        elView.selectAnotherPlanChange(value,label,name);
      }});
    },

    selectAnotherPlanChange: function(value, label, name){
      var tableLength = parseInt($('thead').find('th').length, 10) -1;
      var selectLength = parseInt($('table').find('select').length, 10);
      var col = parseInt(name,10) + 1;

      if(value != 'selectAnotherPlan') {
        this.populateListViewData(value, col);

        //NO NEW COLUMNS PENDING UX
        if(selectLength < 2 && col < 9) {
          //this.createColumn();
        }
      }

    },

    createColumn: function() {
      var tableLength = parseInt($('.thead').find('.th').length, 10) -1;

      for(var i=0; i <= 13; i++) {
        for (var row = i; row <= i; row++) {
          var currentRow = $('.table').children().children()[row];
          if(i === 0) {
            $(currentRow).append('<div class="th"><div><select class="dropdown" name="plan-selector-'+tableLength+'"></select></div></div>');
            this.populateSelect();
            this.styleSelect();
          } else {
            $(currentRow).append('<div class="td"><div>&nbsp;</div></div>');
          }
        }
      }
    },

    handlePrint: function(event) {
      // event.preventDefault();

      // return false;
    },

    handleEmail: function(event) {
      // $(event.target).preventDefault();
      // return false;
    }

  });

  return Views;

});
