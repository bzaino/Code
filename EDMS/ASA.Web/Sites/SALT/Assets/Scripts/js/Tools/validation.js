var validation = validation || {};

validation.global = {
    utils: {
        init: function () {
            //cache dom elements
            var $inputs = $('input.tool-input');

            $inputs.on('keyup', function () {
                var inputVal = this.value,
                    self = $(this);
                validation.global.utils.validateIt(inputVal, self);
            });

        },
        validateIt: function (inputVal, self) {

            if (self.hasClass('numbers')) {
                self.val(validation.global.utils.validateNumber(inputVal));
            } else if (self.hasClass('percentage')) {
                self.val(validation.global.utils.validateDecimal(inputVal));
            }

            //maybe if empty grab value from data attribute
        },
        validateNumber: function (inputVal) {

            //Multiple dot
            var idx = inputVal.indexOf('.');
            if (idx > -1) {
                idx = inputVal.indexOf('.');
                if (inputVal.indexOf('.', idx + 1) > idx) {
                    inputVal = inputVal.slice(0, -1);
                }
            }
            inputVal = inputVal.replace(/[^0-9\.]/g, '');
            return inputVal;


        },
        validateDecimal: function (inputVal) {
            var regex = /^([0-9]{1,2}((\.[0-9]{1,2})|(\.))?)/;
            var match = regex.exec(inputVal);
            if (match !== null) {
                return match[1];
            } else {
                return '';
            }
        }

    } // END: utils
}; // END home global var

$(document).ready(function () {
    // activate utils
    validation.global.utils.init();
});