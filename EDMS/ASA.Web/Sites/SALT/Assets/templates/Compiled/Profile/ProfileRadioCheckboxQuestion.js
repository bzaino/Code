define(["dust", "Compiled/ErrorMessages/ProfileOtherEntry"], function(dust) { (function(){dust.register("Profile/ProfileRadioCheckboxQuestion",body_0);function body_0(chk,ctx){return chk.write("<div class=\"row ").exists(ctx.get("displayBelow"),ctx,{"block":body_1},null).write("js-question-container question-container\"><div class=\"small-12 columns prof-label-wrap\"><label for=\"Q").reference(ctx.getPath(false,["currentQuestion","QuestionID"]),ctx,"h").write("\"><span class=\"js-onboarding-question onboarding-question\">").reference(ctx.getPath(false,["currentQuestion","Question"]),ctx,"h").write("</span>").helper("if",ctx,{"block":body_2},{"cond":body_3}).write("</label></div><div class=\"small-12 columns\"><ul class=\"row\">").section(ctx.getPath(false,["currentQuestion","Answers"]),ctx,{"block":body_4},null).write("</ul></div></div>");}function body_1(chk,ctx){return chk.write("collapse double-padding-top ");}function body_2(chk,ctx){return chk.write("<br>");}function body_3(chk,ctx){return chk.write("'").reference(ctx.getPath(false,["currentQuestion","Question"]),ctx,"h").write("' !== ''");}function body_4(chk,ctx){return chk.helper("if",ctx,{"block":body_5},{"cond":body_13});}function body_5(chk,ctx){return chk.write("<li class=\"left double-padded-bottom qa-input-below\"><input type=\"").reference(ctx.get("styleType"),ctx,"h").write("\" tabindex=\"2\" name=\"").reference(ctx.get("type"),ctx,"h").write("-qid-").reference(ctx.getPath(false,["currentQuestion","QuestionID"]),ctx,"h").write("\" id=\"ans-").reference(ctx.get("AnsID"),ctx,"h").write("\" class=\"css-checkbox ").helper("profileSpecialAnswer",ctx,{},{"ansType":body_6}).write("\" value=\"").reference(ctx.get("AnsID"),ctx,"h").helper("if",ctx,{"block":body_7},{"cond":body_9}).write("\" data-segmentName=\"Dashboard-QA-").reference(ctx.getPath(false,["currentQuestion","QuestionID"]),ctx,"h").write("-").reference(ctx.get("AnsName"),ctx,"h").write("\" ").helper("isProfileInputChecked",ctx,{},{"responsesObj":ctx.getPath(false,["currentQuestion","Responses"]),"ansVal":body_10}).write(" ><label class=\"qa-label\" for=\"ans-").reference(ctx.get("AnsID"),ctx,"h").write("\">").reference(ctx.get("AnsDescription"),ctx,"h").write("</label>").helper("if",ctx,{"block":body_11},{"cond":body_12}).write("</li>");}function body_6(chk,ctx){return chk.reference(ctx.get("AnsName"),ctx,"h");}function body_7(chk,ctx){return chk.exists(ctx.getPath(false,["currentQuestion","otherAnswer"]),ctx,{"block":body_8},null);}function body_8(chk,ctx){return chk.write("-").reference(ctx.getPath(false,["currentQuestion","otherAnswer"]),ctx,"h");}function body_9(chk,ctx){return chk.write("'").reference(ctx.get("AnsName"),ctx,"h").write("' === 'Other'");}function body_10(chk,ctx){return chk.reference(ctx.get("AnsID"),ctx,"h");}function body_11(chk,ctx){return chk.write("<div class=\"other-ans ").helper("profileOtherInputVisibilty",ctx,{},{"responsesObject":ctx.getPath(false,["currentQuestion","Responses"])}).write("\"><label for=\"other-ans-").reference(ctx.get("AnsID"),ctx,"h").write("\">Other:</label><input type=\"text\" pattern=\"profile_other\" id=\"other-ans-").reference(ctx.get("AnsID"),ctx,"h").write("\" class=\"css-checkbox js-other-input\" value=\"").reference(ctx.getPath(false,["currentQuestion","otherAnswer"]),ctx,"h").write("\" data-segmentName=\"Dashboard-QA-").reference(ctx.getPath(false,["currentQuestion","QuestionID"]),ctx,"h").write("-").reference(ctx.get("AnsName"),ctx,"h").write("\" tabindex=\"2\"/>").partial("ErrorMessages/ProfileOtherEntry",ctx,null).write("</div>");}function body_12(chk,ctx){return chk.write("'").reference(ctx.get("AnsName"),ctx,"h").write("' === 'Other'");}function body_13(chk,ctx){return chk.write("'").reference(ctx.get("IsProfileAnswerActive"),ctx,"h").write("' === 'true'");}return body_0;})(); });