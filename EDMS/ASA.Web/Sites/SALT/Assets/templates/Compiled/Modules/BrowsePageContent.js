define(["dust", "Compiled/Modules/Salt3SortControl", "Compiled/partial_todo"], function(dust) { (function(){dust.register("Modules/BrowsePageContent",body_0);function body_0(chk,ctx){return chk.write("<div id=\"browse-fixed-header\" class=\"contain-to-grid\"><div class=\"browse-header-bar clearfix\"><h1 class=\"left\">Find What You Need</h1><div class=\"btn-dropdown-container right\"><span class=\"sort-filter-text\">Sort & Filter</span><input type=\"button\" data-dropdown=\"drop-content-types\" aria-controls=\"drop-content-types\" aria-expanded=\"false\" class=\"btn-dropdown js-dropdown dd-opener\"/></div>").partial("Modules/Salt3SortControl",ctx,null).write("</div><hgroup class=\"small-12 columns hide-for-small\"><h3 class=\"subheader\">See everything Salt has to offer. Add more items to your to-do list to stay motivated.</h3></hgroup></div>").notexists(ctx.get("hasContent"),ctx,{"else":body_1,"block":body_12},null);}function body_1(chk,ctx){return chk.write("<div class=\"magellan-container\" data-magellan-expedition=\"fixed\" data-options=\"active_class:sticky-top;destination_threshold:170;threshold:188;throttle_delay:0\"><dl class=\"sub-nav left\">").section(ctx.get("goalRankResponses"),ctx,{"block":body_2},null).write("</dl></div>").section(ctx.get("goalRankResponses"),ctx,{"block":body_4},null);}function body_2(chk,ctx){return chk.exists(ctx.get("filterEnabled"),ctx,{"block":body_3},null);}function body_3(chk,ctx){return chk.write("<dd data-magellan-arrival=\"").reference(ctx.get("$idx"),ctx,"h").write("\" class=\"browse-arrivals\" ><h2 class=\"browse-magellan-headers\">").reference(ctx.get("AnsDescription"),ctx,"h").write("</h2></dd>");}function body_4(chk,ctx){return chk.exists(ctx.get("filterEnabled"),ctx,{"block":body_5},null);}function body_5(chk,ctx){return chk.write("<section class=\"js-load-more-container load-more-container\" data-loadMore-amount=\"10\" data-currentIndex=\"0\" data-mapKey=\"").reference(ctx.get("nameWithNoSpaces"),ctx,"h").write("\"><h2 data-magellan-destination=\"").reference(ctx.get("$idx"),ctx,"h").write("\">").reference(ctx.get("AnsDescription"),ctx,"h").write("</h2>").exists(ctx.getPath(false,["libraryTasks",ctx.get("nameWithNoSpaces")]),ctx,{"else":body_6,"block":body_7},null).write("</section>");}function body_6(chk,ctx){return chk.write("<div class=\"js-browse-no-content no-result\"><p>Sorry we couldn't find what you're looking for. Choose a different option to check out something else. </p></div>");}function body_7(chk,ctx){return chk.section(ctx.getPath(false,["libraryTasks",ctx.get("nameWithNoSpaces")]),ctx,{"block":body_8},null).write("<div class=\"load-more js-load-more-btn-container\" ").notexists(ctx.getPath(false,["libraryTasks",ctx.get("nameWithNoSpaces")]),ctx,{"block":body_11},null).write("><a href class=\"js-void-href js-load-more\">Load More</a></div>");}function body_8(chk,ctx){return chk.write("<div class=\"row collapse js-todoContainer todo\" data-primary-key=\"").reference(ctx.getPath(false,["attributes","P_Primary_Key"]),ctx,"h").write("\" data-content-type=\"").reference(ctx.getPath(false,["attributes","ContentTypes"]),ctx,"h").write("\" data-content-title=\"").reference(ctx.getPath(false,["attributes","resource_link_title"]),ctx,"h").write("\" ").helper("gt",ctx,{"block":body_9},{"key":ctx.get("$idx"),"value":"2"}).write(">").partial("partial_todo",ctx,{"section-identifier":body_10}).write("</div>");}function body_9(chk,ctx){return chk.write("style=\"display:none;\"");}function body_10(chk,ctx){return chk.write("Library-").reference(ctx.get("nameWithNoSpaces"),ctx,"h");}function body_11(chk,ctx){return chk.write("hidden");}function body_12(chk,ctx){return chk.write("<div class=\"magellan-container small-12 columns\" data-magellan-expedition=\"fixed\" data-options=\"active_class:sticky-top;destination_threshold:170;threshold:188;throttle_delay:0\"><div class=\"js-browse-no-content double-margin-top no-result no-result-margin\"><h2>Nothing To See Here, Folks</h2><p>Sorry we couldn't find what you're looking for. Choose a different option to check out something else. </p></div></div>");}return body_0;})(); });