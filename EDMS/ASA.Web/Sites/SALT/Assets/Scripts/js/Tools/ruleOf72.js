define([
    'jquery',
    'salt',
    'salt/analytics/webtrends'
], function ($, SALT, WTEventLogger) {

    var ruleOf72 = ruleOf72 || {};

    WTEventLogger.subscribe('salt/analytics/webtrends/maps/tools.json');

    ruleOf72.global = {
        utils: {
            /**
             * initializes this object
             *
             * @Function
             * @public
             */
            init: function () {
                // activate global plugins
                ruleOf72.global.utils.plugins.init();
                ruleOf72.global.utils.binds();
                ruleOf72.global.utils.resetInputs();
            },
            /**
             * initializes plugins
             *
             * @Function
             * @public
             */
            plugins: {
                // instantiate
                init: function () {}
            }, // END: plugins
            /**
             * binds slide
             *
             * @Function
             * @public
            */
            binds: function () {
                ruleOf72.global.cachedSelectors.$startButton.on('click', function (e) {

                    WTEventLogger.publish('tool:double_calc:start', {});
                    SALT.publish('content:todo:completed', {
                        contentId: $('.js-todoContainer').attr('data-primary-key'),
                        contentType: $('.js-todoContainer').attr('data-content-type'),
                        contentTitle: $('.js-todoContainer').attr('data-content-title')
                    });

                    e.preventDefault();
                    ruleOf72.global.cachedSelectors.$titleWrapper.fadeOut(500);
                });
                ruleOf72.global.cachedSelectors.$interestRate.on('keyup focus blur', function () {
                    var rate = $(this).val();

                    ruleOf72.global.utils.calculateRule(rate);
                });

                ruleOf72.global.cachedSelectors.$interestRate.on('blur', function () {
                    WTEventLogger.publish('tool:double_calc:complete', {});
                });

            },
            /**
             * calculate total
             *
             * @Function
             * @public
             */
            calculateRule: function (rate) {
                var total = 72 / rate,
                    rounded = Math.round(total * 10) / 10;

                if (ruleOf72.global.utils.isNumber(rate)) {
                    ruleOf72.global.cachedSelectors.$whiteBlock.html(rate + ' %');
                    ruleOf72.global.cachedSelectors.$years.html(rounded + ' years');
                    ruleOf72.global.cachedSelectors.$values.find('p').fadeIn(500);


                } else {
                    ruleOf72.global.cachedSelectors.$whiteBlock.html('');
                    ruleOf72.global.cachedSelectors.$years.html('');
                    ruleOf72.global.cachedSelectors.$values.find('p').fadeOut(500);
                }
                return rounded;


            },
            /**
             * resets all inputs
             *
             * @Function
             * @public
             */
            resetInputs: function () {
                ruleOf72.global.cachedSelectors.$interestRate.val('');
            },
            /**
             * checks if is a number
             *
             * @Function
             * @public
             */
            isNumber: function (rate) {
                return !isNaN(parseFloat(rate)) && isFinite(rate);
            }
        }, // END: utils
        cachedSelectors: {
            $startButton: $('#startBtn'),
            $toolWrap: $('.tool-wrap'),
            $titleWrapper: $('.title-wrapper'),
            $stageWrap: $('.stage-wrap'),
            $slidesWrapper: $('.slides-wrapper'),
            $slides: $('.stage-wrap').find('.slide'),
            $interestRate: $('.slide-left').find('input'),
            $whiteBlock: $('.white-block'),
            $years: $('.years'),
            $values: $('.values')
        }
    }; // END home global var
    $(document).ready(function () {
        // activate utils
        ruleOf72.global.utils.init();
    });

    return ruleOf72;
});
