define(["dust"], function(dust) { (function(){dust.register("FindPinF5",body_0);function body_0(chk,ctx){return chk.section(ctx.getPath(false,["content","mainContent","2","records","0","attributes"]),ctx,{"block":body_1},null);}function body_1(chk,ctx){return chk.write("<div id=\"js-findPin\" class=\"reveal-modal medium upload-info\" data-reveal><div class=\"close-reveal-modal cancel-circle overlayClose\"></div><header class=\"row\"><div class=\"headline\">").reference(ctx.get("answer-4-button-text"),ctx,"h").write("</div>").reference(ctx.get("answer-4-page-name"),ctx,"h").write("</header><div class=\"row collapse\"><div class=\"step-header one\"> ").reference(ctx.get("answer-1-button-text"),ctx,"h").write("</div></div><div class=\"row collapse\"><div class=\"small-12 medium-4 columns has-image\"> ").reference(ctx.get("rich-text-image"),ctx,"h").write("</div><div class=\"small-12 8 medium-8 columns pin-instructions\">").reference(ctx.get("answer-1-page-name"),ctx,"h").write("</div></div><div class=\"row collapse\"><div class=\"step-header two\"> ").reference(ctx.get("answer-2-button-text"),ctx,"h").write("</div></div><div class=\"row collapse\"><div class=\"small-12 medium-4 columns has-image\">").reference(ctx.get("btn-help-1"),ctx,"h").write("</div><div class=\"small-12 8 medium-8 columns pin-instructions\">").reference(ctx.get("answer-2-page-name"),ctx,"h").write("</div></div><div class=\"row collapse\"><div class=\"large-12 columns\"><a class=\"button base-btn main-btn right js-void-href\" href=\"\" onClick=\"dcsMultiTrack('DCS.dcsuri','KnowWhatYouOwe/findpin.html', 'WT.si_n', 'mydataimport', 'WT.si_x', '1');\" data-reveal-id=\"js-loanUpload\">").reference(ctx.get("answer-3-page-name"),ctx,"h").write("</a><a class=\"button base-btn main-btn back js-void-href\" href=\"\" data-reveal-id=\"js-uploadInfo\">").reference(ctx.get("answer-3-button-text"),ctx,"h").write("</a></div></div></div>");}return body_0;})(); });