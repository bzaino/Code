define(["dust"], function(dust) { (function(){dust.register("Overlays/UnderageMessage",body_0);function body_0(chk,ctx){return chk.write("<div id=\"UnderageMessage\" class=\"hide custom-overlay\"><div class=\"row\"><!-- for sidebar forms, don't load window shade overlay markup -->").notexists(ctx.get("sidebarReg"),ctx,{"block":body_1},null).write("<p>We're Sorry ...<br>Unfortunately, you are ineligible to join Salt<sup>&reg;</sup> at this time.<!-- if on sidebar registration form, hide the message about closing the overlay -->").notexists(ctx.get("sidebarReg"),ctx,{"block":body_2},null).write("</p><!-- for sidebar forms, don't load window shade overlay markup -->").notexists(ctx.get("sidebarReg"),ctx,{"block":body_3},null).write("</div></div>");}function body_1(chk,ctx){return chk.write("<div class=\"window-shade small-12 medium-9 large-7 double-padding-bottom right\">");}function body_2(chk,ctx){return chk.write("<br>This window will close automatically.");}function body_3(chk,ctx){return chk.write("</div>");}return body_0;})(); });