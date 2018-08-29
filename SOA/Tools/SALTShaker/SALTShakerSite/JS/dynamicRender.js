var DBTool = DBTool || {};
$(document).ready(function () {
    //write your code here
    DBTool.MemberDetailPage.init();
});

DBTool.MemberDetailPage = {
    init: function() {
        DBTool.MemberDetailPage.CancelBtnClick();
        DBTool.MemberDetailPage.UpdateBtnClick();
        DBTool.MemberDetailPage.SaveBtnClick();
        DBTool.MemberDetailPage.OverlayCloseCallBack();
        DBTool.MemberDetailPage.HighlightOnFocus($('#MainContent_uc_MemberDetail_TextBoxOrganization'));
    },
    CancelBtnClick: function () {
        $('#cancelBtn').click(function () {
            DBTool.MemberDetailPage.CloseOverlay();
        });
    },
    UpdateBtnClick: function () {
        $('#MainContent_uc_MemberDetail_UpdateBtn').click(function () {
            DBTool.MemberDetailPage.PrePopulateFields();
            DBTool.MemberDetailPage.Validation.init();
            //call blur to trigger reset validation when overlay pops up.
            $("#MainContent_uc_MemberDetail_TextBoxFN").blur();
            $("#MainContent_uc_MemberDetail_TextBoxLN").blur();
            $("#MainContent_uc_MemberDetail_TextBoxEA").blur();
        });
    },
    SaveBtnClick: function () {
        $('#MainContent_uc_MemberDetail_saveBtn').click(function () {
            //On Submitting
            $("#MasterForm").submit(function () {
                if (DBTool.Validation.validateFName("#MainContent_uc_MemberDetail_TextBoxFN") && DBTool.Validation.validateEmail("#MainContent_uc_MemberDetail_TextBoxEA") && DBTool.Validation.validateLName("#MainContent_uc_MemberDetail_TextBoxLN"))
                    return true
                else
                    return false;
            });
        });
    },
    OverlayCloseCallBack: function () {
        $('#myModal').on('closed', function () {
          DBTool.MemberDetailPage.PrePopulateFields();
        });
    },
    CloseOverlay: function () {
        $('#myModal').foundation('reveal', 'close');
    },
    PrePopulateFields: function () {
        var firstName = $('#MainContent_uc_MemberDetail_LabelFN').text();
        var lastName = $('#MainContent_uc_MemberDetail_LabelLN').text();
        var emailAddress = $('#MainContent_uc_MemberDetail_LabelEA').text();
        var enrollment = $('#MainContent_uc_MemberDetail_LabelES').text();
        var gradeLevel = $('#MainContent_uc_MemberDetail_LabelGL').text();
        var isActive = $('#MainContent_uc_MemberDetail_CheckBoxActive').prop('checked');
        $('#MainContent_uc_MemberDetail_TextBoxFN').val(firstName);
        $('#MainContent_uc_MemberDetail_TextBoxLN').val(lastName);
        $('#MainContent_uc_MemberDetail_TextBoxEA').val(emailAddress);
        $('#MainContent_uc_MemberDetail_TextBoxES').val(enrollment);
        $('#MainContent_uc_MemberDetail_TextBoxGL').val(gradeLevel);
        $('#MainContent_uc_MemberDetail_CheckBoxActivated').prop('checked', isActive);
    },
    HighlightOnFocus: function(selector){
        selector.click(function(){
            $(this).select();
        });
    },
    Validation: {
        init: function () {
        //On blur
        $("#MainContent_uc_MemberDetail_TextBoxFN").blur(function(){DBTool.Validation.validateFName(this)});
        $("#MainContent_uc_MemberDetail_TextBoxLN").blur(function(){DBTool.Validation.validateLName(this)});
        $("#MainContent_uc_MemberDetail_TextBoxEA").blur(function(){DBTool.Validation.validateEmail(this)});
        //On key press
        $("#MainContent_uc_MemberDetail_TextBoxFN").keyup(function(){DBTool.Validation.validateFName(this)});
        $("#MainContent_uc_MemberDetail_TextBoxLN").keyup(function(){DBTool.Validation.validateLName(this)});
        },
    }
};

