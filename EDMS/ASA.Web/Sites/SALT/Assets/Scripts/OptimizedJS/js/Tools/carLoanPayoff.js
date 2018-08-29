require([
    'jquery',
    'salt',
    'salt/analytics/webtrends',
], function ($, SALT, WTEventLogger) {

    var carLoanPayoff = carLoanPayoff || {};

    WTEventLogger.subscribe('salt/analytics/webtrends/maps/tools.json');

    carLoanPayoff.global = {
        utils: {
            /**
            * inititializes payoff object
            *
            * @Function
            * @public
            */
            init: function () {
                // activate global plugins
                carLoanPayoff.global.utils.binds();
                carLoanPayoff.global.utils.resetFields();
            },
            /**
            * Binds tool
            *
            * @Function
            * @public
            */
            binds: function () {
                var selectors = carLoanPayoff.global.cachedSelectors;

                selectors.$startButton.on('click', function (e) {

                    //Publish start event on click of "Start" button
                    WTEventLogger.publish('tool:carloan_calc:start', {});
                    SALT.publish('content:todo:completed', {
                        contentId: $('.js-todoContainer').attr('data-primary-key'),
                        contentType: $('.js-todoContainer').attr('data-content-type'),
                        contentTitle: $('.js-todoContainer').attr('data-content-title')
                    });

                    e.preventDefault();
                    selectors.$titleWrapper.fadeOut(500);
                });

                selectors.$checkbox.on('click', function (e) {
                    e.preventDefault();
                    var enabler = $(this).next('.tool-input');
                    $(this).toggleClass('checked');
                    $(this).parent('.principle').toggleClass('enabled');
                    if (enabler.attr('disabled') === 'disabled') {
                        enabler.attr('disabled', false);
                        enabler.focus();
                    } else {
                        enabler.val('');
                        enabler.attr('disabled', true);
                    }

                });

                //Adds blur listener for $inputs 
                //WT complete event will be fired when the user leaves an input
                selectors.$inputs.on('blur', function () {
                    WTEventLogger.publish('tool:carloan_calc:complete', {});
                });

                selectors.$months.on('change', function () {
                    carLoanPayoff.global.utils.calculateIt();
                });

                selectors.$inputs.on('keyup focus blur', function () {
                    if ($(this).hasClass('money')) {
                        $(this).toNumber().formatCurrency({
                            roundToDecimalPlace: 0
                        });
                    } else if ($(this).hasClass('percent')) {
                        this.value = this.value + '%';
                    }
                    carLoanPayoff.global.utils.calculateIt();
                });

            },
            /**
            * Calculates cost
            *
            * @Function
            * @public
            */
            calculateIt: function () {

                var selectors = carLoanPayoff.global.cachedSelectors,
                    price = selectors.$price.asNumber(),
                    interestRate = Number(selectors.$interestRate.val()) / (100 * 12);
                var downPayment = selectors.$downPayment.asNumber(),
                    principle = price - downPayment,
                    time = Number($(selectors.$months.selector + ' :selected').val()),
                    total = (time * ((principle * (interestRate)) / (1 - Math.pow((1 + interestRate), (-1 * time))))),
                    interest = total - principle;

                if (selectors.$inactiveWrapper.is(':visible')) {
                    selectors.$inactiveWrapper.fadeOut(500);
                    selectors.$activeWrapper.fadeIn(500);
                }
                if (interest < 0 || isNaN(interest) || isNaN(total)) {
                    selectors.$total.html('$0');
                    selectors.$interest.html('$0');
                } else {
                    selectors.$total.html('$' + total).toNumber().formatCurrency({
                        roundToDecimalPlace: 0
                    });
                    selectors.$interest.html('$' + interest).toNumber().formatCurrency({
                        roundToDecimalPlace: 0
                    });
                    carLoanPayoff.global.utils.checkLength();

                }
            },
            /**
            * checks length of number and adjusts font size
            *
            * @Function
            * @public
            */
            checkLength: function () {
                var parent = carLoanPayoff.global.cachedSelectors.$total.closest('.totals-wrapper'),
                    totalLen = carLoanPayoff.global.cachedSelectors.$total.text().length,
                    interestLen = carLoanPayoff.global.cachedSelectors.$interest.text().length,
                    largest = Math.max(totalLen, interestLen);



                if (largest > 11) {
                    parent.addClass('smallest');
                    parent.removeClass('smaller');
                } else if (largest > 8 && largest <= 11) {
                    parent.addClass('smaller');
                    parent.removeClass('smallest');
                } else if (largest < 8) {
                    parent.removeClass('smallest');
                    parent.removeClass('smaller');
                }
            },
            /**
            * resets all fields on page load
            *
            * @Function
            * @public
            */
            resetFields: function () {
                carLoanPayoff.global.cachedSelectors.$inputs.val('');
                carLoanPayoff.global.cachedSelectors.$downPayment.attr('disabled', true);
            },
        },
        // END: utils
        cachedSelectors: {
            $startButton: $('#startBtn'),
            $toolWrap: $('.tool-wrap'),
            $titleWrapper: $('.title-wrapper'),
            $stageWrap: $('.stage-wrap'),
            $slidesWrapper: $('.slides-wrapper'),
            $slides: $('.stage-wrap').find('div.slide'),
            $months: $('.months-drop'),
            $price: $('input[name="price-of-car"]'),
            $interestRate: $('input[name="interest-rate"]'),
            $downPayment: $('input[name="down-payment"]'),
            $inputs: $('input[type="text"]'),
            $total: $('.total'),
            $interest: $('.interest'),
            $inactiveWrapper: $('.inactive-wrapper'),
            $activeWrapper: $('.active-wrapper'),
            $checkbox: $('.carLoanCheck')

        }
    }; // END home global var
    $(document).ready(function () {


        // activate utils
        carLoanPayoff.global.utils.init();
    });
});