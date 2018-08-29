define(["dust"], function(dust) { (function(){dust.register("Overlays/ScholarshipDetail",body_0);function body_0(chk,ctx){return chk.write(" ").section(ctx.get("Scholarship"),ctx,{"block":body_1},null).write("<div class=\"row\"><div class=\"small-12 columns padding-bottom padding-right\">").exists(ctx.get("ApplicationUrl"),ctx,{"else":body_15,"block":body_17},null).write("</div></div></div>");}function body_1(chk,ctx){return chk.write("<div class=\"reveal-modal medium no-padding scholarship-search-detail\" id=\"js-scholarshipDetail\" data-reveal><div class=\"tabs-div\"><dl class=\"tabs\"><dd class=\"tab-title active\">").reference(ctx.get("Title"),ctx,"h").write("</dd></dl><div class=\"close-reveal-modal cancel-circle overlayClose\"></div></div><div class=\"tabs-content\"><div class=\"row\"><div class=\"small-6 columns\"><div class=\"row collapse\"><div class=\"small-2 medium-1 columns\"><span class=\"hilite-icon\"><i class=\"fa fa-dollar\"></i></span></div><div class=\"small-10 medium-11 columns\"><span class=\"hilite\">Award Amount</span><br><span class=\"ss-loan-amount\">").reference(ctx.get("numericDollarAmount"),ctx,"h").write("</span></div></div></div><div class=\"small-6 columns\"><div class=\"row collapse\"><div class=\"small-2 medium-1 columns\"><span class=\"hilite-icon\"><i class=\"fa fa-calendar\"></i></span></div><div class=\"small-10 medium-11 columns\"><span class=\"hilite\">Application Deadline</span><br><span=\"application-date\">").reference(ctx.get("FormattedDeadlineDate"),ctx,"h").write("</span></div></div></div></div><div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Awarded By</span><br>").reference(ctx.get("ProviderName"),ctx,"h").write("</div></div>").exists(ctx.get("AwardRequirements"),ctx,{"block":body_2},null).exists(ctx.get("DollarAmount"),ctx,{"block":body_4},null).exists(ctx.get("PdfUrl"),ctx,{"block":body_5},null).exists(ctx.get("AdditionalInfo"),ctx,{"block":body_6},null).write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Contact</span><br>").helper("contactParser",ctx,{},{"providerName":ctx.get("ProviderName"),"contactPerson":ctx.get("ContactPerson"),"contactTitle":ctx.get("ContactTitle"),"address1":ctx.get("Address1"),"address2":ctx.get("Address2"),"city":ctx.get("City"),"province":ctx.get("Province"),"state":ctx.get("State"),"zipCode":ctx.get("ZipCode"),"country":ctx.get("Country"),"email1":ctx.get("Email1"),"phone1":ctx.get("Phone1")}).exists(ctx.get("Email1"),ctx,{"block":body_7},null).exists(ctx.get("Email2"),ctx,{"block":body_8},null).exists(ctx.get("Phone1"),ctx,{"block":body_9},null).exists(ctx.get("Phone2"),ctx,{"block":body_10},null).exists(ctx.get("Fax"),ctx,{"block":body_11},null).exists(ctx.get("ProviderUrl"),ctx,{"block":body_12},null).write("</div></div>").exists(ctx.get("FreeTextDeadlineDate"),ctx,{"block":body_13},null).write("</div>");}function body_2(chk,ctx){return chk.write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Requirements</span><br>").section(ctx.get("AwardRequirements"),ctx,{"block":body_3},null).write("</div></div>");}function body_3(chk,ctx){return chk.reference(ctx.getPath(true,[]),ctx,"h").write("<br>");}function body_4(chk,ctx){return chk.write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Awarded Amount</span><br><pre>").reference(ctx.get("DollarAmount"),ctx,"h").write("</pre></div></div>");}function body_5(chk,ctx){return chk.write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Pdf</span><br><a href=\"").helper("urlParser",ctx,{},{"url":ctx.get("PdfUrl")}).write("\">Download</a></div></div>");}function body_6(chk,ctx){return chk.write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Additional Information</span><br><pre>").reference(ctx.get("AdditionalInfo"),ctx,"h").write("</pre></div></div>");}function body_7(chk,ctx){return chk.write(" <a href=\"mailto:").reference(ctx.get("Email1"),ctx,"h").write("\">").reference(ctx.get("Email1"),ctx,"h").write("</a><br>");}function body_8(chk,ctx){return chk.write(" <a href=\"mailto:").reference(ctx.get("Email2"),ctx,"h").write("\">").reference(ctx.get("Email2"),ctx,"h").write("</a><br>");}function body_9(chk,ctx){return chk.reference(ctx.get("Phone1"),ctx,"h").write("<br>");}function body_10(chk,ctx){return chk.reference(ctx.get("Phone2"),ctx,"h").write("<br>");}function body_11(chk,ctx){return chk.write("Fax: ").reference(ctx.get("Fax"),ctx,"h").write("<br>");}function body_12(chk,ctx){return chk.write("<div class=\"small-11 ellipsis\"><a href=\"").helper("urlParser",ctx,{},{"url":ctx.get("ProviderUrl")}).write("\" target=\"_blank\" rel=\"external\">").reference(ctx.get("ProviderUrl"),ctx,"h").write("</a></div>");}function body_13(chk,ctx){return chk.write("<div class=\"row\"><div class=\"small-12 columns\"><span class=\"hilite\">Deadline Information</span>").exists(ctx.get("FreeTextDeadlineDate"),ctx,{"block":body_14},null).write("</div></div>");}function body_14(chk,ctx){return chk.write("<br><pre>").reference(ctx.get("FreeTextDeadlineDate"),ctx,"h").write("</pre>");}function body_15(chk,ctx){return chk.exists(ctx.getPath(false,["Scholarship","ProviderUrl"]),ctx,{"block":body_16},null);}function body_16(chk,ctx){return chk.write("<a href=\"").helper("urlParser",ctx,{},{"url":ctx.getPath(false,["Scholarship","ProviderUrl"])}).write("\" class=\"button base-btn main-btn right\" target=\"_blank\" rel=\"external\">Apply Now</a><br>");}function body_17(chk,ctx){return chk.write("<a href=\"").helper("urlParser",ctx,{},{"url":ctx.get("ApplicationUrl")}).write("\" class=\"button base-btn main-btn right\" target=\"_blank\" rel=\"external\">Apply Now</a><br>");}return body_0;})(); });