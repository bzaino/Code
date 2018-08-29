var home = home || {};

home.global = {
	utils: {
		init: function(){
			// activate global plugins
			home.global.utils.plugins.init();
		},
		
		plugins: {
		  // init all plugins
			init: function(){
				
				
				
				/**
				  Set up the hero carousel using  jQuery Cycle plugin
					http://www.malsup.com/jquery/cycle/
				*/
				$('#hero-content').cycle({
				  fx:       'fade',
				  pause:    0,
				  timeout:  4000,
				  speed:    500,
				  next:     '#hero-next', 
          prev:     '#hero-prev',
          after:    afterCycle
				});
				
				//afterCycle(); updates the hero slide count
				function afterCycle(curr,next,opts) {
      		var count = + (opts.currSlide + 1) + ' / ' + opts.slideCount;
      		$('#hero-count').html(count);
      	}
      	
      	/**
      		This sets the hotspots which will pause the homepage hero
      	*/
      	$('#hero-content .slide a, #hero-nav-container').mouseenter(function(){
      	 $('#hero-content').cycle('pause');
      	}).mouseleave(function(){
      	 $('#hero-content').cycle('resume');
      	});
      	
			}
		} // END: plugins
		
/* 		loadSocial: function(){
  
            var profileData; = ASA.fb.getProfileData();
            if (profileData != undefined){
               ASA.fb.getProfileImage();               
               $('.friends .friend-name').text(profileData.friend1_name);
               $('.friends .friend-image').attr("src", profileData.friend1_avatar);
               $('.notification.social .message .name').text(profileData.friend2_name);
               $('.notification.social .img img').attr("src", profileData.friend2_avatar);
            }
           ASA.fb.fbEnsureInit(FB.getLoginStatus(function(response) {
               if (response.status === 'connected' && response.authResponse != undefined) {

                  //get one friend
                  FB.api('/me/friends', function(response){
                  randomFriend = ASA.fb.getRandomFriend(response.data.length);
                  $('.friends .friend-name').text(response.data[randomFriend].name);
                  $('.friends .friend-image').attr("src", 'http://graph.facebook.com/' + response.data[randomFriend].id + '/picture');
                  });


                  //get another
                  FB.api('/me/friends', function(response){
                  randomFriend = ASA.fb.getRandomFriend(response.data.length);
                  $('.notification.social .message .name').text(response.data[randomFriend].name);
                  $('.notification.social .img img').attr("src", 'http://graph.facebook.com/' + response.data[randomFriend].id + '/picture');

               });
            }
         })); 
		} // END: loadSocial */
	} // END: utils
	
}; // END home global var


// using window ready is better than DOM ready
$(window).load(function(){

	// activate utils
	home.global.utils.init();
	
/*	if($('.recognized').length){
	 home.global.utils.loadSocial();
	}
*/	
});

