/*STYLED INPUTS*/
/* jesus castellanos */


;(function($){

	var methods = {
		init : function(options){

			
			//set default values
			var defaults = {
				boxClass			: 'cbox',
				boxSelectedClass	: 'checked',
				boxTag				: 'div',
				boxPosition			: 'after',
				radioClass			: 'radio',
				radioSelectedClass	: 'checked',
				radioTag			: 'div',
				radioPosition		: 'after'
			};
			
			var options = $.extend(defaults, options);
			
			return $(this).each(function() {
  			//only do this is we are NOT in internet explorer
  			if (!$.browser.msie) {
  				var obj		= $(this);
  				var copy 	= '';
  				var styledFrame;
  				var id = obj.attr('id');
  				var passID = false;
  				var tabIndex = obj.attr('tabindex');
  				
  				if(id === null || id === '' || id === undefined){
  					//no id to transfer
  					passID = false;
  					id = '';
  				} else {
  					//id to transfer:
  					passID = true;
  					id = 'styled-' + id;
  					
  				}
  				
  				//hide() doens't work using css instead
  				//obj.hide();
  				//hide only if its a checkbox or radio button
  				//FIX THIS
  				//seems like it could be done better
  				if(obj.attr('type') == 'checkbox' || obj.attr('type') == 'radio'){
  				  obj.css({
              'visibility': 'hidden',
              'position'  : 'absolute',
              'left'    : '-90000px'
            });
  				}
          
  
  				//see if this has already been manipulated
  				if(!obj.hasClass('inputStyles')){
  					obj.addClass('inputStyles');
  					
  					//check if there is a title attribute defined
  					if(obj.attr('rel') != undefined){
  						copy = obj.attr('rel');
  					}
  					
  					//////////////////////////
  					/* START OF CHECK BOXES */
  					//////////////////////////
  					if(obj.attr('type') == 'checkbox'){			
  						/* construct the styled frame */
  						styledFrame = '<' + options.boxTag + ' class="' + options.boxClass + ' ' + id + '"><span>' + copy + '</span></' + options.boxTag + '>';
  						
              
  						//FIX THIS!
  						//insert element after original element
  						//later make it place it depending on boxPosition variable
  						var styledContainer = $(styledFrame).insertAfter(obj);
  						
  						//check them if they are already checked otherwise make sure they are unchecked
  						if(obj.is(':checked')){
  							styledContainer.addClass(options.boxSelectedClass);
  						} else {
  							styledContainer.removeClass(options.boxSelectedClass);				
  						}
  						
  						//take the tabIndex and add it to the styled version
              obj.attr('tabindex', '');
              styledContainer.attr('tabindex', tabIndex);
              
              //make spacebar or enter toggle the state while focused
              styledContainer.focus(function(){
                $(this).unbind('keypress').keypress(function(e){
                  
                  if(e.which == 32 || e.which == 13){
                    $(this).trigger('click');
                  }
                })
              });
              
  						
  						//if you click on a styled version it will triger a click of its respective form element
  						styledContainer.click(function(){
  							obj.trigger('click');
  							//ie needs to blur so you can spam it
  							obj.blur();
  							
  						});
  						
  						
  						//if a change is made, then reflect the styled view
  						//use propertychange for ie
  						obj.bind($.browser.msie? 'propertychange': 'change', function(e) {
  							if($(this).is(':checked')){
  								styledContainer.addClass(options.boxSelectedClass);
  							} else {
  								styledContainer.removeClass(options.boxSelectedClass);				
  							}
  						});
  					////////////////////////
  					/* END OF CHECK BOXES */
  					////////////////////////
  					} else if(obj.attr('type') == 'radio'){
  					////////////////////////////
  					/* START OF RADIO BUTTONS */
  					////////////////////////////
  						var radioGroup = obj.attr('name');
  						
  						/* construct the styled frame */
  						styledFrame = '<' + options.radioTag + ' class="' + options.radioClass + ' ' + radioGroup + ' ' + id + '"><span>' + copy + '</span></' + options.radioTag + '>';
  						//FIX THIS!
  						//add the styled element depending on variable
  						var styledContainer = $(styledFrame).insertAfter(obj);
  						
  						//check them if they are already checked otherwise make sure they are unchecked
  						if(obj.is(':checked')){
  							styledContainer.addClass(options.radioSelectedClass);
  						} else {
  							styledContainer.removeClass(options.radioSelectedClass);				
  						}
  						
  						//take the tabIndex and add it to the styled version
              obj.attr('tabindex', '');
              styledContainer.attr('tabindex', tabIndex);
              
              //make spacebar or enter toggle the state while focused
              styledContainer.focus(function(){
                $(this).unbind('keypress').keypress(function(e){
                  if(e.which == 32 || e.which == 13){
                    $(this).trigger('click');
                  }
                })
              });
  						
  						//if you click on a styled version it will triger a click of its respective form element
  						styledContainer.click(function(){
  							obj.trigger('click');
  							//ie needs to blur so you can spam it
  							obj.blur();
  						});
  						
  						//if a change is made, then reflect the styled view
  						//use propertychange for ie
  						obj.bind($.browser.msie? 'propertychange': 'change', function(e) {
  							if($(this).is(':checked')){
  								group = '.' + radioGroup;
  								$(group).removeClass(options.radioSelectedClass);
  								
  								styledContainer.addClass(options.radioSelectedClass);
  							} else {
  								styledContainer.removeClass(options.radioSelectedClass);				
  							}
  						});
  					////////////////////////////
  					/* END OF RADIO BUTTONS *///
  					////////////////////////////
  					}//end of IF radio
  					
  				} else { // ELSE already manipulated
  
  				}//end of IF already manipulated
  		}//end of IF not IE
				
			});//end of return
			
		},
		refresh : function(){
			/* remove() all by class name */
			/* then run the init function again */

		}
	};
	
	$.fn.styledInputs = function(method) {
	
		if(methods[method]){
			return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		      return methods.init.apply( this, arguments );
		} else {
			$.error( method + ' does not exist on jQuery.styledInputs' );
		}
	};
})(jQuery);
