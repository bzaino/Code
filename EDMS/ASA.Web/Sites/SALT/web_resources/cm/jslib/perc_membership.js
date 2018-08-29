(function($)
{
    var userloggedIn = false;
    
    $(document).ready(function(){
		protectContent();
    });
    
    function protectContent() {
		//Check if there's protected content on the page.
		$('#protectedRegionInformation').each(function() {
			//Check if there's a logged in user.
			var protectedRegionInformationElem = $(this);
			var sessionId = $.cookie('perc_membership_session_id');
			userloggedIn = false;
			if (sessionId != null && typeof(sessionId) != 'undefined')
            {
				
                //TODO: Call GetUser service.
                $.PercMembershipService.getUser(sessionId, function(status, data)
                {
                    if (status == $.PercServiceUtils.STATUS_SUCCESS && data.userSummary != null && data.userSummary.email != '')
                    {
                        userloggedIn = true;
                        
                    }
                    handleProtectContent(protectedRegionInformationElem);
                });
            }
			else
			{
				handleProtectContent(protectedRegionInformationElem);
			}
       });
    }
    
    function handleProtectContent(elem)
    {
        //Parse the region protection object.
        var protectObj = $.parseJSON(elem.attr('data'));
        if(typeof(protectObj) != "undefined"){
            //Get the id of the region to protect.
            if (typeof(protectObj.protectedRegion) != "undefined" && protectObj.protectedRegion != "")
            {
                if(!userloggedIn)
                {
                    var message = protectObj.protectedRegionText;
                    var linkHtml = '<div class="perc-protected-region-text"><a href="<hrefText>">'+message+'</a></div>';
                    
                    if (typeof(protectObj.siteLoginPage) != "undefined" && protectObj.siteLoginPage != "")
                    {
                        linkHtml = linkHtml.replace("<hrefText>", protectObj.siteLoginPage +
                            '?loginRedirect=' + encodeURIComponent(window.location));
                    }
                    else
                    {
                        linkHtml = linkHtml.replace("<hrefText>", "#");
                    }
                    
                    $('#'+protectObj.protectedRegion).html(linkHtml);
                }
                $('#'+protectObj.protectedRegion).show();
            }
        }
     }
 
})(jQuery);