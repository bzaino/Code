define(["dust", "Compiled/Modules/ReachOutDelegate", "Compiled/partial_schoolLogo", "Compiled/Modules/Carousel"], function(dust) { (function(){dust.register("partial_homepage_sidebar",body_0);function body_0(chk,ctx){return chk.exists(ctx.getPath(false,["SiteMember","ActiveDirectoryKey"]),ctx,{"block":body_1},null).section(ctx.get("secondaryContent"),ctx,{"block":body_2},null).partial("Modules/ReachOutDelegate",ctx,null);}function body_1(chk,ctx){return chk.partial("partial_schoolLogo",ctx,null);}function body_2(chk,ctx){return chk.helper("select",ctx,{"block":body_3},{"key":body_7});}function body_3(chk,ctx){return chk.helper("eq",ctx,{"block":body_4},{"value":"Carousel"});}function body_4(chk,ctx){return chk.section(ctx.get("records"),ctx,{"block":body_5},null);}function body_5(chk,ctx){return chk.partial("Modules/Carousel",ctx,{"recordKey":body_6});}function body_6(chk,ctx){return chk.reference(ctx.getPath(false,["attributes","P_Primary_Key"]),ctx,"h",["trim"]);}function body_7(chk,ctx){return chk.reference(ctx.get("name"),ctx,"h");}return body_0;})(); });