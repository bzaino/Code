var gateway = gateway || {};

gateway.global = {
  utils: {
    init: function(){
      // activate global plugins
      gateway.global.utils.plugins.init();
    },

    plugins: {
      init: function(){
        
        // Update mainnav
        $('#nav-your-money').addClass('selected');
        
        //Reset any selected value, interferes with styledDropDowns
        $("select.dropdown-large.time option:selected").each(function () {
            $(this).removeAttr('selected');
        });
        $("select.dropdown-large.time option:first").attr('selected', 'selected');
        
        // Style dropdown, update page in response to option selected
        $('#gateway .dropdown-large.time').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});
        $('.dropdown-large').change(function(){
          if($(this).val() === 'just-the-basics'){
            $('#lessons').hide();
            $('#lessons-duplicate').fadeIn();
          } else {
            $('#lessons').fadeIn();
            $('#lessons-duplicate').hide();
          }
        });

      }
        
    } // END: plugins

  } // END: utils

}; // END gateway global var

$(window).load(function(){

  gateway.global.utils.init();
});
