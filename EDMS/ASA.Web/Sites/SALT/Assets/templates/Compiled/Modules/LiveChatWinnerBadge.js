define(["dust"], function(dust) { (function(){dust.register("Modules/LiveChatWinnerBadge",body_0);function body_0(chk,ctx){return chk.section(ctx.get("content"),ctx,{"block":body_1},null);}function body_1(chk,ctx){return chk.helper("if",ctx,{"block":body_2},{"cond":body_3});}function body_2(chk,ctx){return chk.write("<div class=\"row margin-top\"><div class=\"small-12 columns double-padding\"><img alt=\"Winner of Best Customer Service Award 2015\" src=\"/Assets/images/special-events/2015_LHNAward.png\" /></div>");}function body_3(chk,ctx){return chk.write("'").reference(ctx.get("name"),ctx,"h").write("' == 'LiveChat' || '").reference(ctx.get("name"),ctx,"h").write("' == 'LiveChatNow'");}return body_0;})(); });