define(["dust", "Compiled/LoadingSpinner"], function(dust) { (function(){dust.register("Profile/AjaxMessages",body_0);function body_0(chk,ctx){return chk.write("<div class=\"message js-profile-saved-message profile-saved-message hide\"><p>Saved Successfully!</p></div><div class=\"message js-profile-failed-message profile-failed-message hide\"><p>Saving Failed!</p></div><div class=\"small-12 centered\">").partial("LoadingSpinner",ctx,null).write("</div><div class=\"js-profile-saved-overlay profile-saved-overlay hide\">&nbsp;</div><span class=\"js-member-backend-error hide\">Oops, an error has occurred. Please contact Member Support for assistance (855.469.2724).</span><span class=\"js-profile-email-message hide\">New email address must be different from the current email address</span>");}return body_0;})(); });