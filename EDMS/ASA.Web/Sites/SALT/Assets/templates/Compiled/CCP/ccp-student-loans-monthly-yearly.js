define(["dust"], function(dust) { (function(){dust.register("CCP/ccp-student-loans-monthly-yearly",body_0);function body_0(chk,ctx){return chk.write("            <div class=\"js-loans-by-").reference(ctx.get("type"),ctx,"h").write("\" hidden><header class=\"row collapse\"><hgroup class=\"small-12 columns\"><h3 class=\"js-").reference(ctx.get("type"),ctx,"h").write("ly-payment\"><b>Estimated ").helper("if",ctx,{"else":body_1,"block":body_2},{"cond":body_3}).write(" Payment</b></h3> </hgroup></header><div class=\"items-col\"><div class=\"row collapse\"><div class=\"small-6 columns padding-bottom\">Federal Student Loans</div><div class=\"small-6 columns nowrap text-right\"><span class=\"js-student-loans js-loans-").reference(ctx.get("type"),ctx,"h").write("ly js-loans-").reference(ctx.get("type"),ctx,"h").write("ly-fsl\" />                            </div></div><div class=\"row collapse\"><div class=\"small-6 columns padding-bottom\">Federal PLUS Loans</div><div class=\"small-6 columns nowrap text-right\"><span class=\"js-student-loans js-loans-").reference(ctx.get("type"),ctx,"h").write("ly js-loans-").reference(ctx.get("type"),ctx,"h").write("ly-fppl\" /></div></div><div class=\"row collapse\"><div class=\"small-6 columns padding-bottom\">Private/Other Loans</div><div class=\"small-6 columns nowrap text-right\"><span class=\"js-student-loans js-loans-").reference(ctx.get("type"),ctx,"h").write("ly js-loans-").reference(ctx.get("type"),ctx,"h").write("ly-other\" /></div></div></div></div>");}function body_1(chk,ctx){return chk.write("Total Annual");}function body_2(chk,ctx){return chk.write("Monthly");}function body_3(chk,ctx){return chk.write("'").reference(ctx.get("type"),ctx,"h").write("' ==='month' ");}return body_0;})(); });