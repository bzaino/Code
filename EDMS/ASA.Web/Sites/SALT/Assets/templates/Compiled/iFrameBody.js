define(["dust", "Compiled/Modules/ContentHeaderBar", "Compiled/Modules/DisplayAuthorAndDate", "Compiled/Modules/ContentFooter", "Compiled/Modules/FlatTags", "Compiled/Modules/AuthorPartnerBlock", "Compiled/TodoCompletor"], function(dust) { (function(){dust.register("iFrameBody",body_0);function body_0(chk,ctx){return chk.write("<article class=\"content\">").section(ctx.get("content"),ctx,{"block":body_1},null).write("</article>").helper("if",ctx,{"block":body_3},{"cond":body_4});}function body_1(chk,ctx){return chk.partial("Modules/ContentHeaderBar",ctx,null).partial("Modules/DisplayAuthorAndDate",ctx,{"englishContent":"english"}).write("<iframe src=\"").reference(ctx.getPath(false,["mainContent","0","records","0","attributes","iframe-url","0"]),ctx,"h",["trim","s"]).write("\" width=\"100%\"  style=\"border:0;\" frameborder=\"0\" class=\"calcXML ").reference(ctx.getPath(false,["mainContent","0","records","0","attributes","iframe-height-class","0"]),ctx,"h",["trim"]).write("\"></iframe><div class=\"row collapse\">").section(ctx.getPath(false,["mainContent","0"]),ctx,{"block":body_2},null).write("</div>").partial("Modules/ContentFooter",ctx,null);}function body_2(chk,ctx){return chk.partial("Modules/FlatTags",ctx,null).partial("Modules/AuthorPartnerBlock",ctx,null);}function body_3(chk,ctx){return chk.partial("TodoCompletor",ctx,null);}function body_4(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["content","mainContent","0","records","0","attributes","RefToDoStatusID"]),ctx,"h").write("' !== '2'");}return body_0;})(); });