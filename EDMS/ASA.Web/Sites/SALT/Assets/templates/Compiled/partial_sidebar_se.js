define(["dust", "Compiled/Modules/LiveChatWinnerBadge", "Compiled/partial_schoolLogo", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/Carousel", "Compiled/Modules/ReachOutDelegate"], function(dust) { (function(){dust.register("partial_sidebar_se",body_0);function body_0(chk,ctx){return chk.write("<link type=\"text/css\" rel=\"stylesheet\" media=\"all\" href=\"/Assets/css/valueprop.css\" />").notexists(ctx.getPath(false,["SiteMember","ActiveDirectoryKey"]),ctx,{"else":body_1,"block":body_6},null).section(ctx.get("content"),ctx,{"block":body_11},null).partial("Modules/LiveChatWinnerBadge",ctx,null);}function body_1(chk,ctx){return chk.partial("partial_schoolLogo",ctx,null).partial("Modules/Carousel",ctx,{"recordKey":body_2}).partial("Modules/Carousel",ctx,{"recordKey":body_3}).partial("Modules/Carousel",ctx,{"recordKey":body_4}).partial("Modules/Carousel",ctx,{"recordKey":body_5});}function body_2(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","1","records","0","attributes","carousel-module-sidebar-1"]),ctx,"h",["trim"]);}function body_3(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","1","records","0","attributes","carousel-module-sidebar-2"]),ctx,"h",["trim"]);}function body_4(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","1","records","0","attributes","carousel-module-sidebar-3"]),ctx,"h",["trim"]);}function body_5(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","1","records","0","attributes","carousel-module-sidebar-4"]),ctx,"h",["trim"]);}function body_6(chk,ctx){return chk.partial("Modules/Carousel",ctx,{"recordKey":body_7}).partial("Modules/Carousel",ctx,{"recordKey":body_8}).partial("Modules/Carousel",ctx,{"recordKey":body_9}).partial("Modules/Carousel",ctx,{"recordKey":body_10}).write("<section><article><a href=\"\" class=\"js-void-href registrationOverlay-link\" data-window-shade=\"registrationOverlay\"><img src=\"/assets/images/articles/tiles/signUpPromo.jpg\" title=\"Sign Up\" alt=\"Sign Up\" width=\"300\" height=\"102\" /></a><!-- SWD-7216 Route 'small screen' users to signup page instead of signup overlay --><a href=\"/register\" class=\"registration-link\"><img src=\"/assets/images/articles/tiles/signUpPromo.jpg\" title=\"Sign Up\" alt=\"Sign Up\" width=\"300\" height=\"102\" /></a></article></section>");}function body_7(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","0","records","0","attributes","carousel-module-sidebar-1"]),ctx,"h",["trim"]);}function body_8(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","0","records","0","attributes","carousel-module-sidebar-2"]),ctx,"h",["trim"]);}function body_9(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","0","records","0","attributes","carousel-module-sidebar-3"]),ctx,"h",["trim"]);}function body_10(chk,ctx){return chk.reference(ctx.getPath(false,["content","secondaryContent","0","records","0","attributes","carousel-module-sidebar-4"]),ctx,"h",["trim"]);}function body_11(chk,ctx){return chk.partial("Modules/ReachOutDelegate",ctx,null);}return body_0;})(); });