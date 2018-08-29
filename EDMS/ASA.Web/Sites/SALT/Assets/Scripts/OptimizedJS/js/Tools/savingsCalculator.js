define(['jquery', 'salt', 'salt/analytics/webtrends'], function ($, SALT, WTEventLogger) {

    var savingsCalculator = savingsCalculator || {};

    WTEventLogger.subscribe('salt/analytics/webtrends/maps/tools.json');

    savingsCalculator.global = {
        utils: {

            init: function () {

                var values = {};
                savingsCalculator.global.utils.resetValues();
                savingsCalculator.global.utils.binds(values);
            },
            /**
             * binds slide
             *
             * @Function
             * @public
             */
            binds: function (values) {
                var selectors = savingsCalculator.global.cachedSelectors;

                selectors.$inputs.on('keyup focus blur', function () {
                    var keyboardDelay;

                    if ($(this).hasClass('money')) {
                        $(this).toNumber().formatCurrency({
                            roundToDecimalPlace: 0
                        });
                    }

                    clearTimeout(keyboardDelay);
                    keyboardDelay = setTimeout(function () {
                        savingsCalculator.global.utils.calculateIt(values);
                    }, 500);
                });

                selectors.$inputs.on('blur', function () {
                    var val = $(this).val();
                    if (savingsCalculator.global.utils.reset(val)) {
                        $(this).val(0);
                    }

                    WTEventLogger.publish('tool:savings_calc:complete', {
                        whichField: $(this).attr('name')
                    });
                });

                selectors.$paymentSchedule.on('change', function () {
                    savingsCalculator.global.utils.calculateIt(values);
                });

                selectors.$startButton.on('click', function (e) {

                    WTEventLogger.publish('tool:savings_calc:start', {});
                    SALT.publish('content:todo:completed', {
                        contentId: $('.js-todoContainer').attr('data-primary-key'),
                        contentType: $('.js-todoContainer').attr('data-content-type'),
                        contentTitle: $('.js-todoContainer').attr('data-content-title')
                    });

                    e.preventDefault();
                    selectors.$titleWrapper.fadeOut(500);
                    $('.slide-left').css({
                        'visibility': 'visible'
                    });

                });

            },
            /**
             * gets all values for inputs
             *
             * @Function
             * @public
             */
            getValues: function (values) {
                var selectors = savingsCalculator.global.cachedSelectors;

                values.interestRate = Number(selectors.$interestRateInput.val());
                values.onePayment = selectors.$oneTimePaymentInput.asNumber();
                values.payment = selectors.$paymentInput.asNumber();
                values.schedule = $(selectors.$paymentSchedule.selector + ' :selected').val();
                return values;
            },
            /**
             * calculates totals
             *
             * @Function
             * @public
             */
            calculateIt: function (values) {
                var selectors = savingsCalculator.global.cachedSelectors;

                values = savingsCalculator.global.utils.getValues(values);
                //move this out of keyup
                savingsCalculator.global.utils.setMonthlyPayment(values);


                var currentAmount = values.onePayment,
                    nodes = {
                        '60': selectors.$five,
                        '120': selectors.$ten,
                        '240': selectors.$twenty
                    },
                    totals = {};


                totals.interestRate = (values.interestRate / 100) / 12;
                totals.currentAmount = currentAmount;
                totals.currentInterest = totals.currentAmount * totals.interestRate;
                totals.totalInterest = totals.currentAmount * totals.interestRate;

                totals.currentAmount = totals.currentAmount + totals.currentInterest + values.monthlyDeposit;
                //add one for the inital interest above
                for (var i = 2; i <= 240; i++) {

                    totals = savingsCalculator.global.utils.calculateInterest(totals, values);

                    if (i in nodes) {
                        if (!isNaN(totals.totalInterest)) {
                            nodes[i].find('p.total-interest').text(totals.totalInterest).toNumber().formatCurrency({
                                roundToDecimalPlace: 0
                            });
                            nodes[i].find('p.total').text(totals.currentAmount).toNumber().formatCurrency({
                                roundToDecimalPlace: 0
                            });
                            savingsCalculator.global.utils.checkLength(nodes[i].find('p.total-interest'));
                            savingsCalculator.global.utils.checkLength(nodes[i].find('p.total'));

                        } else {
                            nodes[i].find('p.total-interest').text('$0');
                            nodes[i].find('p.total').text('$0');
                        }
                    }

                }
                if (selectors.$inactiveWrapper.is(':visible')) {
                    selectors.$inactiveWrapper.fadeOut(500);
                    selectors.$activeWrapper.fadeIn(500);
                }
            },
            /**
             * checks length of number and adjusts font size
             *
             * @Function
             * @public
             */
            checkLength: function (node) {
                var textLength = node.text().length;

                if (textLength > 8) {
                    node.closest('.totals-group ').addClass('smaller');
                } else if ($('.totals-group ').hasClass('smaller')) {
                    $('.totals-group ').removeClass('smaller');
                }

            },
            /**
             * calculates compound interest
             *
             * @Function
             * @public
             */
            calculateInterest: function (totals, values) {
                totals.currentInterest = totals.currentAmount * totals.interestRate;
                totals.currentAmount = totals.currentAmount + values.monthlyDeposit + totals.currentInterest;
                totals.totalInterest = totals.totalInterest + totals.currentInterest;
                return totals;
            },
            setMonthlyPayment: function (values) {
                if (values.schedule === 'week') {
                    values.monthlyDeposit = values.payment * 4;
                } else if (values.schedule === 'two-weeks') {
                    values.monthlyDeposit = values.payment * 2;

                } else if (values.schedule === 'month') {
                    values.monthlyDeposit = values.payment;

                } else if (values.schedule === 'three-months') {
                    values.monthlyDeposit = values.payment * 4 / 12;

                } else if (values.schedule === 'six-months') {
                    values.monthlyDeposit = values.payment * 2 / 12;

                } else if (values.schedule === 'year') {
                    values.monthlyDeposit = values.payment / 12;
                }
                return values;

            },
            /**
             * resets all values
             *
             * @Function
             * @public
             */
            resetValues: function () {
                var selectors = savingsCalculator.global.cachedSelectors;

                selectors.$inputs.val('');
                selectors.$checkBoxes.attr('checked', false);

            },
            /**
             * resets inputs that are empty
             *
             * @Function
             * @public
             */
            reset: function (val) {
                if (val.replace(/ /g, '') === '') {
                    return true;
                }
            },
        },
        // END: utils
        cachedSelectors: {
            $startButton: $('#startBtn'),
            $titleWrapper: $('div.title-wrapper'),
            $inputs: $('input.tool-input'),
            $interestRateInput: $('input[name="rate"]'),
            $oneTimePaymentInput: $('input[name="onetime"]'),
            $paymentInput: $('input[name="recurring"]'),
            $checkBoxes: $('.savingsCalculator input[type="checkbox"]'),
            $paymentSelect: $('#payment-schedule :selected'),
            $paymentSchedule: $('#payment-schedule'),
            $five: $('#five-years'),
            $ten: $('#ten-years'),
            $twenty: $('#twenty-years'),
            $inactiveWrapper: $('div.inactive-wrapper'),
            $activeWrapper: $('div.active-wrapper')

        } // END: cachedSelectors
    }; // END home global var

    $(document).ready(function () {
        // activate utils
        savingsCalculator.global.utils.init();
    });

    return savingsCalculator;
});
