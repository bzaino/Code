/*global describe, before, after, it, chai */
define([
    'jquery',
    'salt/registration'
], function ($, registration) {

    var assert = chai.assert;
    //var chai = require('chai');
    //chai.Assertion.includeStack = true;

    var testableHTML = '<form action="" autocomplete="off" id="regForm" method="post" name="regForm" class=""><fieldset  class="primaryForm reg" id="fs1"><input id="inviteToken" name="inviteToken" type="hidden" value=""/><div class="row"><input id="FirstName" placeholder="First Name" class="twelve" name="FirstName" type="text" value="" tabindex="11" data-val-required="Legal First Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Legal First Name must be between 1 and 30 characters long." data-val="true" title=""/><input id="IndividualId" name="IndividualId" type="hidden" value=""/><input id="LastName" placeholder="Last Name" class="twelve" name="LastName" type="text" value="" tabindex="12" data-val-required="Last Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Last Name must be between 1 and 30 characters long." data-val="true"/><span class="field-validation-valid error"></span></div><div class="row"><input id="EmailAddress" placeholder="Email" class="twelve" name="EmailAddress" tabindex="13" type="text" title="This will be used as your SALT username." data-val-required="Confirm Email Address is required!" data-val-equalto-other="*.EmailAddress" data-val-equalto="The email address and confirmation email address do not match." data-val="true"/><input placeholder="Confirm Email" autocomplete="off" class="twelve" id="EmailAddressConfirm" name="EmailAddressConfirm" tabindex="14" type="text" value=""/></div><div class="password-container row"><div class="meter-txt twelve"><span class="meter-txt"></span><input id="Password" placeholder="Password" class="twelve  strong-password meter" title="Your password must be at least 8 characters. To make your password more secure: Don\'t use your name or a similar easy to guess word, mix capital and lower case letters, use both letters and numbers, add a special character (such as !,@,#,$,*)." type="password" tabindex="15" name="Password" data-val-required="Password is required!" data-val-length-min="8" data-val-length-max="32" data-val-length="The Password must be between 8 and 32 characters long." data-val="true" autocomplete="off" value=""  /></div><input id="ConfirmPassword" placeholder="Confirm Password" class="twelve columns strong-password" type="password" tabindex="16" name="ConfirmPassword" data-val-required="Confirm Password is required!" data-val-equalto-other="*.Password" data-val-equalto="The password and confirmation password do not match." data-val="true" autocomplete="off" value="" /></div><span class="error-msg"></span><input id="OECode" name="OECode" type="hidden" value=""/><div class="selectGroup row"><div id="schoolSearch twelve"><input type="text" autocomplete="off" class="inputWidth twelve columms" id="schoolFilter" name="schoolFilter" value="" tabindex="17" placeholder="School Name (Recommended)" title="Get extra benefits based on the college you attend/attended."/><input id="OEBranch" name="OEBranch" type="hidden" value=""/><div id="schoolsContainer" class="hidden"><ul id="schoolsContainerList"><!-- list of schools gets populated here --></ul><div id="schoolPager" style=" background-color:#ededed"><table border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="left" width="75px"><a id="pagerPrevious_1352479567038" href="#" onclick="return false; " style="display: none; ">&lt; Previous</a></td><td align="center"><div id="pagerMessage_1352479567038"></div></td><td align="right" width="75px"><a id="pagerNext_1352479567038" href="#" onclick="return false; "> Next &gt;</a></td></tr></tbody></table></div></div></div><div class="l-noSchool"><input type="checkbox" id="noSchoolCheckBox" name="noSchoolCheckBox" value="no" tabindex="18"/><label for="noSchoolCheckBox" class="checkLabel eleven"><span><em>Not affiliated with any school</em></span></label></div></div><div class="contactPref twelve"><label class="mainLabel"><strong>Contact Preferences</strong><br/></label><input type="checkbox" checked="checked" name="myContactPref" id="myContactPref" value="true" tabindex="19"/><label for="myContactPref" class="two-liner checkLabel eleven"><p><span>Send me tips for managing my finances and ways to save money on the things I need.</span></p></label></div><div id="formTerms"><div class="agree"><input data-val="true" data-val-required="You must agree to ASA&amp;#39;s Terms of Use and Privacy Policy." id="AcceptTerms" name="AcceptTerms" tabindex="20" type="checkbox" value="true"/><input class="regCheck" name="AcceptTerms" type="hidden" value="false"/><label for="AcceptTerms" class="checkLabel" id="agreeLabel">I agree to SALT\'s <span><strong><a href="/Home/terms.html" class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/terms.html\');" tabindex="21" >Terms of Use</a></strong></span> and <span><strong><a href="/Home/privacy.html"  class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/privacy.html\');" tabindex="22" >Privacy Policy</a></strong></span></label></div></div><div class="error" id="regErrorCode"></div><div class="hidden" id="email-reused" name="email-reused"><br/>Please log in <a class="loginButton" href="#">here</a> or reset your password <a class="forgotPassButton pink-link" href="#"> here</a>.</div><button class="centered button main-btn registrationSubmit twelve" type="submit" tabindex="23" onclick="dcsMultiTrack(\'WT.z_type\',\'Tool Usage\', \'WT.si_n\', regWTTag,\'WT.si_x\', \'15\');"><span>GET STARTED</span></button></fieldset></form>';
    $('#fixtures').html(testableHTML);

    describe('DOM: Registration', function () {
        describe('508: tabindexing', function () {
            it('Check that regForm is in the dom', function () {
                var testableHTML = '<form action="" autocomplete="off" id="regForm" method="post" name="regForm" class=""><fieldset  class="primaryForm reg" id="fs1"><input id="inviteToken" name="inviteToken" type="hidden" value=""/><div class="row"><input id="FirstName" placeholder="First Name" class="twelve" name="FirstName" type="text" value="" tabindex="11" data-val-required="Legal First Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Legal First Name must be between 1 and 30 characters long." data-val="true" title=""/><input id="IndividualId" name="IndividualId" type="hidden" value=""/><input id="LastName" placeholder="Last Name" class="twelve" name="LastName" type="text" value="" tabindex="12" data-val-required="Last Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Last Name must be between 1 and 30 characters long." data-val="true"/><span class="field-validation-valid error"></span></div><div class="row"><input id="EmailAddress" placeholder="Email" class="twelve" name="EmailAddress" tabindex="13" type="text" title="This will be used as your SALT username." data-val-required="Confirm Email Address is required!" data-val-equalto-other="*.EmailAddress" data-val-equalto="The email address and confirmation email address do not match." data-val="true"/><input placeholder="Confirm Email" autocomplete="off" class="twelve" id="EmailAddressConfirm" name="EmailAddressConfirm" tabindex="14" type="text" value=""/></div><div class="password-container row"><div class="meter-txt twelve"><span class="meter-txt"></span><input id="Password" placeholder="Password" class="twelve  strong-password meter" title="Your password must be at least 8 characters. To make your password more secure: Don\'t use your name or a similar easy to guess word, mix capital and lower case letters, use both letters and numbers, add a special character (such as !,@,#,$,*)." type="password" tabindex="15" name="Password" data-val-required="Password is required!" data-val-length-min="8" data-val-length-max="32" data-val-length="The Password must be between 8 and 32 characters long." data-val="true" autocomplete="off" value=""  /></div><input id="ConfirmPassword" placeholder="Confirm Password" class="twelve columns strong-password" type="password" tabindex="16" name="ConfirmPassword" data-val-required="Confirm Password is required!" data-val-equalto-other="*.Password" data-val-equalto="The password and confirmation password do not match." data-val="true" autocomplete="off" value="" /></div><span class="error-msg"></span><input id="OECode" name="OECode" type="hidden" value=""/><div class="selectGroup row"><div id="schoolSearch twelve"><input type="text" autocomplete="off" class="inputWidth twelve columms" id="schoolFilter" name="schoolFilter" value="" tabindex="17" placeholder="School Name (Recommended)" title="Get extra benefits based on the college you attend/attended."/><input id="OEBranch" name="OEBranch" type="hidden" value=""/><div id="schoolsContainer" class="hidden"><ul id="schoolsContainerList"><!-- list of schools gets populated here --></ul><div id="schoolPager" style=" background-color:#ededed"><table border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="left" width="75px"><a id="pagerPrevious_1352479567038" href="#" onclick="return false; " style="display: none; ">&lt; Previous</a></td><td align="center"><div id="pagerMessage_1352479567038"></div></td><td align="right" width="75px"><a id="pagerNext_1352479567038" href="#" onclick="return false; "> Next &gt;</a></td></tr></tbody></table></div></div></div><div class="l-noSchool"><input type="checkbox" id="noSchoolCheckBox" name="noSchoolCheckBox" value="no" tabindex="18"/><label for="noSchoolCheckBox" class="checkLabel eleven"><span><em>Not affiliated with any school</em></span></label></div></div><div class="contactPref twelve"><label class="mainLabel"><strong>Contact Preferences</strong><br/></label><input type="checkbox" checked="checked" name="myContactPref" id="myContactPref" value="true" tabindex="19"/><label for="myContactPref" class="two-liner checkLabel eleven"><p><span>Send me tips for managing my finances and ways to save money on the things I need.</span></p></label></div><div id="formTerms"><div class="agree"><input data-val="true" data-val-required="You must agree to ASA&amp;#39;s Terms of Use and Privacy Policy." id="AcceptTerms" name="AcceptTerms" tabindex="20" type="checkbox" value="true"/><input class="regCheck" name="AcceptTerms" type="hidden" value="false"/><label for="AcceptTerms" class="checkLabel" id="agreeLabel">I agree to SALT\'s <span><strong><a href="/Home/terms.html" class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/terms.html\');" tabindex="21" >Terms of Use</a></strong></span> and <span><strong><a href="/Home/privacy.html"  class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/privacy.html\');" tabindex="22" >Privacy Policy</a></strong></span></label></div></div><div class="error" id="regErrorCode"></div><div class="hidden" id="email-reused" name="email-reused"><br/>Please log in <a class="loginButton" href="#">here</a> or reset your password <a class="forgotPassButton pink-link" href="#"> here</a>.</div><button class="centered button main-btn registrationSubmit twelve" type="submit" tabindex="23" onclick="dcsMultiTrack(\'WT.z_type\',\'Tool Usage\', \'WT.si_n\', regWTTag,\'WT.si_x\', \'15\');"><span>GET STARTED</span></button></fieldset></form>';
                $('#fixtures').append(testableHTML);

                assert.equal($('#regForm').length, 1);
                $('#fixtures').html('');
            });
            it('Check that cbox is not in the dom', function () {

                var testableHTML = '<form action="" autocomplete="off" id="regForm" method="post" name="regForm" class=""><fieldset  class="primaryForm reg" id="fs1"><input id="inviteToken" name="inviteToken" type="hidden" value=""/><div class="row"><input id="FirstName" placeholder="First Name" class="twelve" name="FirstName" type="text" value="" tabindex="11" data-val-required="Legal First Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Legal First Name must be between 1 and 30 characters long." data-val="true" title=""/><input id="IndividualId" name="IndividualId" type="hidden" value=""/><input id="LastName" placeholder="Last Name" class="twelve" name="LastName" type="text" value="" tabindex="12" data-val-required="Last Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Last Name must be between 1 and 30 characters long." data-val="true"/><span class="field-validation-valid error"></span></div><div class="row"><input id="EmailAddress" placeholder="Email" class="twelve" name="EmailAddress" tabindex="13" type="text" title="This will be used as your SALT username." data-val-required="Confirm Email Address is required!" data-val-equalto-other="*.EmailAddress" data-val-equalto="The email address and confirmation email address do not match." data-val="true"/><input placeholder="Confirm Email" autocomplete="off" class="twelve" id="EmailAddressConfirm" name="EmailAddressConfirm" tabindex="14" type="text" value=""/></div><div class="password-container row"><div class="meter-txt twelve"><span class="meter-txt"></span><input id="Password" placeholder="Password" class="twelve  strong-password meter" title="Your password must be at least 8 characters. To make your password more secure: Don\'t use your name or a similar easy to guess word, mix capital and lower case letters, use both letters and numbers, add a special character (such as !,@,#,$,*)." type="password" tabindex="15" name="Password" data-val-required="Password is required!" data-val-length-min="8" data-val-length-max="32" data-val-length="The Password must be between 8 and 32 characters long." data-val="true" autocomplete="off" value=""  /></div><input id="ConfirmPassword" placeholder="Confirm Password" class="twelve columns strong-password" type="password" tabindex="16" name="ConfirmPassword" data-val-required="Confirm Password is required!" data-val-equalto-other="*.Password" data-val-equalto="The password and confirmation password do not match." data-val="true" autocomplete="off" value="" /></div><span class="error-msg"></span><input id="OECode" name="OECode" type="hidden" value=""/><div class="selectGroup row"><div id="schoolSearch twelve"><input type="text" autocomplete="off" class="inputWidth twelve columms" id="schoolFilter" name="schoolFilter" value="" tabindex="17" placeholder="School Name (Recommended)" title="Get extra benefits based on the college you attend/attended."/><input id="OEBranch" name="OEBranch" type="hidden" value=""/><div id="schoolsContainer" class="hidden"><ul id="schoolsContainerList"><!-- list of schools gets populated here --></ul><div id="schoolPager" style=" background-color:#ededed"><table border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="left" width="75px"><a id="pagerPrevious_1352479567038" href="#" onclick="return false; " style="display: none; ">&lt; Previous</a></td><td align="center"><div id="pagerMessage_1352479567038"></div></td><td align="right" width="75px"><a id="pagerNext_1352479567038" href="#" onclick="return false; "> Next &gt;</a></td></tr></tbody></table></div></div></div><div class="l-noSchool"><input type="checkbox" id="noSchoolCheckBox" name="noSchoolCheckBox" value="no" tabindex="18"/><label for="noSchoolCheckBox" class="checkLabel eleven"><span><em>Not affiliated with any school</em></span></label></div></div><div class="contactPref twelve"><label class="mainLabel"><strong>Contact Preferences</strong><br/></label><input type="checkbox" checked="checked" name="myContactPref" id="myContactPref" value="true" tabindex="19"/><label for="myContactPref" class="two-liner checkLabel eleven"><p><span>Send me tips for managing my finances and ways to save money on the things I need.</span></p></label></div><div id="formTerms"><div class="agree"><input data-val="true" data-val-required="You must agree to ASA&amp;#39;s Terms of Use and Privacy Policy." id="AcceptTerms" name="AcceptTerms" tabindex="20" type="checkbox" value="true"/><input class="regCheck" name="AcceptTerms" type="hidden" value="false"/><label for="AcceptTerms" class="checkLabel" id="agreeLabel">I agree to SALT\'s <span><strong><a href="/Home/terms.html" class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/terms.html\');" tabindex="21" >Terms of Use</a></strong></span> and <span><strong><a href="/Home/privacy.html"  class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/privacy.html\');" tabindex="22" >Privacy Policy</a></strong></span></label></div></div><div class="error" id="regErrorCode"></div><div class="hidden" id="email-reused" name="email-reused"><br/>Please log in <a class="loginButton" href="#">here</a> or reset your password <a class="forgotPassButton pink-link" href="#"> here</a>.</div><button class="centered button main-btn registrationSubmit twelve" type="submit" tabindex="23" onclick="dcsMultiTrack(\'WT.z_type\',\'Tool Usage\', \'WT.si_n\', regWTTag,\'WT.si_x\', \'15\');"><span>GET STARTED</span></button></fieldset></form>';
                $('#fixtures').append(testableHTML);

                assert.equal($('.cbox').length, 0);
                $('#fixtures').html('');
            });
            it('This will check each form element appears in the DOM', function () {
                var testableHTML = '<form action="" autocomplete="off" id="regForm" method="post" name="regForm" class=""><fieldset  class="primaryForm reg" id="fs1"><input id="inviteToken" name="inviteToken" type="hidden" value=""/><div class="row"><input id="FirstName" placeholder="First Name" class="twelve" name="FirstName" type="text" value="" tabindex="11" data-val-required="Legal First Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Legal First Name must be between 1 and 30 characters long." data-val="true" title=""/><input id="IndividualId" name="IndividualId" type="hidden" value=""/><input id="LastName" placeholder="Last Name" class="twelve" name="LastName" type="text" value="" tabindex="12" data-val-required="Last Name is required!" data-val-regex-pattern="[-a-zA-Z\s\']+" data-val-regex="Please enter a valid name!" data-val-length-min="1" data-val-length-max="30" data-val-length="The Last Name must be between 1 and 30 characters long." data-val="true"/><span class="field-validation-valid error"></span></div><div class="row"><input id="EmailAddress" placeholder="Email" class="twelve" name="EmailAddress" tabindex="13" type="text" title="This will be used as your SALT username." data-val-required="Confirm Email Address is required!" data-val-equalto-other="*.EmailAddress" data-val-equalto="The email address and confirmation email address do not match." data-val="true"/><input placeholder="Confirm Email" autocomplete="off" class="twelve" id="EmailAddressConfirm" name="EmailAddressConfirm" tabindex="14" type="text" value=""/></div><div class="password-container row"><div class="meter-txt twelve"><span class="meter-txt"></span><input id="Password" placeholder="Password" class="twelve  strong-password meter" title="Your password must be at least 8 characters. To make your password more secure: Don\'t use your name or a similar easy to guess word, mix capital and lower case letters, use both letters and numbers, add a special character (such as !,@,#,$,*)." type="password" tabindex="15" name="Password" data-val-required="Password is required!" data-val-length-min="8" data-val-length-max="32" data-val-length="The Password must be between 8 and 32 characters long." data-val="true" autocomplete="off" value=""  /></div><input id="ConfirmPassword" placeholder="Confirm Password" class="twelve columns strong-password" type="password" tabindex="16" name="ConfirmPassword" data-val-required="Confirm Password is required!" data-val-equalto-other="*.Password" data-val-equalto="The password and confirmation password do not match." data-val="true" autocomplete="off" value="" /></div><span class="error-msg"></span><input id="OECode" name="OECode" type="hidden" value=""/><div class="selectGroup row"><div id="schoolSearch twelve"><input type="text" autocomplete="off" class="inputWidth twelve columms" id="schoolFilter" name="schoolFilter" value="" tabindex="17" placeholder="School Name (Recommended)" title="Get extra benefits based on the college you attend/attended."/><input id="OEBranch" name="OEBranch" type="hidden" value=""/><div id="schoolsContainer" class="hidden"><ul id="schoolsContainerList"><!-- list of schools gets populated here --></ul><div id="schoolPager" style=" background-color:#ededed"><table border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="left" width="75px"><a id="pagerPrevious_1352479567038" href="#" onclick="return false; " style="display: none; ">&lt; Previous</a></td><td align="center"><div id="pagerMessage_1352479567038"></div></td><td align="right" width="75px"><a id="pagerNext_1352479567038" href="#" onclick="return false; "> Next &gt;</a></td></tr></tbody></table></div></div></div><div class="l-noSchool"><input type="checkbox" id="noSchoolCheckBox" name="noSchoolCheckBox" value="no" tabindex="18"/><label for="noSchoolCheckBox" class="checkLabel eleven"><span><em>Not affiliated with any school</em></span></label></div></div><div class="contactPref twelve"><label class="mainLabel"><strong>Contact Preferences</strong><br/></label><input type="checkbox" checked="checked" name="myContactPref" id="myContactPref" value="true" tabindex="19"/><label for="myContactPref" class="two-liner checkLabel eleven"><p><span>Send me tips for managing my finances and ways to save money on the things I need.</span></p></label></div><div id="formTerms"><div class="agree"><input data-val="true" data-val-required="You must agree to ASA&amp;#39;s Terms of Use and Privacy Policy." id="AcceptTerms" name="AcceptTerms" tabindex="20" type="checkbox" value="true"/><input class="regCheck" name="AcceptTerms" type="hidden" value="false"/><label for="AcceptTerms" class="checkLabel" id="agreeLabel">I agree to SALT\'s <span><strong><a href="/Home/terms.html" class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/terms.html\');" tabindex="21" >Terms of Use</a></strong></span> and <span><strong><a href="/Home/privacy.html"  class="pink-link" target="_blank" onclick="dcsMultiTrack(\'WT.z_type\',\'Link Clicks\',\'WT.z_name\',\'Internal Page Link\',\'WT.z_action\',\'/Home/privacy.html\');" tabindex="22" >Privacy Policy</a></strong></span></label></div></div><div class="error" id="regErrorCode"></div><div class="hidden" id="email-reused" name="email-reused"><br/>Please log in <a class="loginButton" href="#">here</a> or reset your password <a class="forgotPassButton pink-link" href="#"> here</a>.</div><button class="centered button main-btn registrationSubmit twelve" type="submit" tabindex="23" onclick="dcsMultiTrack(\'WT.z_type\',\'Tool Usage\', \'WT.si_n\', regWTTag,\'WT.si_x\', \'15\');"><span>GET STARTED</span></button></fieldset></form>';
                $('#fixtures').append(testableHTML);
                assert.equal($('input[tabindex=11]').length, 1);
                assert.equal($('input[tabindex=12]').length, 1);
                assert.equal($('input[tabindex=13]').length, 1);
                assert.equal($('input[tabindex=14]').length, 1);
                assert.equal($('input[tabindex=15]').length, 1);
                assert.equal($('input[tabindex=16]').length, 1);
                assert.equal($('input[tabindex=17]').length, 1);
                assert.equal($('input[tabindex=18]').length, 1);
                assert.equal($('input[tabindex=19]').length, 1);
                assert.equal($('input[tabindex=20]').length, 1);
                assert.equal($('*[tabindex=21]').length, 1);
                assert.equal($('*[tabindex=22]').length, 1);
                assert.equal($('*[tabindex=23]').length, 1);
                $('#fixtures').html('');
            });
        });
    });
});
