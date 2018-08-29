define([], function () {
    return {
        freeNumberRegEx: '^\\d+$',
        interestRegEx: '^(\\d{1,2})(\\.\\d{1,2})?$',
        validateInput: function (input, reg) {
            var regex = new RegExp(reg);
            if (!regex.test(input)) {
                return false;
            } else {
                return true;
            }
        },
        extractDropdownDate: function () {
            return {
                day: $('#day').val(),
                month: $('#month').val(),
                year: $('#year').val()
            };
        },
        prepareDate: function (day, month, year) {
            /// <summary>Takes D M Y as parameters and returns a date in the format the SAL will accept (UTC).</summary>
            var dateString = new Date(month + '/' + day + '/' + year);
            var formattedDate = '\/Date(' + dateString.getTime() + ')\/';
            return formattedDate;
        },
        convertDate: function (dateString) {
            return new Date(parseInt(dateString.substr(6), 10));
        },
        openNSLDSwindow: function (url, title, w, h) {
            var left = (screen.width / 2) - (w / 2);
            var top = (screen.height / 2) - (h / 2);
            return window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        }
    };
});
