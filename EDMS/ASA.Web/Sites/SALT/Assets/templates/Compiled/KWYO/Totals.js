define(["dust"], function(dust) { (function(){dust.register("KWYO/Totals",body_0);function body_0(chk,ctx){return chk.write("<div class=\"totals-data row\"><div class=\"columns medium-6 kwyo-total\"><span>Total Owed: $").reference(ctx.getPath(false,["Total","total"]),ctx,"h",["currencyComma"]).write("</span></div><div class=\"columns medium-6 kwyo-monthly\"><span>Monthly Payment: $").reference(ctx.getPath(false,["Monthly","total"]),ctx,"h",["currencyComma"]).write("</span></div></div>").exists(ctx.get("LastDate"),ctx,{"block":body_1},null);}function body_1(chk,ctx){return chk.write("<div class=\"row\"><div class=\"columns\"><small class=\"left\"><i>Last Updated: ").reference(ctx.get("LastDate"),ctx,"h").write("</i></small></div></div>");}return body_0;})(); });