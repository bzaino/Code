define(["dust", "Compiled/KWYO/loanDetailNonImportedHeader", "Compiled/KWYO/KWYO_forms_sharedFields", "Compiled/KWYO/KWYO_forms_sharedFields_nonCC", "Compiled/KWYO/loanDetailContentEditorBlock", "Compiled/KWYO/loanDetailButtons"], function(dust) { (function(){dust.register("KWYO/loan_allTypes",body_0);function body_0(chk,ctx){return chk.write("<form id=\"edit-allTypes-form\" data-abide>").partial("KWYO/loanDetailNonImportedHeader",ctx,null).write("<section class=\"fields\">").partial("KWYO/KWYO_forms_sharedFields",ctx,{"required":"required","LoanLabel":"Debt Name:"}).partial("KWYO/KWYO_forms_sharedFields_nonCC",ctx,null).write("</section>").partial("KWYO/loanDetailContentEditorBlock",ctx,null).partial("KWYO/loanDetailButtons",ctx,null).write("</form>");}return body_0;})(); });