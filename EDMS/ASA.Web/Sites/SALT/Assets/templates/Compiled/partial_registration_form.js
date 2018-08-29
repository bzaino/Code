define(["dust", "Compiled/ErrorMessages/name", "Compiled/ErrorMessages/name", "Compiled/ErrorMessages/username", "Compiled/ErrorMessages/password", "Compiled/ErrorMessages/Age", "Compiled/ErrorMessages/organizationName", "Compiled/ErrorMessages/GradDate", "Compiled/ErrorMessages/organizationName", "Compiled/ErrorMessages/terms", "Compiled/ErrorMessages/organizationName", "Compiled/ErrorMessages/GradDate", "Compiled/partial_organization_remove_button", "Compiled/ErrorMessages/organizationName", "Compiled/partial_organization_remove_button", "Compiled/Overlays/RegistrationStatus", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_remove_button", "Compiled/partial_organization_add_button", "Compiled/partial_organization_add_button", "Compiled/partial_organization_add_button"], function(dust) { (function(){dust.register("partial_registration_form",body_0);function body_0(chk,ctx){return chk.write("<script>require(['salt','jquery','salt/registration','salt/pschecker','modules/overlays','salt/analytics/webtrends'], function (SALT, $) {$(function () {if ($(window).width() > 640 && !$('.js-reg-overlay-identifier').length) {SALT.publish('Registration:Start');$('#FirstName').focus();} else {$('#FirstName').one('focus', function (e) {if (!$('.js-reg-overlay-identifier').length) {SALT.publish('Registration:Start');}});}});window.regWTTag = $('meta[name=\"regWTTag\"]').attr(\"content\") || 'Member Registration';").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_1},null).write("});</script><div class=\"subcontent\"><div class=\"row \"><div class=\"small-11 columns\"><header><h2>Create Your Free Account</h2></header></div></div><form action=\"\" autocomplete=\"off\" id=\"regForm\" method=\"post\" name=\"regForm\" data-abide=\"ajax\"><div class=\"row\"><div class=\"small-6  columns reg-first-name\"><input type=\"text\" pattern=\"alpha\" id=\"FirstName\" placeholder=\"First Name\" name=\"FirstName\" tabindex=\"1\" required/>").partial("ErrorMessages/name",ctx,null).write("</div><div class=\"small-6 columns reg-last-name\"><input type=\"text\" pattern=\"alpha\" id=\"LastName\" placeholder=\"Last Name\"  name=\"LastName\" value=\"\" tabindex=\"1\" required/>").partial("ErrorMessages/name",ctx,null).write("</div></div><div class=\"password-container row\"><div class=\"small-12 columns reg-email\" ><input id=\"EmailAddress\" placeholder=\"Email\" name=\"EmailAddress\" type=\"text\" pattern=\"email\" data-tooltip data-options=\"disable_for_touch:true\" class=\" has-tip tip-top\" title=\"This will be your Salt username.\" tabindex=\"1\" required />").partial("ErrorMessages/username",ctx,null).write("</div><div class=\"meter-txt small-12 columns reg-password relative\"><span class=\"meter-txt\"></span><input id=\"Password\" placeholder=\"Password\" class=\"strong-password meter has-tip tip-top\" data-tooltip data-options=\"disable_for_touch:true\" title=\"Your password must be at least eight characters. We recommend mixing letters, numbers, and special characters to increase its security.\" type=\"password\" name=\"Password\" pattern=\"password\" tabindex=\"1\" required/> <div class=\"js-pw-show-hide pw-show-hide\"><small><a class=\"js-show-hide-text js-void-href\" href=\"\" tabindex=\"1\" >Show</a></small></div>").partial("ErrorMessages/password",ctx,null).write("</div><p><span class=\"error-msg small error\"></span></p><input id=\"OECode\" name=\"OECode\" type=\"hidden\" value=\"\"/></div><div class=\"row\"><div class=\"small-12 columns reg-yob\"><div class=\"styled-select\"><select id=\"YearOfBirth\" name=\"YearOfBirth\" required tabindex=\"1\"><option value=\"\">Year Of Birth</option>").helper("yearDropdown",ctx,{},{"startYear":"-5","range":"-104"}).write("</select>").partial("ErrorMessages/Age",ctx,null).write("</div></div></div> <div class=\"row padding-top\"><div id=\"AffiliationSection\" class=\"reg-school-search small-12 columns\"><h3 class=\"js-select-aff-msg\">Select Your Affiliations <em>(up to two)</em></h3><div id=\"rb-container\" class=\"row padding-bottom collapse").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_3},null).write("\"><div class=\"small-4 columns margin-right-adj\"><input type=\"radio\" ").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"else":body_4,"block":body_5},null).write("name=\"affiliationRadioBtn\" id=\"School\" tabindex=\"1\" value=\"School\" class=\"css-checkbox\"/><label class=\"").exists(ctx.get("rapidRegCssLabel"),ctx,{"else":body_7,"block":body_8},null).write("\" for=\"School\">School</label></div><div class=\"small-4 columns\"><input type=\"radio\" ").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_9},null).write("name=\"affiliationRadioBtn\" id=\"Organization\" tabindex=\"1\" value=\"Organization\" class=\"css-checkbox\"/><label class=\"").exists(ctx.get("rapidRegCssLabel"),ctx,{"else":body_11,"block":body_12},null).write("\" for=\"Organization\">Organization</label></div><div class=\"small-4 columns margin-right-adj2\"><input type=\"radio\" name=\"affiliationRadioBtn\" id=\"Neither\" tabindex=\"1\" value=\"Neither\" class=\"css-checkbox\"/><label class=\"").exists(ctx.get("rapidRegCssLabel"),ctx,{"else":body_13,"block":body_14},null).write("\" for=\"Neither\">Neither</label></div></div><div class=\"js-organization js-firstAffiliation js-SCHL ").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_15},null).write("\"><div class=\"row collapse\"><div class=\"small-12 columns\"><div class=\"js-organization-search\"><div><input id=\"qa-sch-1\" type=\"text\" autocomplete=\"off\" class=\"has-tip tip-top SCHL organizationFilter js-org\" name=\"organizationFilter\" value=\"").exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_17},null).write("\" tabindex=\"1\" placeholder=\"School Name\" data-tooltip data-options=\"disable_for_touch:true\" title=\"Access special content and benefits based on our partnership with your school.\" data-abide-validator=\"organizationLookup\"/>").partial("ErrorMessages/organizationName",ctx,{"organizationValidation":"a school"}).write("</div><input class=\"OEBranch\" name=\"OEBranch\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_18},null).write("\"/><input class=\"OECode\" name=\"OECode\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_20},null).write("\" /><input class=\"SchoolType\" name=\"SchoolType\" type=\"hidden\" value=\"\" /><input class=\"OrganizationId\" name=\"OrganizationId\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_22},null).write("\" /></div></div></div><div class=\"row collapse\"><div class=\"small-12 columns\"><div class=\"styled-select\"><select id=\"qa-sch-yr-1\" class=\"yearDropdown js-org\" name=\"GraduationDate\" required tabindex=\"1\"><option value=\"\">Graduation Year</option>").helper("yearDropdown",ctx,{},{"startYear":"15","range":"-99","gradDate":body_24}).write("<option value=\"1900\" ").helper("checkGradYear",ctx,{},{"gradDate":body_25}).write(">Unknown</option></select>").partial("ErrorMessages/GradDate",ctx,null).write("</div></div></div>").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"else":body_26,"block":body_27},null).write("<h3 class=\"js-pre-popped-msg hidden\">Add a Second Affiliation <em>(if applicable)</em></h3></div><div class=\"js-organization js-firstAffiliation js-WELL ").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"else":body_30,"block":body_31},null).write("\"><div class=\"row collapse\"><div class=\"small-12 columns\"><div class=\"js-organization-search\"><div><input id=\"qa-org-1\" type=\"text\" autocomplete=\"off\" class=\"has-tip tip-top WELL organizationFilter js-org no-bottom-margin\" name=\"organizationFilter\"  tabindex=\"1\" value=\"").notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_33},null).write("\"  placeholder=\"Organization Name\" data-tooltip data-options=\"disable_for_touch:true\" title=\"Access special content and benefits based on our partnership with your community organization, credit union, or college access institution.\" data-abide-validator=\"organizationLookup\"/>").partial("ErrorMessages/organizationName",ctx,{"organizationValidation":"an organization"}).write("</div><input class=\"OEBranch\" name=\"OEBranch\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_34},null).write("\"/><input class=\"OECode\" name=\"OECode\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_36},null).write("\" /><input class=\"SchoolType\" name=\"SchoolType\" type=\"hidden\" value=\"\" /><input class=\"OrganizationId\" name=\"OrganizationId\" type=\"hidden\" value=\"").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"block":body_38},null).write("\" /></div></div></div>").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"else":body_40,"block":body_41},null).write("<h3 class=\"js-pre-popped-msg hidden\">Add a Second Affiliation <em>(if applicable)</em></h3></div><div id=\"noAffiliationInfo\" class=\"hidden\"><p class=\"small-i\">You can always add a school or organization later to access special content and benefits.</p></div>").exists(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,{"else":body_44,"block":body_45},null).write("</div></div><div class=\"row padding-top\"><div class=\"small-12 columns\"><input type=\"checkbox\" checked=\"checked\" name=\"myContactPref\" id=\"myContactPref\" class=\"css-checkbox\" value=\"true\" tabindex=\"1\"/><label for=\"myContactPref\" class=\"").exists(ctx.get("rapidRegCssLabel"),ctx,{"else":body_48,"block":body_49},null).write("\"><p><small>Send me tips and guidance about money.</small></p></label></div></div><div class=\"row reg-agree padding-top\"><div class=\"small-12 columns\"><input id=\"AcceptTerms\" name=\"AcceptTerms\" type=\"checkbox\" class=\"css-checkbox\" required tabindex=\"1\" /><label for=\"AcceptTerms\" class=\"").exists(ctx.get("rapidRegCssLabel"),ctx,{"else":body_50,"block":body_51},null).write("\"><p><small>By selecting 'Sign Up',  I acknowledge that I have read and agree to Salt's <a href=\"/Home/terms.html\" tabindex=\"1\" target=\"_blank\" onclick=\"dcsMultiTrack('WT.z_type','Link Clicks','WT.z_name','Internal Page Link','WT.z_action','/Home/terms.html');\">Terms Of Use</a> and <a href=\"/Home/privacy.html\" tabindex=\"1\" target=\"_blank\" onclick=\"dcsMultiTrack('WT.z_type','Link Clicks','WT.z_name','Internal Page Link','WT.z_action','/Home/privacy.html');\">Privacy Policy</a>.</small></p></label>").partial("ErrorMessages/terms",ctx,null).write("<div class=\"error server-error js-server-error-text\" id=\"regErrorCode\"> </div><div class=\"hidden error server-error\" id=\"email-reused\" name=\"email-reused\">Please log in <a href=\"\" class=\"js-void-href loginButton\" data-window-shade=\"loginOverlay\">here</a> or reset your password <a class=\"forgotPassButton js-void-href\" data-window-shade=\"forgotPasswordOverlay\" href=\"\">here</a>.</div></div></div><div class=\"row\"><div class=\"small-12  columns reg-submit\"><button class=\"button base-btn main-btn registrationSubmit\" type=\"submit\"  tabindex=\"1\" onclick=\"dcsMultiTrack('WT.z_type','Tool Usage', 'WT.si_n', regWTTag, 'WT.z_actname', '', 'WT.z_acttype', '', 'WT.z_acttx', '', 'WT.si_x', '12');\" disabled><span>").exists(ctx.get("btnRegSpecialEventText"),ctx,{"else":body_52,"block":body_53},null).write("</span></button></div></div><div class=\"row\"><div class=\"small-12 columns already-registered\"><small class=\"show-on-overlay\">Need help? <a href=\"/home/contact.html\" tabindex=\"1\" >Contact Us</a>.</small><small>Already registered? <a data-window-shade=\"loginOverlay\" href=\"\" class=\"js-void-href registrationOverlay-link loginButton\" tabindex=\"1\">Log in</a><!-- SWD-7217 Route 'small screen' users to logon page instead of logon overlay --><a href=\"/logon\" class=\"registration-link\" tabindex=\"1\">Log in</a>.</small></div></div><input id=\"inviteToken\" name=\"inviteToken\" type=\"hidden\" value=\"\"/><input id=\"IndividualId\" name=\"IndividualId\" type=\"hidden\" value=\"\"/><input id=\"SpecialEvent\" name=\"SpecialEvent\" type=\"hidden\" value=\"\"/><input id=\"OEBranch\" name=\"OEBranch\" type=\"hidden\" value=\"\"/><input id=\"OrganizationId\" name=\"OrganizationId\" type=\"hidden\" value=\"\"/><input class=\"regCheck\" name=\"AcceptTerms\" type=\"hidden\" value=\"false\" /><input id=\"registration-source\" name=\"Source\" type=\"hidden\" value=\"").reference(ctx.get("RegistrationSource"),ctx,"h").write("\" /></form><div class=\"affiliationTemplates hidden\"><div class=\"schoolAddTemplate\"><div class=\"js-organization js-newAffiliation js-SCHL\"><div class=\"row collapse\"><div class=\"small-12 columns\"><div class=\"js-organization-search\"><div><input type=\"text\" autocomplete=\"off\" tabindex=\"1\" class=\"no-bottom-margin has-tip tip-top SCHL organizationFilter js-org\" data-options=\"disable_for_touch:true\" name=\"organizationFilter\" value=\"\"  placeholder=\"School Name\" data-tooltip title=\"Access special content and benefits based on our partnership with your school.\" data-abide-validator=\"organizationLookup\"/>").partial("ErrorMessages/organizationName",ctx,{"organizationValidation":"a school"}).write("</div><input class=\"OEBranch\" name=\"OEBranch\" type=\"hidden\" value=\"\"/><input class=\"OECode\" name=\"OECode\" type=\"hidden\" value=\"\" /><input class=\"SchoolType\" name=\"SchoolType\" type=\"hidden\" value=\"\" /><input class=\"OrganizationId\" name=\"OrganizationId\" type=\"hidden\" value=\"\" /></div></div></div><!-- Grad Year new school--><div class=\"row padding-top collapse\"><div class=\"small-12 columns\"><div class=\"styled-select\"><select class=\"yearDropdown js-org\" name=\"GraduationDate\" required tabindex=\"1\"><option value=\"\">Graduation Year</option>").helper("yearDropdown",ctx,{},{"startYear":"15","range":"-99","gradDate":body_54}).write("<option value=\"1900\" ").helper("checkGradYear",ctx,{},{"gradDate":body_55}).write(">Unknown</option></select>").partial("ErrorMessages/GradDate",ctx,null).write("</div></div></div>").partial("partial_organization_remove_button",ctx,{"tabIndex":"1"}).write("</div></div><div class=\"organizationAddTemplate\"><div class=\"js-organization js-newAffiliation js-WELL\"><div class=\"row collapse\"><div class=\"small-12 columns\"><div class=\"js-organization-search\"><div><input type=\"text\" autocomplete=\"off\" tabindex=\"1\" class=\"no-bottom-margin has-tip tip-top WELL organizationFilter js-org\" data-options=\"disable_for_touch:true\" name=\"organizationFilter\" value=\"\" placeholder=\"Organization Name\" data-tooltip title=\"Access special content and benefits based on our partnership with your community organization, credit union, or college access institution.\" data-abide-validator=\"organizationLookup\"/>").partial("ErrorMessages/organizationName",ctx,{"organizationValidation":"an organization"}).write("</div><input class=\"OEBranch\" name=\"OEBranch\" type=\"hidden\" value=\"\"/><input class=\"OECode\" name=\"OECode\" type=\"hidden\" value=\"\" /><input class=\"SchoolType\" name=\"SchoolType\" type=\"hidden\" value=\"\" /><input class=\"OrganizationId\" name=\"OrganizationId\" type=\"hidden\" value=\"\" /> </div></div></div>").partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":"padding-top"}).write("</div></div></div></div>").partial("Overlays/RegistrationStatus",ctx,null);}function body_1(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_2},null);}function body_2(chk,ctx){return chk.write(" $('.js-organization.js-firstAffiliation.js-SCHL').find('.yearDropdown').prop('required', false); ");}function body_3(chk,ctx){return chk.write(" hidden");}function body_4(chk,ctx){return chk.write(" CHECKED ");}function body_5(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_6},null);}function body_6(chk,ctx){return chk.write(" CHECKED ");}function body_7(chk,ctx){return chk.write("css-label");}function body_8(chk,ctx){return chk.reference(ctx.get("rapidRegCssLabel"),ctx,"h");}function body_9(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_10},null);}function body_10(chk,ctx){return chk.write(" CHECKED ");}function body_11(chk,ctx){return chk.write("css-label");}function body_12(chk,ctx){return chk.reference(ctx.get("rapidRegCssLabel"),ctx,"h");}function body_13(chk,ctx){return chk.write("css-label");}function body_14(chk,ctx){return chk.reference(ctx.get("rapidRegCssLabel"),ctx,"h");}function body_15(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_16},null);}function body_16(chk,ctx){return chk.write("hidden");}function body_17(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,"h");}function body_18(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_19},null);}function body_19(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OEBranch"]),ctx,"h");}function body_20(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_21},null);}function body_21(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OECode"]),ctx,"h");}function body_22(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_23},null);}function body_23(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OrgId"]),ctx,"h");}function body_24(chk,ctx){return chk.reference(ctx.getPath(true,["ExpectedGraduationYear"]),ctx,"h");}function body_25(chk,ctx){return chk.reference(ctx.get("ExpectedGraduationYear"),ctx,"h");}function body_26(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":"hidden"});}function body_27(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"else":body_28,"block":body_29},null);}function body_28(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":"padding-top"});}function body_29(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":""});}function body_30(chk,ctx){return chk.write("hidden");}function body_31(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_32},null);}function body_32(chk,ctx){return chk.write("hidden");}function body_33(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","CurrentSchool"]),ctx,"h");}function body_34(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_35},null);}function body_35(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OEBranch"]),ctx,"h");}function body_36(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_37},null);}function body_37(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OECode"]),ctx,"h");}function body_38(chk,ctx){return chk.notexists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"block":body_39},null);}function body_39(chk,ctx){return chk.reference(ctx.getPath(false,["configuration","OrgId"]),ctx,"h");}function body_40(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":"hidden padding-top"});}function body_41(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"else":body_42,"block":body_43},null);}function body_42(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":" padding-top"});}function body_43(chk,ctx){return chk.partial("partial_organization_remove_button",ctx,{"tabIndex":"1","class":"hidden padding-top"});}function body_44(chk,ctx){return chk.partial("partial_organization_add_button",ctx,{"tabIndex":"1","initialVisibility":"hidden"});}function body_45(chk,ctx){return chk.exists(ctx.getPath(false,["configuration","IsSchool"]),ctx,{"else":body_46,"block":body_47},null);}function body_46(chk,ctx){return chk.partial("partial_organization_add_button",ctx,{"tabIndex":"1","initialVisibility":""});}function body_47(chk,ctx){return chk.partial("partial_organization_add_button",ctx,{"tabIndex":"1","initialVisibility":"hidden"});}function body_48(chk,ctx){return chk.write("css-label");}function body_49(chk,ctx){return chk.reference(ctx.get("rapidRegCssLabel"),ctx,"h");}function body_50(chk,ctx){return chk.write("css-label");}function body_51(chk,ctx){return chk.reference(ctx.get("rapidRegCssLabel"),ctx,"h");}function body_52(chk,ctx){return chk.write("Sign Up");}function body_53(chk,ctx){return chk.reference(ctx.get("btnRegSpecialEventText"),ctx,"h");}function body_54(chk,ctx){return chk.reference(ctx.getPath(true,["ExpectedGraduationYear"]),ctx,"h");}function body_55(chk,ctx){return chk.reference(ctx.get("ExpectedGraduationYear"),ctx,"h");}return body_0;})(); });