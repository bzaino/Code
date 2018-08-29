define(["dust"], function(dust) { (function(){dust.register("Overlays/RepaymentDate",body_0);function body_0(chk,ctx){return chk.write("<div id=\"repaymentDateOverlay\" class=\"reveal-modal small\" data-reveal><div class=\"close-reveal-modal cancel-circle overlayClose\"></div><section class=\"cf\" id=\"repaymentDate\"><div class=\"left list\"><h2>What Do You Owe and When Is It Due?</h2><p>Your student loan bill contains lot of important information, but there are 2 things that matter immediately:</p><ol><li>Your monthly loan amount</li><li>When you need to pay it</li></ol><p>This information should be prominently featured on your bill. Can't find it? Check the top-left and top-right corner of the page.</p><p>If you still don’t see this information, contact your loan servicer directly.</p><a class=\"button\" id=\"closeRepaymentOverlay\"  href=\"\"><span>okay, got it</span></a></div></section></div><script type=\"text/javascript\">require(['jquery','salt','modules/overlays'], function ($, SALT) {$('#repaymentButton').click(function (e) {SALT.openOverlay('#repaymentDateOverlay', 'repaymentDate', '');});$('#closeRepaymentOverlay').click(function (e) {SALT.closeOverlay('#repaymentDateOverlay');});});</script>");}return body_0;})(); });