define(["dust"], function(dust) { (function(){dust.register("Modules/DisplayAuthorAndDate",body_0);function body_0(chk,ctx){return chk.helper("if",ctx,{"block":body_1},{"cond":body_4}).write("<small><em>").helper("if",ctx,{"else":body_5,"block":body_7},{"cond":body_9}).write("</em></small>");}function body_1(chk,ctx){return chk.write("<small><em>").exists(ctx.get("englishContent"),ctx,{"else":body_2,"block":body_3},null).write(" ").reference(ctx.getPath(false,["mainContent","0","record","attributes","author-name"]),ctx,"h").write(" - </em></small>");}function body_2(chk,ctx){return chk.write("Por");}function body_3(chk,ctx){return chk.write("By");}function body_4(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["mainContent","0","record","attributes","author-name"]),ctx,"h").write("' !== ' '");}function body_5(chk,ctx){return chk.helper("postedDateFormatter",ctx,{},{"version":ctx.getPath(false,["mainContent","0","record","attributes","Item_Version"]),"lng":body_6,"date":ctx.getPath(false,["mainContent","0","record","attributes","sys_contentpostdate"])});}function body_6(chk,ctx){return chk.reference(ctx.get("spanishContent"),ctx,"h");}function body_7(chk,ctx){return chk.helper("postedDateFormatter",ctx,{},{"version":ctx.getPath(false,["mainContent","0","record","attributes","Item_Version"]),"lng":body_8,"date":ctx.getPath(false,["mainContent","0","record","attributes","spanish-modified-date"])});}function body_8(chk,ctx){return chk.reference(ctx.get("spanishContent"),ctx,"h");}function body_9(chk,ctx){return chk.write("'").reference(ctx.get("spanishContent"),ctx,"h").write("' === 'spanish' && '").reference(ctx.getPath(false,["mainContent","0","record","attributes","spanish-modified-date"]),ctx,"h").write("' !== ' ' ");}return body_0;})(); });