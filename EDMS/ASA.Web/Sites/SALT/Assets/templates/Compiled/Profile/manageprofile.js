define(["dust", "Compiled/page_parent"], function(dust) { (function(){dust.register("Profile/manageprofile",body_0);var blocks={'dynamicTitle':body_1,'dynamicCSS':body_2,'dynamicScripts':body_3,'dynamicBody':body_4};function body_0(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.partial("page_parent",ctx,null);}function body_1(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.write(" Manage My Profile - Salt ");}function body_2(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk;}function body_3(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.write("<script>require(['modules/Profile/myProfile']);</script>");}function body_4(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.write("<div class=\"row\"><article class=\"small-12 columns\"><header><h1>Help Us Help You!</h1><h5>Your answers to the following questions will help us understand you and your specific needs&mdash;so we can make sure the tips, tools, and information you’re looking for are easy for you to find.</h5></header></article></div><div class=\"row js-profileQA-container profileQA-container\"><div class=\"small-12 columns padded\"><div id=\"personal-information-section\"></div><div id=\"financial-information-section\"></div><div id=\"school-information-section\"></div><div id=\"account-details-section\"></div></div></div>");}return body_0;})(); });