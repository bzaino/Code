define(['jquery'], function ($) {

    var minLength = 8;

    function containsAlpha(str) {
        var rx = new RegExp(/[a-z]/);
        if (rx.test(str)) { return 1; }
        return 0;
    }
    function containsNumeric(str) {
        var rx = new RegExp(/[0-9]/);
        if (rx.test(str)) { return 1; }
        return 0;
    }
    function containsUpperCase(str) {
        var rx = new RegExp(/[A-Z]/);
        if (rx.test(str)) { return 1; }
        return 0;
    }
    function containsSpecialCharacter(str) {
        var rx = new RegExp(/[\W]/);
        if (rx.test(str)) { return 1; }
        return 0;
    }
    function clearPasswordStrengthClasses($meter, $meterTxt) {
        $meter.removeClass('strong').removeClass('medium').removeClass('weak');
        $meterTxt.removeClass('strong').removeClass('medium').removeClass('weak');
    }
   
    function checkPasswordStrength($meter) {
        var pstr = $meter.val().toString(),
            $meterTxt = $meter.closest('.password-container').find('.meter-txt'),
            result = containsAlpha(pstr) + containsNumeric(pstr) + containsUpperCase(pstr) + containsSpecialCharacter(pstr);
        if (pstr.length < minLength) {
            $meter.removeClass('strong').removeClass('medium').removeClass('weak');
            $meterTxt.removeClass('strong').removeClass('medium').removeClass('weak');
        }
        if (pstr.length > 0) {
            if (result > 3) {
                clearPasswordStrengthClasses($meter, $meterTxt);
                $meter.addClass('strong');
                $meterTxt.addClass('strong');
            } else if (result > 2) {
                clearPasswordStrengthClasses($meter, $meterTxt);
                $meter.addClass('medium');
                $meterTxt.addClass('medium'); 
            } else {
                clearPasswordStrengthClasses($meter, $meterTxt);
                $meter.addClass('weak');
                $meterTxt.addClass('weak');
            }
        }
    }

    $.fn.pschecker = function () {
        this.find('.strong-password').on('keyup blur focus', function () {
            checkPasswordStrength($(this));
        }).closest('form').on('reset', function () {
            var $passwordInput = $(this).find('.strong-password');
            //Empty the value of the password field before triggering the strength indicator, otherwise indicator will continute to show
            $passwordInput.val('');
            checkPasswordStrength($passwordInput);
        });
    };

    $(function () {
        $(document).pschecker();
    });
});
