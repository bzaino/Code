define(["dust", "Compiled/partial_social", "Compiled/UserRatingWidget"], function(dust) { (function(){dust.register("Modules/ContentFooter",body_0);function body_0(chk,ctx){return chk.write("<div class=\"row collapse\"><div class=\"small-12 medium-6 columns\">").partial("partial_social",ctx,null).write("</div>").helper("if",ctx,{"block":body_1},{"cond":body_2}).write("</div>");}function body_1(chk,ctx){return chk.write("<div class=\"small-12 medium-6 columns\">").partial("UserRatingWidget",ctx,null).write("</div>");}function body_2(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["mainContent","0","record","attributes","ContentTypes","0"]),ctx,"h").write("' !== 'Lesson'");}return body_0;})(); });