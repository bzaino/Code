define(["dust"], function(dust) { (function(){dust.register("partial_schoolLogo",body_0);function body_0(chk,ctx){return chk.helper("if",ctx,{"block":body_1},{"cond":body_8});}function body_1(chk,ctx){return chk.write("<div class=\"logo-row\">").helper("returnOrgLogos",ctx,{"block":body_2},null).write("</div>");}function body_2(chk,ctx){return chk.helper("math",ctx,{"block":body_3},{"key":ctx.get("orgIndex"),"method":"mod","operand":2});}function body_3(chk,ctx){return chk.helper("eq",ctx,{"else":body_4,"block":body_5},{"value":0});}function body_4(chk,ctx){return chk.write("<div class=\"js-school-logo small-6 columns school-logo\"><img src=\"/assets/images/logos/large/").reference(ctx.get("Brand"),ctx,"h").write("-web.png\" alt=\"").reference(ctx.get("OrganizationName"),ctx,"h").write(" Logo\" title=\"").reference(ctx.get("OrganizationName"),ctx,"h").write("\" class=\"vert-align-top\" /></div></div><div class=\"row logo-row\">");}function body_5(chk,ctx){return chk.helper("eq",ctx,{"else":body_6,"block":body_7},{"key":ctx.get("orgLength"),"value":"1","type":"number"}).write("</div>");}function body_6(chk,ctx){return chk.write("<div class=\"js-school-logo small-6 columns school-logo\"><img src=\"/assets/images/logos/large/").reference(ctx.get("Brand"),ctx,"h").write("-web.png\" alt=\"").reference(ctx.get("OrganizationName"),ctx,"h").write(" Logo\" title=\"").reference(ctx.get("OrganizationName"),ctx,"h").write("\" class=\"vert-align-top\" />");}function body_7(chk,ctx){return chk.write("<div class=\"js-school-logo small-12 columns centered\"><img src=\"/assets/images/logos/large/").reference(ctx.get("Brand"),ctx,"h").write("-web.png\" alt=\"").reference(ctx.get("OrganizationName"),ctx,"h").write(" Logo\" title=\"").reference(ctx.get("OrganizationName"),ctx,"h").write("\" class=\"vert-align-top\" />");}function body_8(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["SiteMember","ActiveDirectoryKey"]),ctx,"h").write("' || '").reference(ctx.getPath(false,["content","name"]),ctx,"h").write("' === 'SchoolActivation' && '").reference(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,"h").write("'");}return body_0;})(); });