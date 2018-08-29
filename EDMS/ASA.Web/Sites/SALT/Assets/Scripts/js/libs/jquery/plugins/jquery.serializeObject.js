(function ($) {
    $.fn.serializeObject = function () {
        'use strict';

        var result = {};
        var extend = function (i, element) {
            var node = result[element.name];

            // If node with same name exists already, need to convert it to an array as it
            // is a multi-value field (i.e., checkboxes)

            if ('undefined' !== typeof node && node !== null) {
                if ($.isArray(node)) {
                    node.push(element.value);
                } else {
                    result[element.name] = [node, element.value];
                }
            } else {
                var isProfileQuestion = element.name.indexOf('-qid-');
                if (element.value || isProfileQuestion === -1) {
                    // a regex that accepts (YYYY/MM/DD) or (YYYY-MM-DD)
                    var dateReg = /\d{4}[.\/-]\d{2}[.\/-]\d{2}$/,
                        treatAsString = /USPostalCode|OECode|OEBranch/.test(element.name);
                    if (dateReg.test(element.value)) {
                        var year = element.value.substr(0, 4);
                        var month = element.value.substr(5, 2) - 1;
                        var day = element.value.substr(8, 2);
                        var date = new Date(year, month, day);
                        result[element.name] = '/Date(' + Date.parse(date) + ')/';
                    } else if (!isNaN(element.value) && !treatAsString) {
                        result[element.name] = parseFloat(element.value, 10);
                    } else {
                        result[element.name] = element.value || null;
                    }
                }
            }
        };

        $.each(this.serializeArray(), extend);
        return result;
    };
})(jQuery);
