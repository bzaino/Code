var barGraph = barGraph || {};

barGraph.global = {

    graph: function() {

      var graph = $('#barGraph'),
          bars = graph.find('.bars'),
          balance = graph.find('#balance'),
          balanceZero = balance.find('.zero'),
          balanceContainer = balance.find('.bar-box'),
          balanceTop = balance.find('.top'),
          balanceMid = balance.find('.mid'),
          balanceBt = balance.find('.bottom'),
          expense = graph.find('#expense'),
          expenseZero = expense.find('.zero'),
          expenseContainer = expense.find('.bar-box'),
          expenseTop = expense.find('.top'),
          expenseMid = expense.find('.mid'),
          expenseBt = expense.find('.bottom'),
          MAXheight = 150,
          topHeight = 0;


      zeroGraph();

      $('.keep-going').click(function(){
        console.log('clicky');
        calculateTotalCredit(100);

      });

      $('.add-expense').click(function(){
        console.log('expensey');
        animateGraphExpense(1,50);
      });


      //display zero
      function zeroGraph(){
        balanceZero.css('display','block');
        balanceContainer.css('display','none');
        expense.css('display','none');
        console.log('iran');
      }//END zeroGraph();

      //calculate new total for credit card balance
      function calculateTotalCredit(balanceValue){
        console.log('calculateTotal');
        console.log(balanceValue);
        var percentageHeight = balanceValue/MAXheight;

        if(percentageHeight > 1){
          console.log('i am too big');
          animateGraph(ele,86);
        }

        else {
          console.log('percentageHeight '+percentageHeight);
          percentageHeight = MAXheight * percentageHeight;
          console.log('percentageHeight '+percentageHeight);
          var ele = balanceContainer;
          animateGraph(ele, 86);
        }

      }//END calculateTotalCredit();

      //calculate total for expenses
      function calculateTotal(expenseValue) {

      }//END calculateTotal();

      //animate graph
      function animateGraph(ele,newHeight) {
        if (newHeight > 0) {
          balanceZero.css('display','none');
          balanceContainer.fadeIn();
          balanceMid.animate({
            height: newHeight + 'px'
          });

          topHeight = newHeight + 32;

          balanceTop.animate({
            bottom: topHeight + 'px'
          });
        }
        // $('#vis #bars #income').stop(true,false).animate({
        //   height: incomeGraph + '%'
        // }, 1000, function() {


      }//END animateGraph();

      function animateGraphExpense(newHeight,newExpense) {
        if(newExpense > 0) {
          expense.fadeIn();

          expensePosition = topHeight + 3;
          expenseBt.css({
           bottom: expensePosition + 'px'
          });

          expensePositionMid = expensePosition + 32;

          expenseMid.css({bottom: expensePositionMid + 'px'});

          expenseMid.animate({
            height: 50 +'px'
          });

          expensePositionTop = expensePositionMid + 50;
          expenseTop.css({bottom: expensePositionMid + 'px'});
          expenseTop.animate({
            bottom: expensePositionTop + 'px'
          });
        }

      }//END animateGraphExpense

    },

}; // END expenses global var


$('document').ready(function(){
  // activate utils
  barGraph.global.graph();
});

