define(["dust"], function(dust) { (function(){dust.register("SalaryEstimatorResults",body_0);function body_0(chk,ctx){return chk.write("Students who complete a <b>").reference(ctx.get("MajorText"),ctx,"h").write("</b> graduate degree from ").reference(ctx.get("SchoolText"),ctx,"h").write(" in ").reference(ctx.get("StateText"),ctx,"h").write(" earn an average starting salary of:<br><br><h4 class=\"row collapse\"><span class=\"small-8 columns\">Average Salary</span><span class=\"small-3 columns\">$<span class='finalAverage emphasis'></span></span></h4><br>Here are some of the most common jobs and salaries that new graduates of this program typically have:<br><hr>").section(ctx.get("JSIQuizList"),ctx,{"block":body_1},null);}function body_1(chk,ctx){return chk.write("<h4 class=\"row collapse\"><span class=\"small-8 columns\">").reference(ctx.get("OccupationName"),ctx,"h").write("<br><br></span><span class=\"small-3 columns right\">$<span class=\"average emphasis\">").reference(ctx.get("EstimatedSalaryAmount"),ctx,"h").write("</span><br><br></span><hr></h4>");}return body_0;})(); });