define(["dust", "Compiled/page_parent", "Compiled/searchResultsBody"], function(dust) { (function(){dust.register("Pages/searchresults",body_0);var blocks={'dynamicBody':body_1,'singlePageApp':body_2};function body_0(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.partial("page_parent",ctx,null);}function body_1(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.partial("searchResultsBody",ctx,null);}function body_2(chk,ctx){ctx=ctx.shiftBlocks(blocks);return chk.write("<script>require(['modules/HomePage/Application']);</script>");}return body_0;})(); });