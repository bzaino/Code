define(["dust", "Compiled/Overlays/UnderageMessage"], function(dust) { (function(){dust.register("Overlays/ConfirmPersonalInfo",body_0);function body_0(chk,ctx){return chk.write("<div id=\"confirmPersonalInfo\" class=\"hide custom-overlay\"><div class=\"row\"><!-- for sidebar forms, don't load window shade overlay markup -->").notexists(ctx.get("sidebarReg"),ctx,{"block":body_1},null).write("<h2>One More Thing ...</h2><p>Please confirm the following before we create your account.</p><table><tr><td>First Name</td><td>Last Name</td><td>Year Of Birth</td></tr><tr><td><span id=\"js-confirm-first\"></span></td><td> <span id=\"js-confirm-last\"></span></td><td><span id=\"js-confirm-yob\"></span></td></tr></table><p>Is this information correct?</p><span class=\"right\"> &nbsp;<button class=\"js-back-to-reg button base-btn main-btn\">No, Update Now</button></span><span class=\"right\"><button id=\"underage\" class=\"button base-btn main-btn\">Yes</button>&nbsp;</span><!-- for sidebar forms, don't load window shade overlay markup -->").notexists(ctx.get("sidebarReg"),ctx,{"block":body_2},null).write("</div></div>").partial("Overlays/UnderageMessage",ctx,null).write("<script>require(['salt/ConfirmPersonalInfo']);</script>");}function body_1(chk,ctx){return chk.write("<div class=\"window-shade small-12 medium-9 large-7 double-padding-bottom right\"><a href=\"\" class=\"js-void-href window-shade-close cancel-circle\"></a>");}function body_2(chk,ctx){return chk.write("</div>");}return body_0;})(); });