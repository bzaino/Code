define(["dust"], function(dust) { (function(){dust.register("LandingPage/dynamic_banner",body_0);function body_0(chk,ctx){return chk.write("<!-- Banners --><article class=\"value-prop row collapse\"><div class=\"value-entry ").exists(ctx.get("hideCopyForSmall"),ctx,{"block":body_1},null).write("\"><section class=\"columns banner-overlay medium-").helper("dynamicSchoolOutput",ctx,{},{"noLogoCase":"6","schoolCase":"5","CurrentSchoolBrand":body_2}).write("\"><div class=\"").helper("dynamicSchoolOutput",ctx,{},{"noLogoCase":"banner-text-overlay","schoolCase":"school-text-overlay","CurrentSchoolBrand":body_3}).write("\">").helper("if",ctx,{"else":body_4,"block":body_5},{"cond":body_6}).write("</div></section></div><figure class=\"banner-full\">").helper("dynamicBanner",ctx,{},{"CurrentSchool":body_7,"CurrentSchoolBrand":body_8,"CMImage":body_9,"mediumAvailable":"true"}).write("</figure></article><!-- /Banners -->").helper("eq",ctx,{"block":body_10},{"key":body_11,"value":"101-17537"});}function body_1(chk,ctx){return chk.write("hide-for-small");}function body_2(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchoolBrand"]),ctx,"h");}function body_3(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchoolBrand"]),ctx,"h");}function body_4(chk,ctx){return chk.write("<header><h1>").reference(ctx.getPath(false,["bannerData","attributes","headline"]),ctx,"h").write("</h1></header><div class=\"value-content\">").reference(ctx.getPath(false,["bannerData","attributes","body"]),ctx,"h").write("</div>");}function body_5(chk,ctx){return chk.write("<header><h1 class=\"home-default-text\">").reference(ctx.getPath(false,["bannerData","attributes","headline"]),ctx,"h").write(" </h1></header><div class=\"value-content home-default-text\">").reference(ctx.getPath(false,["bannerData","attributes","body"]),ctx,"h").write("</div>");}function body_6(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["bannerData","attributes","P_Primary_Key"]),ctx,"h").write("' === '101-16157' || '").reference(ctx.getPath(false,["bannerData","attributes","P_Primary_Key"]),ctx,"h").write("' === '101-17537'");}function body_7(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchoolBrand"]),ctx,"h");}function body_8(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchoolBrand"]),ctx,"h");}function body_9(chk,ctx){return chk.reference(ctx.getPath(false,["bannerData","attributes","banner-image"]),ctx,"h");}function body_10(chk,ctx){return chk.write("<div class=\"reg-branded small-12\"><p class=\"organization-name-text-color\">").reference(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,"h").write("</p>").reference(ctx.getPath(false,["bannerData","attributes","subhead"]),ctx,"h").write("</div>");}function body_11(chk,ctx){return chk.reference(ctx.getPath(false,["bannerData","attributes","P_Primary_Key"]),ctx,"h");}return body_0;})(); });