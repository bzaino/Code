define(["dust"], function(dust) { (function(){dust.register("KWYO/KWYO_forms_sharedFields_nonCC",body_0);function body_0(chk,ctx){return chk.write("<div class=\"row collapse\"><div class=\"small-12 medium-6 columns\"><label>Amount Borrowed:</label></div><div class=\"small-12 medium-6 columns prefix-div\"><span class=\"prefix\">$</span><input type=\"balance\" value=\"").reference(ctx.get("OriginalLoanAmount"),ctx,"h").write("\" name=\"OriginalLoanAmount\" class=\"prefixed\" required/><small class=\"error\">A valid amount is required.</small></div></div><div class=\"row collapse\"><div class=\"small-12 medium-6 columns\"><label>Date This Debt Was Borrowed:</label></div><div class=\"small-12 medium-6 columns\"><input type=\"date\" value=\"").reference(ctx.get("OriginalLoanDate"),ctx,"h").write("\" name=\"OriginalLoanDate\" placeholder=\"mm/dd/yyyy\" required/><small class=\"error\">A valid date is required.</small></div></div>");}return body_0;})(); });