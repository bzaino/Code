define([
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'jquery.iframe-transport',
    'jquery.fileupload'
], function ($, SALT, SiteMember) {

    var handleIERequest = false;
    if ($.browser.msie && $.browser.version.substr(0, 2) < 10) {
        handleIERequest = true;
    }

    SiteMember.done(function () {
        $('#indId').val(SiteMember.get('IndividualId'));
    });

    SALT.subscribe('loanimport:upload:success', function () {
        //Trigger the loans changed event for widgets that may need to make use of updated data
        SALT.trigger('user:loans:changed');
        //Add/update tasks for KWYO and Repayment Navigator to be "In Progress" on the todo-list
        SALT.trigger('content:todo:inProgress', {contentId: '101-13584'});
        SALT.trigger('content:todo:inProgress', {contentId: '101-8645'});
    });

    SALT.subscribe('loanimport:upload:error', function (data) {
        $('#welcomeMessage').hide();
        if (!data || data.result.ErrorList.length > 0 && data.result.ErrorList[0].DetailMessage === 'Invalid NSLDS file') {
            $('#errorTextInvalid').show();
        }
        else {
            $('#errorTextGeneral').show();
        }
    });

    SALT.mydataupload = {
        checkForLoans: function (data) {
            if (!data.result || data.result.Loans.length === 0) {
                if (handleIERequest) {
                    SALT.mydataupload.fileWasSuccess();
                } else {
                    SALT.mydataupload.uploadErrorhandler();
                }
            } else {
                SALT.mydataupload.fileWasSuccess();
            }
        },

        uploadErrorhandler: function (data) {
            SALT.publish('loanimport:upload:error', data);
        },

        fileWasSuccess: function () {
            SALT.publish('loanimport:upload:success', {
                location: {
                    pathname: 'loanimport/upload.html'
                }
            });
            SALT.trigger('vlcWT:fire', 'complete', 'DoYouKnowHowMuch');
            SALT.trigger('SiteIntercept:run');
        },

        init: function () {
            var userAgent = navigator.userAgent.toString().toLowerCase();
            var isSafari = (userAgent.indexOf('safari') !== -1) && (userAgent.indexOf('chrome') === -1);

            if (!$.browser.msie && !isSafari) {
                $('.mydataupload').bind('click', function () {
                    $('#fileupload').click();
                });

                $('#fileupload').css('display', 'none');
            } else {
                $('#importedFileIE, #fileupload').css('margin-left', '15px');

                // keep this .css change so label doesn't overlap the NSLDS link on browser window resize
                $('#importedFileIE').show().css('margin-bottom', '90px');

                $('.mydataupload').hide();
            }

            if (isSafari) {
                // this condition will show a div to cover up the text "no file selected" on safari
                $('#hideFileSafari').show();
            }

            $('#fileupload').fileupload({
                maxNumberOfFiles: 1,
                dataType: 'json',

                add: function (e, data) {
                    data.context = $('#uploadSubmit').click(function () {
                        $('#errorTextInvalid').hide();
                        $('#errorTextGeneral').hide();

                        var rx = new RegExp(/.txt$/i);
                        if (data.files[0].size > 64000 || !rx.test(data.files[0].name)) {
                            SALT.mydataupload.uploadErrorhandler();
                        } else {
                            data.submit();
                            $('#uploadSubmit').unbind();
                        }
                    });
                },

                done: function (e, data) {
                    if (handleIERequest) {
                        SALT.mydataupload.checkForLoans(data);
                    } else {
                        if (data.result.ErrorList.length > 0 || data.result.Loans.length === 0) {
                            SALT.mydataupload.uploadErrorhandler(data);
                        } else {
                            SALT.mydataupload.checkForLoans(data);
                        }
                    }
                },

                change: function (e, data) {
                    $('#importedFileIE').html('You selected: ' + '<strong>' + data.files[0].name + '</strong>' + '<p></p> Click Submit now!');
                    $('#fileInp').val(data.files[0].name);
                },

                stop: function (e, data) {
                    if (handleIERequest) {
                        SALT.mydataupload.checkForLoans(data);
                    }
                }
            });
        }
    };

    $(function () {
        SALT.mydataupload.init();
    });

    return SALT.mydataupload;
});
