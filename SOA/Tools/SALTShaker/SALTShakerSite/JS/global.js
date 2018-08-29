var DBTool = DBTool || {};

DBTool.Validation = {
    //validation functions
    validateEmail: function (selector) {
        //testing regular expression
        var email = $(selector);
        var emailInfo = $("#emailInfo");
        var filter = /^[a-zA-Z0-9]+[a-zA-Z0-9_.-]+[a-zA-Z0-9_-]+@[a-zA-Z0-9]+[a-zA-Z0-9.-]+.[a-z]{2,4}$/;
        //if it's valid email
        if (filter.test(email.val())) {
            email.removeClass("error");
            emailInfo.text("");
            emailInfo.removeClass("error");
            return true;
        }
        //if it's NOT valid
        else {
            email.addClass("error");
            emailInfo.text("Please enter a valid email.");
            emailInfo.addClass("error");
            return false;
        }
    },
    validateEmailCanEmpty: function (selector) {
        //testing regular expression
        var email = $(selector);
        var emailInfo = $("#emailInfo");
        var filter = /^$|^[a-zA-Z0-9]+[a-zA-Z0-9_.-]+[a-zA-Z0-9_-]+@[a-zA-Z0-9]+[a-zA-Z0-9.-]+.[a-z]{2,4}$/;
        //if it's valid email
        if (filter.test(email.val())) {
            email.removeClass("error");
            emailInfo.text("");
            emailInfo.removeClass("error");
            return true;
        }
        //if it's NOT valid
        else {
            email.addClass("error");
            emailInfo.text("Please enter a valid email.");
            emailInfo.addClass("error");
            return false;
        }
    },
    validateFName: function (selector) {
        var fname = $(selector);
        var fnameInfo = $("#fNInfo");
        var filter = /[A-Za-z ]+$/;
        //if it's NOT valid
        if (fname.val().length < 1 || fname.val().length > 30) {
            fname.addClass("error");
            fnameInfo.text("First name must be between 1 and 30 characters long.");
            fnameInfo.addClass("error");
            return false;
        }
        else if (!filter.test(fname.val())) {
            fname.addClass("error");
            fnameInfo.text("First name must be letters only.");
            fnameInfo.addClass("error");
            return false;

        }
        //if it's valid
        else {
            fname.removeClass("error");
            fnameInfo.text("");
            fnameInfo.removeClass("error");
            return true;
        }

    },
    validateLName: function (selector) {
        var lname = $(selector);
        var lnameInfo = $("#lNInfo");
        var filter = /[A-Za-z ]+$/;
        //if it's NOT valid
        if (lname.val().length < 1 || lname.val().length > 30) {
            lname.addClass("error");
            lnameInfo.text("Last name must be between 1 and 30 characters long.");
            lnameInfo.addClass("error");
            return false;
        }
        else if (!filter.test(lname.val())) {
            lname.addClass("error");
            lnameInfo.text("Last name must be letters only.");
            lnameInfo.addClass("error");
            return false;

        }
        //if it's valid
        else {
            lname.removeClass("error");
            lnameInfo.text("");
            lnameInfo.removeClass("error");
            return true;
        }

    }
};

DBTool.Controls = {
    isdirtycheckGlobal: false,
    updateGlobalDirty: function () {
        var bToDoText = $('.isdirtycheckToDoText').val() === 'true';
        var bToDoDate = $('.isdirtycheckToDoDate').val() === 'true';
        var bUpdateToDoDate = $('.isdirtycheckUpdateToDoDate').val() === 'true';
        var bUpdateToDoText = $('.isdirtycheckUpdateToDoText').val() === 'true';
        var bUpdateToDoStatus = $('.isdirtycheckUpdateToDoStatus').val() === 'true';

        DBTool.Controls.isdirtycheckGlobal = (bToDoText || bToDoDate || bUpdateToDoDate || bUpdateToDoText || bUpdateToDoStatus);

        if (DBTool.Controls.isdirtycheckGlobal === true) {
            $('.isdirtycheck').val('true');
        }
        else if (DBTool.Controls.isdirtycheckGlobal === false) {
            $('.isdirtycheck').val('false');
        }
    },
        toggleAddToDoTextDirtyFlagValue: function () {
        if ($('.isdirtycheckAddToDoText').val() == 'true') {
            $('.isdirtycheckAddToDoText').val('false');
        } else if ($('.isdirtycheckAddToDoText').val() == 'false') {
            $('.isdirtycheckAddToDoText').val('true');
        }
        DBTool.Controls.updateGlobalDirty();
    },
    toggleAddToDoDateDirtyFlagValue: function () {
        if ($('.isdirtycheckAddToDoDate').val() == 'true') {
            $('.isdirtycheckAddToDoDate').val('false');
        } else if ($('.isdirtycheckAddToDoDate').val() == 'false') {
            $('.isdirtycheckAddToDoDate').val('true');
        }
        DBTool.Controls.updateGlobalDirty();
    },
    toggleUpdateToDoTextDirtyFlagValue: function () {
        if ($('.isdirtycheckUpdateToDoText').val() == 'true') {
            $('.isdirtycheckUpdateToDoText').val('false');
        } else if ($('.isdirtycheckUpdateToDoText').val() == 'false') {
            $('.isdirtycheckUpdateToDoText').val('true');
        }
        DBTool.Controls.updateGlobalDirty();
    },
    toggleUpdateToDoDateDirtyFlagValue: function () {
        if ($('.isdirtycheckUpdateToDoDate').val() == 'true') {
            $('.isdirtycheckUpdateToDoDate').val('false');
        } else if ($('.isdirtycheckUpdateToDoDate').val() == 'false') {
            $('.isdirtycheckUpdateToDoDate').val('true');
        }
        DBTool.Controls.updateGlobalDirty();
    },
    toggleUpdateToDoStatusDirtyFlagValue: function () {
        if ($('.isdirtycheckUpdateToDoStatus').val() == 'true') {
            $('.isdirtycheckUpdateToDoStatus').val('false');
        } else if ($('.isdirtycheckUpdateToDoStatus').val() == 'false') {
            $('.isdirtycheckUpdateToDoStatus').val('true');
        }
        DBTool.Controls.updateGlobalDirty();
    },
    clearAllDirtyFlags: function () {
        $('.isdirtycheck').val('false');
    }
};

//handles the async errors for scriptManager.
DBTool.ScriptManagerAsyncError = {

    pageLoad: function () {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(this.onEndRequest);
    },
    onEndRequest: function (sender, args) {
        if (args.get_error()) {
            var msg = args.get_error().message;
            //alert(msg);
            $('#DivWarning').html(msg);
            args.set_errorHandled(true);
            //see LoadingSpinner.js for more
            DBTool.Spinner.hide();
        }
    }
}

