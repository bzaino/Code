define(["dust"], function(dust) { (function(){dust.register("Modules/ConsumptionTime",body_0);function body_0(chk,ctx){return chk.helper("if",ctx,{"else":body_1,"block":body_7},{"cond":body_10});}function body_1(chk,ctx){return chk.helper("if",ctx,{"block":body_2},{"cond":body_6});}function body_2(chk,ctx){return chk.write("<li class=\"subheader icon-time-to-read ").reference(ctx.get("classParams"),ctx,"h").write("\"><i class=\"fa fa-clock-o\"></i> ").helper("math",ctx,{},{"key":body_3,"method":"divide","round":"true","operand":"200"}).write("m.</li>").helper("if",ctx,{"block":body_4},{"cond":body_5});}function body_3(chk,ctx){return chk.reference(ctx.getPath(false,["attributes","Word-Count"]),ctx,"h");}function body_4(chk,ctx){return chk.write("<div class=\"row-divider\"><span class=\"\">|</span></div>");}function body_5(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["attributes","Language","0"]),ctx,"h").write("' === 'English' && '").reference(ctx.getPath(false,["attributes","Language","1"]),ctx,"h").write("' === 'Spanish'");}function body_6(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["attributes","ContentTypes"]),ctx,"h").write("' === 'Article'");}function body_7(chk,ctx){return chk.write("<li class=\"subheader icon-time-to-read ").reference(ctx.get("classParams"),ctx,"h").write("\"><i class=\"fa fa-clock-o\"></i> ").reference(ctx.getPath(false,["attributes","consumption-time"]),ctx,"h",["trim"]).write("m.</li>").helper("if",ctx,{"block":body_8},{"cond":body_9});}function body_8(chk,ctx){return chk.write("<div class=\"row-divider\"><span class=\"\">|</span></div>");}function body_9(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["attributes","Language","0"]),ctx,"h").write("' === 'English' && '").reference(ctx.getPath(false,["attributes","Language","1"]),ctx,"h").write("' === 'Spanish'");}function body_10(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["attributes","consumption-time"]),ctx,"h",["trim"]).write("' !== ''");}return body_0;})(); });