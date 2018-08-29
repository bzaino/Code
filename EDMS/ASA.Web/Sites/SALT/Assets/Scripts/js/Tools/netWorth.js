require(['jquery', 'salt', 'salt/analytics/webtrends'], function ($, SALT, WTEventLogger) {

    var netWorth = netWorth || {};

    WTEventLogger.subscribe('salt/analytics/webtrends/maps/tools.json');

    netWorth.global = {
        utils: {
            /**
             *  Initiates module
             *
             * @Function
             * @public
             */
            init: function () {
                // activate global plugins
                netWorth.global.utils.binds();
                netWorth.global.utils.setUpSlides();
                netWorth.global.utils.resetInputs();


            },
            /**
             * sets up and caches valus for slider
             *
             * @Function
             * @public
             */
            setUpSlides: function () {
                var selectors = netWorth.global.cachedSelectors,
                    data = netWorth.global.cachedValues;

                data.slideWid = selectors.$toolWrap.outerWidth();
                data.slideNum = selectors.$slides.length;
                data.totalWidth = data.slideWid * data.slideNum;
                selectors.$slides.add(selectors.$stageWrap).width(data.slideWid);
                selectors.$slidesWrapper.width(data.totalWidth);
                selectors.$slidesWrapper.css({
                    'left': (data.idx * data.slideWid) * -1
                });
            },
            /**
             * resets all inputs
             *
             * @Function
             * @public
             */
            resetInputs: function () {
                var selectors = netWorth.global.cachedSelectors;

                selectors.$checkboxes.removeClass('checked');
                selectors.$assetsInputs.val('');
                selectors.$liabilityInputs.val('');
                selectors.$addedAssets.val('');
                selectors.$toolWrap.find('.bold').removeClass('bold');
                selectors.$toolWrap.find('.js-to-hide').hide();
                selectors.$additionalInput.hide();
                selectors.$additionalInput.find('.cbox').show().addClass('checked');
                selectors.$additionalInput.find('.js-to-hide').show();
                selectors.$addButton.show();
                selectors.$slidesWrapper.css({
                    left: '0'
                });
                selectors.$printTotalLiability.html('$0');
                selectors.$printTotalAssets.html('$0');
                selectors.$negative.hide();
                selectors.$positive.hide();

                $('div.assets').find('form').show();
                $('div.liabilities').find('form').hide();
                $('div.liabilities').find('a').hide();
                netWorth.global.cachedValues.idx = 0;

            },
            /**
             * gets totals for inputs
             *
             * @Function
             * @public
             */
            totalInputs: function (type) {
                var assets = 0,
                    liabilities = 0,
                    checkNumber, selectors = netWorth.global.cachedSelectors;
                if (type === 'assets') {
                    selectors.$assetsInputs.each(function () {
                        assets = assets + $(this).asNumber();
                    });
                    netWorth.global.totals.totalAssets = assets;
                } else if (type === 'liabilities') {
                    selectors.$liabilityInputs.each(function () {
                        liabilities = liabilities + $(this).asNumber();
                    });
                    netWorth.global.totals.totalLiability = liabilities;
                } else if (type === 'printMeAssets') {
                    var totals = 0;
                    selectors.$assetsInputs.each(function () {
                        checkNumber = $(this).asNumber();
                        //isNaN(thisVal) ? 0 : thisVal;
                        totals = Number(totals) + Number(checkNumber);
                    });
                    selectors.$printTotalAssets.html('$' + totals).formatCurrency({
                        roundToDecimalPlace: 0
                    });

                } else if (type === 'printMeLiability') {
                    var totalLiability = 0;
                    selectors.$liabilityInputs.each(function () {
                        checkNumber = $(this).asNumber();
                        totalLiability = Number(totalLiability) + Number(checkNumber);
                    });
                    selectors.$printTotalLiability.html(totalLiability).formatCurrency({
                        roundToDecimalPlace: 0
                    });
                }

            },
            /**
             * binds all buttons
             *
             * @Function
             * @public
             */
            binds: function () {
                var selectors = netWorth.global.cachedSelectors;
                selectors.$startButton.off('click').on('click', function () {

                    WTEventLogger.publish('tool:networth_calc:start', {});
                    SALT.publish('content:todo:completed', {
                        contentId: $('.js-todoContainer').attr('data-primary-key'),
                        contentType: $('.js-todoContainer').attr('data-content-type'),
                        contentTitle: $('.js-todoContainer').attr('data-content-title')
                    });

                    selectors.$titleWrapper.fadeOut(500);
                });
                $(selectors.$checkboxes).on('click', function () {
                    $(this).toggleClass('checked');
                    $(this).parent().next('.right').fadeToggle(500).find('input').focus();
                    $(this).parent().find('label').toggleClass('bold');
                    if (!$(this).hasClass('checked')) {
                        $(this).parent().next('.right').find('input.numbers').val('');
                    }
                });

                selectors.$labels.on('click', function (e) {
                    e.preventDefault();
                    $(this).toggleClass('bold');
                    $(this).parent().next('.right').fadeToggle(500).find('input').focus();
                    $(this).prev('.cbox').toggleClass('checked');
                    if (!$(this).closest('.cbox').hasClass('checked')) {
                        $(this).parent().next('.right').find('input.numbers').val('');
                    }



                });
                selectors.$addButton.on('click', function (e) {
                    e.preventDefault();
                    // selector could just be the label
                    var $addButton = $(this).parent().find('.add');
                    var $label = $(this).parent().find('label');
                    $addButton.add($label).hide();

                    $addButton.prev('div.additional-input').fadeIn(500).find('.added-assets').focus();

                    selectors.$additionalInput.find('.right').fadeIn(500);
                });
                selectors.$nextButton.on('click', function (e) {
                    e.preventDefault();
                    var type = $(this).attr('data'),
                        parent = $(this).closest('div.slide');
                    netWorth.global.utils.totalInputs(type);
                    netWorth.global.utils.slideNext(type, parent);
                });
                selectors.$previousButton.on('click', function (e) {
                    e.preventDefault();
                    var type = $(this).attr('data'),
                        parent = $(this).closest('div.slide');
                    netWorth.global.utils.slideNext(type, parent);
                });

                selectors.$assetsInputs.on('keyup focus blur', function () {
                    $(this).toNumber().formatCurrency({
                        roundToDecimalPlace: 0
                    });
                    netWorth.global.utils.totalInputs('printMeAssets');
                });
                selectors.$liabilityInputs.on('keyup focus blur', function () {
                    $(this).toNumber().formatCurrency({
                        roundToDecimalPlace: 0
                    });
                    netWorth.global.utils.totalInputs('printMeLiability');
                });

                selectors.$liabilityInputs.on('blur', function () {
                    var liabFieldValue = $(this).attr('value') ? $(this).attr('value') : '$0';
                    var liabFieldName = $(this).attr('name');

                    WTEventLogger.publish('tool:networth_calc:liability', {
                        fieldName: liabFieldName,
                        fieldValue: liabFieldValue
                    });
                });

                selectors.$assetsInputs.on('blur', function () {
                    var assetFieldValue = $(this).attr('value') ? $(this).attr('value') : '$0';
                    var assetFieldName = $(this).attr('name');

                    WTEventLogger.publish('tool:networth_calc:asset', {
                        fieldName: assetFieldName,
                        fieldValue: assetFieldValue
                    });
                });

                selectors.$startOver.on('click', function () {
                    netWorth.global.utils.resetInputs();
                    selectors.$scoreSlide.hide();
                    selectors.$titleWrapper.show();

                });

                $(window).on('resize', netWorth.global.utils.setUpSlides);
            },
            /**
             * animates to next slide
             *
             * @Function
             * @public
             */
            slideNext: function (type, parent) {
                var selectors = netWorth.global.cachedSelectors,
                    data = netWorth.global.cachedValues;

                if (type === 'assets') {
                    parent.find('form').fadeOut(500);
                    parent.next('div.slide').find('form').fadeIn(500);
                    parent.next('div.slide').find('a').show();
                    data.idx = data.idx + 1;
                    selectors.$slidesWrapper.animate({
                        'left': '+=' + data.slideWid * -1
                    }, 500);
                } else if (type === 'prev') {
                    parent.find('form').fadeOut(500);
                    parent.find('a').hide();
                    parent.prev('div.slide').find('form').fadeIn(500);
                    data.idx = data.idx - 1;
                    selectors.$slidesWrapper.animate({
                        'left': '+=' + data.slideWid
                    }, 500);
                } else {

                    WTEventLogger.publish('tool:networth_calc:complete', {});
                    selectors.$addButton.hide();
                    selectors.$additionalInput.hide();
                    var totaledUp = netWorth.global.totals.totalAssets - netWorth.global.totals.totalLiability;
                    if (totaledUp < 0) {
                        totaledUp = totaledUp * -1;
                        selectors.$totalNetWorth.html('-' + totaledUp).formatCurrency({
                            roundToDecimalPlace: 0,
                            negativeFormat: '-$%n'
                        });
                        selectors.$negative.show();

                    } else {
                        selectors.$totalNetWorth.html(totaledUp).formatCurrency({
                            roundToDecimalPlace: 0
                        });
                        selectors.$positive.show();
                    }
                    selectors.$scoreSlide.fadeIn(500);
                }
            }

        },
        // END: utils
        /**
         * Cached selectors and values
         *
         * @Function
         * @public
         */
        cachedSelectors: {
            $startButton: $('#startBtn'),
            $toolWrap: $('.tool-wrap'),
            $titleWrapper: $('.title-wrapper'),
            $stageWrap: $('.stage-wrap'),
            $slidesWrapper: $('.slides-wrapper'),
            $slides: $('.stage-wrap').find('div.slide'),
            $checkboxes: $('.cbox'),
            $labels: $('.netWorthCalculator label'),
            $addButton: $('.add, .add + label'),
            $additionalInput: $('.additional-input'),
            $nextButton: $('.next-btn'),
            $previousButton: $('a.prev-btn'),
            $assetsInputs: $('.assets').find('input.tool-input'),
            $printTotalAssets: $('.assets').find('.print-total span'),
            $liabilityInputs: $('.liabilities').find('input.tool-input'),
            $printTotalLiability: $('.liabilities').find('.print-total span'),
            $totalNetWorth: $('.total-net-worth'),
            $scoreSlide: $('.scoreSlide'),
            $startOver: $('.start-over'),
            $negative: $('.negative'),
            $positive: $('.positive'),
            $addedAssets: $('.added-assets')


        },
        cachedValues: {
            idx: 0
        },
        totals: {}

    }; // END home global var

    $(document).ready(function () {
        // activate utils
        netWorth.global.utils.init();
    });
});
