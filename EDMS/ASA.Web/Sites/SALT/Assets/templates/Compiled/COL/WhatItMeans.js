define(["dust"], function(dust) { (function(){dust.register("COL/WhatItMeans",body_0);function body_0(chk,ctx){return chk.write("<div id=\"").reference(ctx.get("blockID"),ctx,"h").write("\" class=\"js-what-it-means-blocks hide\"><div class=\"row collapse\"><a href class=\"close-reveal-modal cancel-circle overlayClose\"></a><div class=\"headline\"><h1>").reference(ctx.get("header"),ctx,"h").write("</h1> </div></div><div class=\"small-12 medium-6 columns no-left-padding\"><div class=\"margin-bottom whatitmeans-text\">").reference(ctx.get("body"),ctx,"h").write("</div><article class=\"link-list full-width\"><ul class=\"no-left-margin\">").reference(ctx.get("links"),ctx,"h").write("</ul></article></div><div class=\"small-12 medium-6 columns no-left-padding\">").reference(ctx.get("image"),ctx,"h").write("</div></div>");}return body_0;})(); });