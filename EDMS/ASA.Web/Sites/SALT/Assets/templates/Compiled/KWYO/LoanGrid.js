define(["dust"], function(dust) { (function(){dust.register("KWYO/LoanGrid",body_0);function body_0(chk,ctx){return chk.write("<tr class=\"js-loan-row\" id=\"").reference(ctx.get("LoanSelfReportedEntryId"),ctx,"h").write("\"><td class=\"hide-for-small\"><div style=\"background:").reference(ctx.get("Color"),ctx,"h").write("\" class=\"arrow-boxed-color\"><!-- <div class=\"arrow-boxed\"> --></div></div></td><td>").reference(ctx.get("TypeName"),ctx,"h").write("</td><td><a value=\"").reference(ctx.get("LoanSelfReportedEntryId"),ctx,"h").write("\" class=\"js-loanDetailLink loan-name\">").reference(ctx.get("LoanName"),ctx,"h").write("</a></td><td>$").reference(ctx.get("PrincipalBalanceOutstandingAmount"),ctx,"h",["currencyComma"]).write("</td><td>$").reference(ctx.get("MonthlyPaymentAmount"),ctx,"h",["currencyComma"]).write("</td></tr>");}return body_0;})(); });