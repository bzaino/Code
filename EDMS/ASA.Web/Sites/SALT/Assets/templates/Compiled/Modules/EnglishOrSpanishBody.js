define(["dust", "Compiled/partial_body", "Compiled/partial_body"], function(dust) { (function(){dust.register("Modules/EnglishOrSpanishBody",body_0);function body_0(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","isSpanish"]),ctx,{"else":body_1,"block":body_2},null);}function body_1(chk,ctx){return chk.partial("partial_body",ctx,{"english":"active","englishToggle":"active","spanish":"hide"});}function body_2(chk,ctx){return chk.partial("partial_body",ctx,{"spanish":"active","spanishToggle":"active","english":"hide"});}return body_0;})(); });