var ASA = ASA || {};

require(['jquery', 'jquery.colorbox', 'jquery.client'], function ($) {
  ASA.global = {
    utils: {
      init: function() {
        // activate global plugins
        ASA.global.utils.plugins.init();
        ASA.global.utils.login.init();
        ASA.global.utils.login.validate();
        ASA.global.utils.notifications.init();
        ASA.global.utils.badges.init();
        ASA.global.utils.common.init();
        
      },
      plugins: {
        init: function() {
          // init all plugins
          userOS = $.client.os;
          if(userOS == 'Windows'){
            $('html').addClass('windows');
          }
        }
      },
      
      // Set up Log in functionality
      login: {
        init: function() {
          
          $('a[rel=log-in-header], a[rel=log-in]').colorbox({
            inline: true,
            initialWidth: 336,
            initialHeight: 440
          }).click(function(e){
            e.preventDefault();
          });
          
          //Remove logout button if not logged in
          if(!$('#container .recognized').length){
            $('#logoutLink').remove();
          } else {
            $('#logoutLink a').click(function(e){
              e.preventDefault();
              if(ASA.fb.getFBLoggedInStatus()){
                ASA.fb.fbLogout();
              } else {
                window.location = 'index.html';
              }
            });
          };
        },
        
        // Initialized Log in Validation
        validate: function() {
          
          $('#loginForm').validate({
            
            // Validation rules
            rules: {
              // Email field allows for text input, max 40 characters
              loginEmail: {
                required: true,
                email: true,
                maxlength: 40
              },
              // Password field allows for text input, max 8 characters
              loginPassword: {
                required: true,
                minlength: 8
              }
            },
            
            // Custom validator messages -- we may need to move these to the DOM for CMS capability, leaving here for POC
            messages: {
              loginPassword: {
                required: '<strong>Oh, no!</strong> We didn’t recognize your password.',
                minlength: '<strong>Whoa!</strong> Passwords have to be at least 8 characters.'
              },
              loginEmail: {
                required: '<strong>Uh, oh!</strong> We didn’t recognize your email address.',
                email: '<strong>Hmmm...</strong> We don’t recognize that email address.'
              }
            },
            
            // Submit handler function
            submitHandler: function(form){
              
              // Change Continue button
              $('a.submit').addClass('submitting').text('Wait for it...');
              
              //for POC mock a server request then log user in to recognized homepage
              setTimeout(offlineLogIn(),5000);
              return false;
            },
            invalidHandler: function(){
              //resize the colorbox overlay after submit and errors get called
              //$.colorbox.resize({innerWidth:336, innerHeight:490});
            },

            // Debug set to true for POC purposes
            debug: true

          });
          
          function offlineLogIn(){
            window.location = "index-recognized.html";
          }
          // Validate onClick
          $('#loginForm a.submit').click(function(e){
            e.preventDefault();
            $('#loginForm').submit();
          });

        }

      },

      // Set up our notifications
      notifications: {
        init: function() {
          
          // Add hover class
          $('.notification').hover(
            function() {
              $(this).addClass('hover');
            },
            function() {
              $(this).removeClass('hover');
            }
          );

          // Function for closing individual notification
          var closeButton = $('a.dismiss');
          closeButton.click(function(e){
            e.preventDefault();
            var thisNotification = $(this).closest('.notification');
            thisNotification.slideUp('fast');
          });
        }
      },
      
      // Set up badges/rewards
      badges: {
        init: function() {

          // Trigger overlay
          $('a[rel=badges]').colorbox({
            initialWidth: 504,
            initialHeight: 495,
            onComplete:function(){
              var shareButton = $('#share-it');

              // Enable Share button
              function checkSocial(){
                var networks = $('.social-buttons a.selected').length;              
                if(networks > 0){
                  shareButton.removeClass('disabled');
                } else {
                  shareButton.addClass('disabled');
                }
              }
              
              $('.social-buttons a').click(function(e){
                e.preventDefault();

                // Check for Facebook
                if($(this).attr('rel') === 'facebook'){
                  //If not logged in
                  if(!$('a.btn-facebook .selected').length) {
                       //Give user option to log in now
                       FB.login(function(response) {
                           if (response.authResponse) {
                             $('a.btn-facebook').toggleClass('selected');
                             checkSocial();
                          }
                       });
                    }
                  // Check for Twitter
                } else if($(this).attr('rel') === 'twitter') {
                  // Do Twitter stuff
                  $('a.btn-twitter').toggleClass('selected');
                } else {
                  // neither selected
                }
                checkSocial();
              });
                                      
              // Share button
              shareButton.click(function(event){
                event.preventDefault();

                // Check if form is disabled
                if(shareButton.hasClass('disabled')){

                } else {
                  // Slide the frame to the left
                  $('#sliding-content').animate({
                    marginLeft: '-=504'
                  }, 300, function() {
                    //animate in the img.popout
                    $('img.popout').animate({
                      top: '-12',
                      opacity: '1'
                      }, 350);
                    });
                  }
              });
              
              // Rental Code hover demo
              $('h4#rental-code').hover(
                function(){
                  $(this).addClass('hover');
                  $('h4#rental-code span').fadeIn('fast');
                },
                function(){
                  $(this).removeClass('hover');
                  $('h4#rental-code span').fadeOut('fast');
                }
              );
              
              // Copy Rental Code to clipboard demo
              $('h4#rental-code').click(function(){
                // TO DO: Copy to clipboard
                $('h4#rental-code').addClass('copied');
                $('span.tooltip-message').addClass('copied').html('Copied!');
              });
                          
              // Save for Later
              $('a.save-for-later').click(function(){
                $.colorbox.close();
                return false;
              });
            }
          });

        } //END badges init
      },
      
      // Other common scripts
      common: {
        init: function() {
          // Open rel="external" links in new window
          $('a[rel*=external]').click(function(){
            this.target = "_blank";
          });
          
          // Fix Placeholders in non-compliant browsers
          if(!Modernizr.input.placeholder){

          	$('[placeholder]').focus(function() {
          	  var input = $(this);
          	  if (input.val() == input.attr('placeholder')) {
          		input.val('');
          		input.removeClass('placeholder');
          	  }
          	}).blur(function() {
          	  var input = $(this);
          	  if (input.val() == '' || input.val() == input.attr('placeholder')) {
          		input.addClass('placeholder');
          		input.val(input.attr('placeholder'));
          	  }
          	}).blur();
          	$('[placeholder]').parents('form').submit(function() {
          	  $(this).find('[placeholder]').each(function() {
          		var input = $(this);
          		if (input.val() == input.attr('placeholder')) {
          		  input.val('');
          		}
          	  })
          	});

          }
        }
      }

      // END: utils
    }
    // END ASA global var
  };

  // using window ready is better than DOM ready
  $(window).load(function() {
    // activate utils
    ASA.global.utils.init();
  });
});