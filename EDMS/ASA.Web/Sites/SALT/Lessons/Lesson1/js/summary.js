var gateway = gateway || {};

gateway.global = {
  utils: {
    init: function(){
      // activate global plugins
      gateway.global.utils.plugins.init();
      
      //load data from other pages
      userData = $.jStorage.get('cachedUserData', new Object());
      
      //original values
      $('.original-income').text(userData.totalIncome).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      $('.original-expenses').text(userData.totalExpenses).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      $('.original-cashflow').text(userData.cashflow).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      
      
      //modified values
      $('.new-income').text(userData.newTotalIncome).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      $('.new-expenses').text(userData.newTotalExpenses).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      
      //calculate off of new values
      $('.new-cashflow').text(userData.newTotalIncome - userData.newTotalExpenses).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });
      
      //check to see if new cashflow is positive or negative
      if((userData.newTotalIncome - userData.newTotalExpenses) < 0 ){
        $('.new-cashflow').removeClass('up');
        $('.new-cashflow').addClass('down');
      } else {
        $('.new-cashflow').removeClass('down');
        $('.new-cashflow').addClass('up');
      }
      
      //check to see if old cashflow is positive or negative
      if((userData.totalIncome - userData.totalExpenses) < 0 ){
        $('.original-cashflow').addClass('alert');
      } else {
        $('.original-cashflow').removeClass('alert');
      }

    },

    plugins: {
      init: function(){
        
        // Update mainnav
        $('#nav-your-money').addClass('selected');
        
        // Trigger overlay
        $('a[rel=badges]').trigger('click');

      }
        
    } // END: plugins

  } // END: utils

}; // END gateway global var

$(window).load(function(){
  gateway.global.utils.init();
});
