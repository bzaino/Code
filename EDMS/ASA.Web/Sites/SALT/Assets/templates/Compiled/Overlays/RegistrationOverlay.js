define(["dust", "Compiled/partial_registration_form"], function(dust) { (function(){dust.register("Overlays/RegistrationOverlay",body_0);function body_0(chk,ctx){return chk.write("<div id=\"registrationOverlay\" class=\"hide custom-overlay js-reg-overlay-identifier\"><div class=\"row\"><div class=\"window-shade small-12 medium-6 large-4 right noprint\"><a href=\"\" class=\"js-void-href\"><div class=\"window-shade-close cancel-circle\" tabindex=\"1\" data-focus-sel=\"#js-signup-link\"></div></a>").partial("partial_registration_form",ctx,{"RegistrationSource":body_1}).write("</div></div></div>");}function body_1(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","0","records","0","attributes","registration-source","0"]),ctx,"h",["trim"]);}return body_0;})(); });