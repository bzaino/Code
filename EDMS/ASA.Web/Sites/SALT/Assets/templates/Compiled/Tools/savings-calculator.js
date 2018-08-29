define(["dust"], function(dust) { (function(){dust.register("Tools/savings-calculator",body_0);function body_0(chk,ctx){return chk.write("<link type=\"text/css\" href=\"/Tools/css/all.css\" media=\"screen\" rel=\"stylesheet\" /><link type=\"text/css\" href=\"/Tools/css/main.css\" media=\"screen\" rel=\"stylesheet\" /><link type=\"text/css\" href=\"/Tools/css/quiz.css\" media=\"screen\" rel=\"stylesheet\" /><link type=\"text/css\" href=\"/Tools/css/calculators.css\" media=\"screen\" rel=\"stylesheet\" /><link type=\"text/css\" href=\"/Assets/css/plugins/dropkick.css\" media=\"screen\" rel=\"stylesheet\" /><script>require(['jquery.formatCurrency','Tools/helpers','Tools/validation','Tools/savingsCalculator'])</script><div class=\"savingsCalculator\"><div class=\"tool-wrap clearfix calculator\"><div class=\"stage-wrap ").exists(ctx.get("blockedContent"),ctx,{"block":body_1},null).write("\"><div class=\"slides-wrapper\"><div class=\"slide\"><div class=\"slide-title clearfix\"><ul class=\"title-icons\"></ul><h1>Savings Calculator</h1></div><div class=\"slide-content clearfix\"><div class=\"tool-inputs-wrap slide-left\"><div class=\"pad-it clearfix\"><p class=\"calc-title\">At an annual interest rate of <span class=\"special-input-container\"><input type=\"text\" value=\"0\" name=\"rate\" class=\"percentage tool-input inlineInput pad-right\" maxlength=\"5\" tabindex=\"1\" /></span><br> what happens if I:</p><ul><li class=\"calc-subtitle\">Make a one time deposit of <input type=\"text\" value=\"0\" name=\"onetime\" class=\"tool-input money inlineInput\" maxlength=\"8\" tabindex=\"2\" placeholder=\"$\"/></li><li class=\"calc-subtitle\"><p class=\"float-inline\"><span>Deposit</span><span><input type=\"text\" value=\"0\" name=\"recurring\" class=\"tool-input money\" maxlength=\"8\" tabindex=\"3\" placeholder=\"$\"/></span><span> every</span><div class=\"styled-select\"><select name=\"payment-schedule\" id=\"payment-schedule\" tabindex=\"4\"><option value=\"week\">Week</option><option value=\"two-weeks\">Two Weeks</option><option value=\"month\">Month</option><option value=\"three-months\">Three Months</option><option value=\"six-months\">Six Months</option><option value=\"year\">Year</option></select></div></p></li></ul></div></div><div class=\"totals-wrap slide-right totals\"><div class=\"active-wrapper clearfix\"><div class=\"totals-group clearfix\"><div id=\"five-years\" class=\"total-section\"><h3>5 years</h3><p class=\"total\"></p><p class=\"total-interest\"></p><p class=\"interest-header\">In interest</p></div><div id=\"ten-years\"class=\"total-section\"><h3>10 years</h3><p class=\"total\"></p><p class=\"total-interest\"></p><p class=\"interest-header\">In interest</p></div><div id=\"twenty-years\"class=\"total-section\"><h3>20 years</h3><p class=\"total\"></p><p class=\"total-interest\"></p><p class=\"interest-header\">In interest</p></div></div><div class=\"additional-info clearfix\"><p>Learn more about <a href=\"/content/media/Article/how-do-banks-work/_/R-101-2200\">how banks can work for you.</a></p></div><div class=\"salt-logo-wrapper\"><div class=\"salt-tool-logo\"></div></div></div><div class=\"inactive-wrapper\"><p>Enter your info, and we'll tell you how much it could be worth.</p></div></div></div></div></div></div><div class=\"slide title-wrapper\"><h1>Savings Calculator</h1><h3>When it comes to saving money, small differences in your regular deposits can make a big impact in the interest you receive over time. This calculator can help you see how these little changes can affect how your money grows.</h3><a href=\"#\" id=\"startBtn\">Start</a><!-- <div class=\"salt-logo-wrapper\"><div class=\"salt-tool-logo\"></div></div> --></div></div></div>");}function body_1(chk,ctx){return chk.write(" noprint");}return body_0;})(); });