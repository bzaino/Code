/*STYLED DROPDOWNS*/


;(function($){
	var methods = {
		init : function(options){

			
			//set default values
			var defaults = {
				ddClass			    : 'dd',
				ddPosition			: 'after',
				autoAdjust      : false
			};
			
			var options = $.extend(defaults, options);
			
			return $(this).each(function() {
  			if(!$('html').hasClass('lt-ie9')){
  				var obj		= $(this);
  				var copy 	= '';
  				var styledFrame;
  				var id = obj.attr('id');
  				var passID = false;
  				var tabIndex = obj.attr('tabindex');
  				var wrapperWidth;
  				var biggest = 0;
  				
  				if(id === null || id === '' || id === undefined){
  					//no id to transfer
  					passID = false;
  					id = '';
  				} else {
  					//id to transfer:
  					passID = true;
  					id = 'styled-' + id;
  					
  				}
  				
  				//hide the original
  				if(obj){
  				  obj.css({
              'visibility': 'hidden',
              'position'  : 'absolute',
              'left'    : '-90000px'
            });
  				}
  				
  				function adjustWidth(){
  				  
  		       if(options.autoAdjust == true && biggest == 0){
  
  		          hidden = null;
                //show then hide opacity
                $('dd ul',styledContainer).css({opacity: 0}).show();
                
                //find the largest element in the dropdown
                $('dd ul li',styledContainer).each(function(key, value){
                  
                  if($(this).hasClass('hidden')){
                    hidden = $(this);
                    $(this).removeClass('hidden');
                  }
                  
                  if($(this).outerWidth() >= biggest){
  
                    biggest = $(this).outerWidth();
                  }
                })
                
                hidden.addClass('hidden');
                $('dd ul',styledContainer).hide().css({opacity: 1})
                $(styledContainer).css({width: biggest});
                $(styledContainer).parent('span').css({width: biggest});
            }
  				}
         
         //dont style this if its already been styled.
          if(!obj.hasClass('ddStyled')){
  					obj.addClass('ddStyled');
            //start the structure of the new element
            styledFrame = '<dl class="ddStyled ' + options.ddClass + ' ' + id + '">' + '</dl>';
            //insert styled version 
            var styledContainer = $(styledFrame).insertAfter(obj);
            //add all of the required elements inside the styled frame
            $(styledContainer).append('<dt><a href="#">' + obj.find('option[selected]').text() + '</a></dt>');
            $(styledContainer).append('<dd><ul></ul></dd>');
            $('option', obj).each(function(){
                $(styledContainer).find('dd ul').append('<li data-name="' + $(this).val() + '"><a href="#">' + $(this).text() + '</a></li>');
                //$("dd ul", $(styledContainer)).append('<li class="' + $(this).val()  + '"><a href="#" class="'  + $(this).text() + '">' + $(this).text() + '<span class="value">' + $(this).val() + '</span></a></li>');
            });
            
            //hide default selected
            $(styledContainer).find('dd ul li[data-name=' + $(this).val() + ']').addClass('hidden');
            
            adjustWidth();
            
            if(options.autoAdjust == true){
              $(styledContainer).css({width: biggest + 35})
              $(styledContainer).parent('span').css({width: biggest + 35});
            }
              				
    				//user interactions
    				$(styledContainer).find('dt').click(function(e) {
    				  e.preventDefault();
    				  
    				  //when i click on a styled version
    				  //toggle away all other styled versions
    				  $('.ddStyled.active').not($(styledContainer)).find('ul').hide();
    				  $('.ddStyled.active').not($(styledContainer)).removeClass('active');
    				  
    				  
    				  $(styledContainer).find('ul').slideToggle(150);
    				  
    				  if(!$(styledContainer).hasClass('active')){
    				    $(styledContainer).addClass('active');
    				  } else {
    				    $(styledContainer).removeClass('active');
    				  }
              
              
              
            });
            
            //clicking out
            $(document).bind('click', function(e) {
                var clicked = $(e.target);
                if(!clicked.parents().hasClass(options.ddClass)){
                  $('.' + options.ddClass + " dd ul").hide();
                  $('.' + options.ddClass).removeClass('active')
                }
            });
            
            //item clicked
            //does the swap
            $(styledContainer).find('dd ul li a').click(function(e) {
  
              
                e.preventDefault();
                var chosen  = $(this).parents('ul li').attr('data-name');
                var text    = $(this).text()
                
                $(styledContainer).find('dt a').text(text);
                $(styledContainer).find('dd ul').hide();
                
                $(styledContainer).removeClass('active');
                
                
                
                
                //set new value on the select element
                obj.val(chosen);
                obj.change();
    
            });
            
    				obj.change(function(){
  
    				  //change the styled value
    				  chosen      = $(this).val();
  
    				  
    				  //unhide the hidden ones
    				  $(styledContainer).find('dd ul li.hidden').removeClass('hidden');
    				  
    				  //find the one you just chose and hide it
    				  $(styledContainer).find('dd ul li[data-name=' + chosen + ']').addClass('hidden');
  
    				  obj.find('option').removeAttr('selected', '');
              //add newly selected thing + 
              obj.find('option[value=' + chosen + ']').attr('selected', 'selected');
              
              displayText = $(this).find('option[selected=selected]').text();
    				  //make sure the styled one shows the right value
    				  $(styledContainer).find('dt a').text(displayText);
              
              //resize
              adjustWidth();
    				});
    		  } else { 
    		    adjustWidth();
    		  }
    		}//end of ie9 conditional
			});//end of return
			
		}, // //END init
		refresh : function(){
			/* remove() all by class name */
			/* then run the init function again */

			


		} // END refresh
	};
	
	$.fn.styledDropDowns = function(method) {
	
		if(methods[method]){
			return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		      return methods.init.apply( this, arguments );
		} else {
			$.error( method + ' does not exist on jQuery.styledDropDowns' );
		}
	};
})(jQuery);
