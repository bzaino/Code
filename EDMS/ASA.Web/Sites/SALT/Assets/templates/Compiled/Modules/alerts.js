define(["dust"], function(dust) { (function(){dust.register("Modules/alerts",body_0);function body_0(chk,ctx){return chk.section(ctx.get("mainContent"),ctx,{"block":body_1},null);}function body_1(chk,ctx){return chk.write("\t    ").helper("if",ctx,{"block":body_2},{"cond":body_8});}function body_2(chk,ctx){return chk.exists(ctx.get("records"),ctx,{"else":body_3,"block":body_4},null);}function body_3(chk,ctx){return chk.write("<section data-alert class=\"hidden\" tabindex=\"0\" aria-live=\"assertive\" role=\"alertdialog\"><p>No alerts!</p><a href=\"#\" class=\"close\">&times;</a></section><!--<script src=\"Assets/Scripts/js/libs/foundation5/ReferencePlugins/foundation.alert.js\"></script>-->");}function body_4(chk,ctx){return chk.section(ctx.get("records"),ctx,{"block":body_5},null);}function body_5(chk,ctx){return chk.write("<div class=\"no-padding small-12 Content-Type_").reference(ctx.getPath(false,["attributes","ContentTypes"]),ctx,"h").write("  primary-key_").reference(ctx.getPath(false,["attributes","P_Primary_Key"]),ctx,"h").write("\">").helper("if",ctx,{"block":body_6},{"cond":body_7}).write("</div>");}function body_6(chk,ctx){return chk.reference(ctx.getPath(false,["attributes","alert-msg"]),ctx,"h");}function body_7(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["attributes","ContentTypes"]),ctx,"h").write("' === 'Alert' ");}function body_8(chk,ctx){return chk.write("'").reference(ctx.get("name"),ctx,"h").write("' === 'Alerts' ");}return body_0;})(); });