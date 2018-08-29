define(["dust", "Compiled/oldTiles", "Compiled/HomePage/CTAProp"], function(dust) { (function(){dust.register("HomePage/HomeBody",body_0);function body_0(chk,ctx){return chk.helper("math",ctx,{"block":body_1},{"key":ctx.get("index"),"method":"mod","operand":2});}function body_1(chk,ctx){return chk.helper("eq",ctx,{"else":body_2,"block":body_4},{"value":0});}function body_2(chk,ctx){return chk.write("<div class=\"row\"><div class=\"padded\"><header class=\"page-title\"><h2>").reference(ctx.get("name"),ctx,"h").write("</h2></header></div></div><div class=\"row padding-top\">").section(ctx.get("records"),ctx,{"block":body_3},null).write("</div>");}function body_3(chk,ctx){return chk.write("<div class=\"home-tile-section small-6 medium-4 large-4 columns ").reference(ctx.get("name"),ctx,"h").write("\">").partial("oldTiles",ctx,null).write("</div>");}function body_4(chk,ctx){return chk.write("<div class=\"hero-container\"><div class=\"small-12 hero-copy-area\"><section class=\"row\"><div class=\"large-5 medium-6 medium-offset-1 columns home-persona-text\"><header><hroup><h1 class=\"hero-headline\">").reference(ctx.getPath(false,["records","0","attributes","headline"]),ctx,"h").write("</h2></hroup></header>").reference(ctx.getPath(false,["records","0","attributes","body"]),ctx,"h").write("<div class=\"intro-and-bullets spaceleft\">").reference(ctx.getPath(false,["records","0","attributes","bullet-list-copy"]),ctx,"h").write("</div>").partial("HomePage/CTAProp",ctx,null).write("</div></section></div><div class=\"small-12 text-center\"><figure class=\"noprint\">").helper("responsiveImage",ctx,{},{"mediumAvailable":"true","imageSource":body_5}).write("</figure><div class=\"small-12 position-absolute slider-arrow-container noprint\"><div class=\"row\"><div class=\"small-1 columns slider-arrows\"><a href='#' class=\"js-arrow\" title=\"previous\"><span class=\"hp-arrows left-arrow\"> </span></a></div><div class=\"small-1 columns slider-arrows\"><a href='#' class=\"js-arrow\" title=\"next\"><span class=\"hp-arrows right-arrow\"> </span></a></div></div></div></div><div class=\"service-nav bottom position-absolute small-12 show-for-small noprint\"><div class=\"row collapse\"><div class=\"columns small-4\"><ul class=\"bullet-nav inline-list right noprint\">").helper("personaNav",ctx,{},{"copy":body_6,"links":body_7}).write("</ul></div><div class=\"columns small-4 noprint\"><!-- SWD-7216 Route 'small screen' users to signup page instead of signup overlay --><a href=\"/register\" class=\" salt_btn noprint registration-link\"> Join Salt </a></div><div class=\"columns small-4\"><ul class=\"bullet-nav inline-list\">").helper("personaNav",ctx,{},{"copy":body_8,"links":body_9}).write("</ul></div></div></div><div class=\"service-nav position-absolute small-12 hide-for-small noprint\"><nav class=\"row position-relative\"><div class=\"columns large-12 no-padding\"><ul class=\"no-margin text-center\">").helper("personaNav",ctx,{},{"links":body_10,"copy":body_11}).write("</ul></div></nav></div></div>");}function body_5(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","banner-image"]),ctx,"h");}function body_6(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","column1-button-copy"]),ctx,"h");}function body_7(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","column1-button-links"]),ctx,"h");}function body_8(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","column2-button-copy"]),ctx,"h");}function body_9(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","column2-button-links"]),ctx,"h");}function body_10(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","persona-links"]),ctx,"h");}function body_11(chk,ctx){return chk.reference(ctx.getPath(false,["records","0","attributes","persona-link-copy"]),ctx,"h");}return body_0;})(); });