define([
  "lesson2/app",

  // Libs
  "backbone"
],

function(app, Backbone) {
  
  var Views = {};

  Views.BarGraph = Backbone.View.extend({
    template: "graph/bar-graph",
    className: 'wrapper',
    
    initialize: function(options){
      options.viewsHolder.barGraph = this;
      this.bind('handleBarChange',this.handleBarChange);
    },

    afterRender: function(){
      var onloadCheck = 1;
      //once loaded draw graph
      this.handleBarChange(onloadCheck);
    },

    handleBarChange: function(onloadCheck){

      //get bargraph values from model
      var balanceValue = this.model.attributes.balance;
      var expensesValue = this.model.attributes.expenses;
      var purchasesValue = this.model.attributes.purchases;

      //bar graph variables
      var graph = $('#vis'),
        bars = graph.find('#bars'),
        balance = graph.find('#balance'),
        balanceZero = balance.find('.zero'),
        balanceContainer = balance.find('.bar-box'),
        balanceTop = balance.find('.top'),
        balanceMid = balance.find('.mid'),
        balanceBt = balance.find('.bottom'),
        expenses = graph.find('#expenses'),
        expensesZero = expenses.find('.zero'),
        expensesContainer = expenses.find('.bar-box'),
        expensesTop = expenses.find('.top'),
        expensesMid = expenses.find('.mid'),
        expensesBt = expenses.find('.bottom'),
        purchases = graph.find('#purchases'),
        purchasesZero = purchases.find('.zero'),
        purchasesContainer = purchases.find('.bar-box'),
        purchasesTop = purchases.find('.top'),
        purchasesMid = purchases.find('.mid'),
        purchasesBt = purchases.find('.bottom'),
        MAXheight = 473,
        topHeight = 0;

      calculateTotal(balanceValue,expensesValue, purchasesValue, onloadCheck);
      updateTotal(balanceValue,expensesValue, purchasesValue);
      
      //calculate new total for credit card balance
      function calculateTotal(balanceValue, expensesValue, purchasesValue, onloadCheck){

        //check if NAN or empty values
        if (isNaN(balanceValue) || balanceValue === '') {
          balanceValue = 0;
        }
        if (isNaN(expensesValue) || expensesValue === '') {
          expensesValue = 0;
        }
        if (isNaN(purchasesValue) || purchasesValue === '') {
          purchasesValue = 0;
        }

        //Add up all the values
        var totalValue = parseInt(balanceValue) + parseInt(expensesValue) + parseInt(purchasesValue);

        //Find each values percentage of the whole and turn it into a pixel value
        var balanceHeight = (balanceValue/totalValue) * MAXheight;
        var expensesHeight = (expensesValue/totalValue) * MAXheight;
        var purchasesHeight = (purchasesValue/totalValue) * MAXheight;

        //All three Balance, Purchases, Expenses
        if (balanceHeight !== 0 && expensesHeight !== 0 && purchasesHeight !== 0) {
          if (balanceHeight <= 46 || expensesHeight <= 46 || purchasesHeight <= 46) {

            if (balanceHeight <= 46 && expensesHeight <= 46) {
              var purchasesHeight = purchasesHeight - 138;
            }
            else if (purchasesHeight <= 46 && expensesHeight <= 46) {
              var balanceHeight = balanceHeight - 138;
            }
            else if (purchasesHeight <= 46 && balanceHeight <= 46) {
              var expensesHeight = expensesHeight - 138;
            }
            else if(balanceHeight <= 46) {
              expensesHeight = expensesHeight - 69;
              purchasesHeight = purchasesHeight - 69;
            }
            else if(purchasesHeight <= 46) {
              expensesHeight = expensesHeight - 69;
              balanceHeight =balanceHeight - 69;
            }
            else if(expensesHeight <= 46) {
              purchasesHeight = purchasesHeight - 69;
              balanceHeight = balanceHeight - 69;
            }

          }
          else {
            balanceHeight = balanceHeight - 46;
            expensesHeight = expensesHeight - 46;
            purchasesHeight = purchasesHeight - 46;
          }
        }

        //Balance and Expenses only
        else if (balanceHeight !== 0 && expensesHeight !==0) {
          if (balanceHeight <= 56 || expensesHeight <= 56 ) {
            if(balanceHeight <= 56) {
              var expensesHeight = ((expensesValue/totalValue) * MAXheight) - 101;
            }
            else {
              var balanceHeight = (balanceValue/totalValue) * MAXheight - 101;
            }
          }
          else {
            expensesHeight = expensesHeight - 50.5;
            balanceHeight = balanceHeight - 50.5; 
          }
        }
        else if (expensesHeight !== 0 && balanceHeight === 0 ) {
          var expensesHeight = ((expensesValue/totalValue) * MAXheight) - 101;
        }

        //Balance and Purchases only
        else if (balanceHeight !== 0 && purchasesHeight !==0) {
          if (balanceHeight <= 56 || purchasesHeight <= 56 ) {
            if(balanceHeight <= 56) {  
              var purchasesHeight = ((purchasesValue/totalValue) * MAXheight) - 101;
            }
            else {
              var balanceHeight = (balanceValue/totalValue) * MAXheight - 101;
            }
          }
          else {
            purchasesHeight = purchasesHeight - 50.5;
            balanceHeight = balanceHeight - 50.5;
          }
        }
        else if (purchasesHeight !== 0 && balanceHeight == 0 ) {
          var purchasesHeight = ((purchasesValue/totalValue) * MAXheight) - 101;
        }

        //Only one bar expense shown
        else if (purchasesHeight === 0 && expensesHeight === 0) {
          balanceHeight = balanceHeight - 64;
        }
        else if (purchasesHeight === 0 && balanceHeight === 0) {
          expensesHeight = expensesHeight - 64;
        }
        else if (balanceHeight === 0 && expensesHeight === 0) {
          purchasesHeight = purchasesHeight - 64;
        }

        //check if page load is true
        if (onloadCheck === 1){
          if(balanceValue === 0 && expensesValue === 0 && purchasesValue === 0) {
            zeroGraph();
          } else {
            drawGraph(balanceHeight, expensesHeight, purchasesHeight);
          }
        }

        //otherwise it's an input change
        else {
          if(balanceValue === 0 && expensesValue === 0 && purchasesValue === 0) {
            zeroGraph();
          } else {
            animateGraph(balanceHeight, expensesHeight, purchasesHeight);
          }
        }

      }//END calculateTotal();

      //animate graph
      function animateGraph(balanceValue, expensesValue, purchasesValue) {
        var animateSpeed = 800;

        if(balanceContainer.is(':visible')) {
          balanceMid.stop(true,false).animate({
            height: balanceValue + 'px'
          },animateSpeed);

          var topHeight = balanceValue + 31;
          balanceTop.stop(true,false).animate({
            bottom: topHeight + 'px'
          },animateSpeed);
        }
        else {
          balanceZero.css('display','none');
          balanceContainer.fadeIn();

          var topHeight = balanceValue + 31;
          balanceTop.css('bottom', '31px');
          balanceTop.stop(true,false).animate({
            bottom: topHeight + 'px'
          },animateSpeed);

          balanceMid.css('height','0px');
          balanceMid.stop(true,false).animate({
            height: balanceValue + 'px'
          },animateSpeed);

        }

        //expense bar
        if(expensesValue === 0) {
          expenses.fadeOut();
        }
        else if (expenses.is(':visible')){
          var expensesPosition = balanceValue + 37;

          var expensesPositionBt = expensesPosition + 'px';
          expensesBt.stop(true, false).animate({
            bottom: expensesPositionBt
          },animateSpeed);

          var expensesPositionMid = expensesPosition + 32;
          expensesMid.stop(true, false).animate({
            bottom: expensesPositionMid + 'px',
            height: expensesValue + 'px'
          },animateSpeed);

          var expensesPositionTop = expensesPositionMid + expensesValue - 1;
          expensesTop.stop(true, false).animate({
            bottom: expensesPositionTop + 'px'
          },animateSpeed);

        }
        else  {
          //set position
          var expensesPosition = balanceValue + 37;

          var expensesPositionBt = expensesPosition + 'px';
          expensesBt.css('bottom', expensesPosition);

          var expensesPositionMid = expensesPosition + 32;
          var expensesPositionMidpx = expensesPositionMid + 'px';
          expensesMid.css('bottom', expensesPositionMidpx);

          var expensesPositionTop = expensesPositionMid + expensesValue - 1;
          var expensesPositionTopBeg = expensesPositionMid + 8;
          expensesTop.css('bottom', expensesPositionTopBeg);
          
          //show graph
          expenses.fadeIn();

          expensesTop.stop(true,false).animate({
            bottom: expensesPositionTop + 'px'
          },animateSpeed);

          expensesMid.stop(true,false).animate({
            height: expensesValue + 'px'
          },animateSpeed);

        }

        //purchase bar
        if(purchasesValue === 0) {
          purchases.fadeOut();
        }
        else if(expensesValue === 0) {
          if (purchases.is(':visible')){
            var expensesPosition = balanceValue + 37;

            var expensesPositionBt = expensesPosition + 'px';
            purchasesBt.stop(true, false).animate({
              bottom: expensesPositionBt
            },animateSpeed);

            var expensesPositionMid = expensesPosition + 32;
            purchasesMid.stop(true, false).animate({
              bottom: expensesPositionMid + 'px',
              height: purchasesValue + 'px'
            },animateSpeed);

            var expensesPositionTop = expensesPositionMid + purchasesValue - 1;
            purchasesTop.stop(true, false).animate({
              bottom: expensesPositionTop + 'px'
            },animateSpeed);

          }
          else  {
            //set position
            var expensesPosition = balanceValue + 37;

            var expensesPositionBt = expensesPosition + 'px';
            purchasesBt.css('bottom', expensesPosition);

            var expensesPositionMid = expensesPosition + 32;
            var expensesPositionMidpx = expensesPositionMid + 'px';
            purchasesMid.css('bottom', expensesPositionMidpx);

            var expensesPositionTop = expensesPositionMid + purchasesValue - 1;
            var expensesPositionBeg = expensesPositionMid  + 8;
            purchasesTop.css('bottom',expensesPositionBeg);
            
            //show graph
            purchases.fadeIn();

            purchasesMid.stop(true,false).animate({
              height: purchasesValue + 'px'
            },animateSpeed);

            purchasesTop.stop(true,false).animate({
              bottom: expensesPositionTop + 'px'
            },animateSpeed);

          }
        } else {
          if (purchases.is(':visible')){

            var purchasesPosition = expensesPositionTop + 5;

            var purchasesPositionBt = purchasesPosition + 'px';
            
            purchasesBt.stop(true, false).animate({
              bottom: purchasesPositionBt
            }, animateSpeed);

            var purchasesPositionMid = purchasesPosition + 32;
            purchasesMid.stop(true,false).animate({
              bottom: purchasesPositionMid + 'px',
              height: purchasesValue + 'px'
            },animateSpeed);

            var purchasesPositionTop = purchasesPositionMid + purchasesValue - 1;
            purchasesTop.stop(true,false).animate({
              bottom: purchasesPositionTop + 'px'
            },animateSpeed);

          }
          else  {

            var purchasesPosition = expensesPositionTop + 5;

            var purchasesPositionBt = purchasesPosition + 'px';
            purchasesBt.css('bottom', purchasesPosition);

            var purchasesPositionMid = purchasesPosition + 32;
            var purchasesPositionMidpx = purchasesPositionMid + 'px';
            purchasesMid.css('bottom', purchasesPositionMidpx);

            var purchasesPositionTop = purchasesPositionMid + purchasesValue - 1;
            var purchasesPositionBeg = purchasesPositionMid + 1;
            purchasesTop.css('bottom', purchasesPositionBeg);

            purchases.fadeIn();

            purchasesMid.stop(true,false).animate({
              height: purchasesValue + 'px'
            },animateSpeed);

            purchasesTop.stop(true,false).animate({
              bottom: purchasesPositionTop + 'px'
            },animateSpeed);

          }
        }

      }//END animateGraph();

      //nonanimate draw graph
      function drawGraph(balanceValue, expensesValue, purchasesValue) {

        //balance
        if(balanceValue === 0) {
          balanceContainer.css('display','none');
          balanceZero.css('display','block');
        }
        else {
          balanceZero.css('display','none');
          balanceContainer.css('display','block');

          balance.css('display','block');
          var balanceValuePx = balanceValue + 'px';
          balanceMid.css('height', balanceValuePx);

          var topHeightPx = balanceValue + 32 + 'px';
          balanceTop.css('bottom', topHeightPx);
        }
        //expenses
        if(expensesValue === 0) {
          expenses.css('display','none');
        }
        else {
          expenses.css('display','block');
          var expensesPosition = balanceValue + 37;

          var expensesPositionBt = expensesPosition + 'px';
          expensesBt.css('bottom', expensesPositionBt);

          var expensesPositionMid = expensesPosition + 32;
          var expensesPositionMidpx = expensesPosition + 32 + 'px';
          var expensesValuePx = expensesValue + 'px';
          expensesMid.css({'bottom': expensesPositionMidpx,'height': expensesValuePx});

          var expensesPositionTop = expensesPositionMid + expensesValue;
          var expensestopHeightPx = expensesValue + expensesPositionMid - 1 + 'px';
          expensesTop.css('bottom', expensestopHeightPx);
        }
        //purchases
        if(purchasesValue === 0) {
          purchases.css('display', 'none');
        }
        else if (expensesValue === 0) {
          purchases.css('display','block');
          var expensesPosition = balanceValue + 37;

          var expensesPositionBt = expensesPosition + 'px';
          purchasesBt.css('bottom', expensesPositionBt);

          var expensesPositionMid = expensesPosition + 32;
          var expensesPositionMidpx = expensesPosition + 32 + 'px';
          var expensesValuePx = purchasesValue + 'px';
          purchasesMid.css({'bottom': expensesPositionMidpx,'height': expensesValuePx});

          var expensesPositionTop = expensesPositionMid + purchasesValue;
          var expensestopHeightPx = purchasesValue + expensesPositionMid - 1 + 'px';
          purchasesTop.css('bottom', expensestopHeightPx);         
        }
        else {
          purchases.css('display','block');
          var purchasesPosition = expensesPositionTop + 5;

          var purchasesPositionBt = purchasesPosition + 'px';
          purchasesBt.css('bottom', purchasesPositionBt)  ;

          var purchasesPositionMid = purchasesPosition + 32 + 'px';
          var purchasesValuePx = purchasesValue + 'px';
          purchasesMid.css({'bottom': purchasesPositionMid,'height': purchasesValuePx});

          var purchasestopHeightPx = purchasesValue + purchasesPositionMid - 2 + 'px';
          purchasesTop.css('bottom', purchasestopHeightPx);
        }

      }//END drawGraph();
      
      //display zero
      function zeroGraph(){
        balanceZero.css('display','block');
        balanceContainer.css('display','none');
        expensesContainer.css('display','none');
        purchasesContainer.css('display','none');
      }//END zeroGraph();

      function updateTotal(balanceValue, expensesValue, purchasesValue){
        var total = balanceValue + expensesValue + purchasesValue;
        $('#vis').find('.total-number').html(total).formatCurrency({ roundToDecimalPlace: 0 });
      }//END updateTotal

    }

  });

  return Views;

});
